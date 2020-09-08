using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using static InstructionType;

public class UIHandler : MonoBehaviour
{

    public Tape tape;

    public string filename = null;
    public bool autoFileName;

    public List<Instruction> instructions;
    public GameObject instructionObject;
    public Transform instructionListSpawnPoint;
    public RectTransform content;

    public Sprite gotoSprite;
    public Sprite writeSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite breakSprite;
    public Sprite incrementSprite;
    public Sprite decrementSprite;

    public Instruction selectedInstruction;

    // Start is called before the first frame update
    void Start()
    {
      instructions = new List<Instruction>();
      selectedInstruction = null;

      if(autoFileName){
        filename = LevelHandler.lh.toLoad.levelName + ".txt";
      }
      fileToUI(filename);
    }

    // Update is called once per frame
    void Update()
    {
      if(selectedInstruction != null && (Input.GetKeyDown(KeyCode.Delete))){
        instructions.RemoveAt(selectedInstruction.index);
        updatePositions(selectedInstruction.index);
        Destroy(selectedInstruction.gameObject);
      }
    }

    void fileToUI(string filename){
      FileInfo theSourceFile = new FileInfo("Assets/Resources/" + filename);
      StreamReader reader = theSourceFile.OpenText();

      string line = "";

      while(reader.Peek() > 0){
        line = reader.ReadLine();
        if(line.Length < 1){
          continue;
        }
        string[] words = line.Split(' ');
        if(words[0].Substring(0, 1) == "#")
        {
          addInstructionToList(Comment);
          continue;
        }
        switch(words[0]){
          case "left":
            addInstructionToList(Left);
            break;
          case "right":
            addInstructionToList(Right);
            break;
          case "goto":
            Instruction cpmg = addInstructionToList(Goto);
            cpmg.UISetUserContent(words[1]);
            break;
          case "gotoif":
            Instruction cpmgi = addInstructionToList(GotoIf);
            cpmgi.UISetUserContent(words[1]);
            cpmgi.UISetExtraUserContent(words[2].Replace("B", "%"));
            break;
          case "break":
            addInstructionToList(Break);
            break;
          case "write":
            Instruction cpmw = addInstructionToList(Write);
            if(words.Length > 1){
              cpmw.UISetUserContent(words[1].Replace("B", "%"));
            }
            break;
          case "increment":
            addInstructionToList(Increment);
            break;
          case "decrement":
            addInstructionToList(Decrement);
            break;
        }
      }
    }

    bool interruptFlag = false;

    public void execute(){
      interruptFlag = false;
      StartCoroutine(executeEnum());
    }

    IEnumerator executeEnum(){
      if(tape.executing){
        tape.interrupt();
      }
      while(tape.executing){
        yield return null;
      }
      StartCoroutine(tape.executeInstructions(instructions.ToArray()));
    }

    public void submit(){
      interrupt();
      StartCoroutine(submitEnum());
    }

    IEnumerator submitEnum(){
      LevelHandler.lh.currentTapeIndex = -1;
      Debug.Log(LevelHandler.lh.toLoad.levelName);
      bool incorrect = false;
      interruptFlag = false;

      while(LevelHandler.lh.loadNextTape()){
        if(interruptFlag){
          break;
        }
        StartCoroutine(tape.executeInstructions(instructions.ToArray()));
        while(tape.executing){
          yield return null;
        }
        if(!LevelHandler.lh.validateAnswer()){
          incorrect = true;
          while(tape.incorrect.isPlaying){
            yield return null;
          }
          Debug.Log("Failed on test " + (LevelHandler.lh.currentTapeIndex + 1));
          break;
        }
        else{
          while(tape.correct.isPlaying){
            yield return null;
          }
          Debug.Log("Test " + (LevelHandler.lh.currentTapeIndex + 1) + " complete!");
        }
      }

      if(!incorrect && !interruptFlag){
        Debug.Log("Level " + LevelHandler.lh.levelIndex + " complete!");
        if(LevelHandler.lh.levels.Count > LevelHandler.lh.levelIndex){
          LevelHandler.lh.nextLevelButton.SetActive(true);
        }
        else{
          Debug.Log("Out of Levels!");
        }
      }
    }

    public void interrupt(){
      interruptFlag = true;
      tape.interrupt();
    }

    public void reset(){
      StartCoroutine(resetEnum());
      interruptFlag = false;
    }

    IEnumerator resetEnum(){
      tape.interrupt();
      while(tape.executing){
        yield return null;
      }
      tape.reinitialize();
    }

