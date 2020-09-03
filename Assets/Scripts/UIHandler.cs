using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UIHandler : MonoBehaviour
{

    public Tape tape;

    public string filename = null;

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

      if(filename != null){
        fileToUI(filename);
      }
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
        if(words[0].Substring(0, 1) == "#"){
          comment();
          continue;
        }
        switch(words[0]){
          case "left":
            left();
            break;
          case "right":
            right();
            break;
          case "goto":
            Instruction cpmg = addInstructionToList("Goto");
            cpmg.setContentString(words[1]);
            break;
          case "gotoif":
            Instruction cpmgi = addInstructionToList("GotoIf");
            cpmgi.setContentString(words[1]);
            cpmgi.setExtraContentString(words[2].Replace("B", ""));
            break;
          case "break":
            br();
            break;
          case "write":
            Instruction cpmw = addInstructionToList("Write");
            if(words.Length > 1){
              cpmw.setContentString(words[1].Replace("B", ""));
            }
            break;
          case "increment":
            increment();
            break;
          case "decrement":
            decrement();
            break;
        }
      }
    }

    public void execute(){
      StartCoroutine(executeEnum());
    }

    IEnumerator executeEnum(){
      if(tape.executing){
        tape.interrupt();
      }
      LevelHandler.lh.loadTapeN(0);
      StartCoroutine(tape.executeInstructions(instructions.ToArray()));
      while(tape.executing){
        yield return null;
      }
      if(!tape.interruptFlag){
        while(tape.click.isPlaying || tape.pencilScratch.isPlaying || tape.zoom.isPlaying){
          yield return null;
        }
        if(LevelHandler.lh.validateAnswer()){
          tape.correct.Play();
          while(tape.correct.isPlaying){
            yield return null;
          }
          while(LevelHandler.lh.loadNextTape()){
            //execute();
          }
          Debug.Log("Level complete!");
        }
        else{
          tape.incorrect.Play();
          LevelHandler.lh.loadTapeN(0);
        }
      }
    }

    public void interrupt(){
      tape.interrupt();
    }

    public void reset(){
      StartCoroutine(resetEnum());
    }

    IEnumerator resetEnum(){
      tape.interrupt();
      while(tape.executing){
        Debug.Log("waiting");
        yield return null;
      }
      tape.reinitialize();
    }

    void updatePositions(int startIndex){
      for(int j = startIndex; j < instructions.Count; j++){
        Instruction i = instructions[j];

        i.index = j;
        i.text.text = j + ") " + i.instructionType;

        RectTransform rt = i.gameObject.GetComponent<RectTransform>();

        Vector3 pos = rt.anchoredPosition;
        pos = new Vector3(pos.x, pos.y + 65f, pos.z);

        rt.anchoredPosition = pos;
      }
    }

    public void go_to(){
      addInstructionToList("Goto");
    }

    public void go_to_If(){
      addInstructionToList("GotoIf");
    }

    public void br(){
      addInstructionToList("Break");
    }

    public void write(){
      addInstructionToList("Write");
    }

    public void increment(){
      addInstructionToList("Increment");
    }

    public void decrement(){
      addInstructionToList("Decrement");
    }

    public void left(){
      addInstructionToList("Left");
    }

    public void right(){
      addInstructionToList("Right");
    }

    public void comment(){
      addInstructionToList("Comment");
    }

    public void selectInstruction(Instruction inst){
      selectedInstruction = inst;
    }

    public Instruction addInstructionToList(string instruction){
      content.sizeDelta = new Vector2(0, 60 * (instructions.Count + 1));

      float spawnY = 495 - (65 * (instructions.Count + 1));
      float spawnX = 0f;
      Vector3 pos = new Vector3(spawnX, spawnY, instructionListSpawnPoint.position.z);
      GameObject spawnedInstruction = (GameObject)Instantiate(instructionObject, pos, instructionListSpawnPoint.rotation);
      spawnedInstruction.transform.SetParent(instructionListSpawnPoint, false);
      spawnedInstruction.name = instruction + (instructions.Count);

      Instruction instructionComponent = spawnedInstruction.GetComponent<Instruction>();
      instructionComponent.instructionType = instruction;
      instructions.Add(instructionComponent);

      instructionComponent.text.text = (instructions.Count - 1) + ") " + instruction;
      instructionComponent.index = instructions.Count - 1;

      Button instructionButton = spawnedInstruction.GetComponent<Button>();
      instructionButton.onClick.AddListener(delegate {selectInstruction(instructionComponent);});

      InputField inputField = instructionComponent.first;
      InputField second = instructionComponent.second;

      inputField.onValidateInput += delegate (string s, int i, char c) { return char.ToUpper(c); };

      switch(instruction){
        case "Goto":
          instructionComponent.image.color = new Color32(84, 13, 110, 150);
          instructionComponent.sprite.sprite = gotoSprite;

          inputField.placeholder.GetComponent<Text>().text = "Line";
          inputField.contentType = InputField.ContentType.IntegerNumber;

          second.gameObject.SetActive(false);

          break;
        case "Comment":
          instructionComponent.image.color = new Color32(150, 150, 150, 150);
          instructionComponent.sprite.sprite = null;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case "GotoIf":
          instructionComponent.image.color = new Color32(255, 210, 63, 150);
          instructionComponent.sprite.sprite = gotoSprite;

          inputField.placeholder.GetComponent<Text>().text = "Line";
          inputField.contentType = InputField.ContentType.IntegerNumber;

          second.placeholder.GetComponent<Text>().text = "Symbol";

          break;
        case "Break":
          instructionComponent.image.color = new Color32(255, 0, 0, 150);
          instructionComponent.sprite.sprite = breakSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case "Write":
          instructionComponent.image.color = new Color32(59, 206, 172, 150);
          instructionComponent.sprite.sprite = writeSprite;

          inputField.placeholder.GetComponent<Text>().text = "Value";
          inputField.characterLimit = 1;
          second.gameObject.SetActive(false);

          break;
        case "Increment":
          instructionComponent.image.color = new Color32(14, 173, 105, 150);
          instructionComponent.sprite.sprite = incrementSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case "Decrement":
          instructionComponent.image.color = new Color32(14, 173, 105, 150);
          instructionComponent.sprite.sprite = decrementSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case "Left":
          instructionComponent.image.color = new Color32(25, 123, 189, 150);
          instructionComponent.sprite.sprite = leftSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case "Right":
          instructionComponent.image.color = new Color32(25, 123, 189, 150);
          instructionComponent.sprite.sprite = rightSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
      }
      return instructionComponent;
    }
}
