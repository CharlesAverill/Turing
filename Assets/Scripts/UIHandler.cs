using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    public Tape tape;

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

    public Instruction selectedInstruction;

    // Start is called before the first frame update
    void Start()
    {
      instructions = new List<Instruction>();
      selectedInstruction = null;
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

    public void execute(){
      if(tape.executing){
        tape.interrupt();
      }
      StartCoroutine(tape.executeInstructions(instructions.ToArray()));
    }

    public void interrupt(){
      tape.interrupt();
    }

    public void reset(){
      interrupt();
      StartCoroutine(tape.reinitialize());
    }

    void updatePositions(int startIndex){
      for(int j = startIndex; j < instructions.Count; j++){
        Instruction i = instructions[j];

        i.index = j;
        i.text.text = (j + 1) + ") " + i.instructionType;

        RectTransform rt = i.gameObject.GetComponent<RectTransform>();

        Vector3 pos = rt.anchoredPosition;
        pos = new Vector3(pos.x, pos.y + 65f, pos.z);

        rt.anchoredPosition = pos;
      }
    }

    public void read(){
      addInstructionToList("Read");
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

    public void left(){
      addInstructionToList("Left");
    }

    public void right(){
      addInstructionToList("Right");
    }

    public void selectInstruction(Instruction inst){
      selectedInstruction = inst;
    }

    public void addInstructionToList(string instruction){
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
        case "Left":
          instructionComponent.image.color = new Color32(25, 123, 189, 150);
          instructionComponent.sprite.sprite = leftSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
        case "Right":
          instructionComponent.image.color = new Color32(225, 132, 170, 200);
          instructionComponent.sprite.sprite = rightSprite;

          inputField.gameObject.SetActive(false);
          second.gameObject.SetActive(false);

          break;
      }
    }
}