    void updatePositions(int startIndex){
      for(int j = startIndex; j < instructions.Count; j++){
        Instruction i = instructions[j];

        i.index = j;
        i.text.text = j + ") " + i.InstructionType;

        RectTransform rt = i.gameObject.GetComponent<RectTransform>();

        Vector3 pos = rt.anchoredPosition;
        pos = new Vector3(pos.x, pos.y + 65f, pos.z);

        rt.anchoredPosition = pos;
      }
    }

    public void selectInstruction(Instruction inst){
      selectedInstruction = inst;
    }

    public void addInstructionByString(string str){
      switch(str){
        case "goto":
          addInstructionToList(Goto);
          break;
        case "comment":
          addInstructionToList(Comment);
          break;
        case "gotoif":
          addInstructionToList(GotoIf);
          break;
        case "break":
          addInstructionToList(Break);
          break;
        case "write":
          addInstructionToList(Write);
          break;
        case "increment":
          addInstructionToList(Increment);
          break;
        case "decrement":
          addInstructionToList(Decrement);
          break;
        case "left":
          addInstructionToList(Left);
          break;
        case "right":
          addInstructionToList(Right);
          break;
      }
    }

    public Instruction addInstructionToList(InstructionType instruction){
      content.sizeDelta = new Vector2(0, 60 * (instructions.Count + 1));

      float spawnY = 495 - (65 * (instructions.Count + 1));
      float spawnX = 0f;
      Vector3 pos = new Vector3(spawnX, spawnY, instructionListSpawnPoint.position.z);
      GameObject spawnedInstruction = (GameObject)Instantiate(instructionObject, pos, instructionListSpawnPoint.rotation);
      spawnedInstruction.transform.SetParent(instructionListSpawnPoint, false);
      spawnedInstruction.name = instruction.ToString() + (instructions.Count);

      Instruction instructionComponent = spawnedInstruction.GetComponent<Instruction>();
      instructionComponent.InstructionType = instruction;
      instructions.Add(instructionComponent);

      instructionComponent.text.text = (instructions.Count - 1) + ") " + instruction;
      instructionComponent.index = instructions.Count - 1;

      Button instructionButton = spawnedInstruction.GetComponent<Button>();
      instructionButton.onClick.AddListener(delegate {selectInstruction(instructionComponent);});

      InputField inputField = instructionComponent.first;
      InputField second = instructionComponent.second;

      inputField.onValidateInput += delegate (string s, int i, char c) { return char.ToUpper(c); };

      switch(instruction){
        case Goto:
          instructionComponent.image.color = new Color32(84, 13, 110, 150);
          instructionComponent.sprite.sprite = gotoSprite;

          inputField.placeholder.GetComponent<Text>().text = "Line";
          inputField.contentType = InputField.ContentType.IntegerNumber;

          second.gameObject.SetActive(false);

          break;
        case Comment:
          instructionComponent.image.color = new Color32(150, 150, 150, 150);
          instructionComponent.sprite.sprite = null;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case GotoIf:
          instructionComponent.image.color = new Color32(255, 210, 63, 150);
          instructionComponent.sprite.sprite = gotoSprite;

          inputField.placeholder.GetComponent<Text>().text = "Line";
          inputField.contentType = InputField.ContentType.IntegerNumber;

          second.placeholder.GetComponent<Text>().text = "Symbol";

          break;
        case Break:
          instructionComponent.image.color = new Color32(255, 0, 0, 150);
          instructionComponent.sprite.sprite = breakSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case Write:
          instructionComponent.image.color = new Color32(59, 206, 172, 150);
          instructionComponent.sprite.sprite = writeSprite;

          inputField.placeholder.GetComponent<Text>().text = "Value";
          inputField.characterLimit = 1;
          second.gameObject.SetActive(false);

          break;
        case Increment:
          instructionComponent.image.color = new Color32(14, 173, 105, 150);
          instructionComponent.sprite.sprite = incrementSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case Decrement:
          instructionComponent.image.color = new Color32(14, 173, 105, 150);
          instructionComponent.sprite.sprite = decrementSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case Left:
          instructionComponent.image.color = new Color32(25, 123, 189, 150);
          instructionComponent.sprite.sprite = leftSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case Right:
          instructionComponent.image.color = new Color32(25, 123, 189, 150);
          instructionComponent.sprite.sprite = rightSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
      }
      return instructionComponent;
    }
}
