using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    public List<Instruction> instructions;
    public GameObject instructionObject;
    public Transform instructionListSpawnPoint;
    public RectTransform content;

    public Sprite readSprite;
    public Sprite writeSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

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
      if(selectedInstruction != null && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))){
        instructions.RemoveAt(selectedInstruction.index);
        updatePositions(selectedInstruction.index);
        Destroy(selectedInstruction.gameObject);
      }
    }

    void updatePositions(int startIndex){
      for(int j = startIndex; j < instructions.Count; j++){
        Instruction i = instructions[j];

        i.index = j;

        Transform tr = i.gameObject.transform;
        Vector3 newPos = new Vector3(tr.position.x, tr.position.y + 65, tr.position.z);

        tr.position = newPos;
      }
    }

    public void read(){
      addInstructionToList("Read");
    }

    public void write(){
      addInstructionToList("Write");
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

      float spawnY = 485 - (65 * (instructions.Count + 1));
      float spawnX = 0f;
      Vector3 pos = new Vector3(spawnX, spawnY, instructionListSpawnPoint.position.z);
      GameObject spawnedInstruction = (GameObject)Instantiate(instructionObject, pos, instructionListSpawnPoint.rotation);
      spawnedInstruction.transform.SetParent(instructionListSpawnPoint, false);
      spawnedInstruction.name = instruction + (instructions.Count);

      Instruction instructionComponent = spawnedInstruction.GetComponent<Instruction>();
      instructions.Add(instructionComponent);

      instructionComponent.text.text = instruction;
      instructionComponent.index = instructions.Count - 1;

      Button instructionButton = spawnedInstruction.GetComponent<Button>();
      instructionButton.onClick.AddListener(delegate {selectInstruction(instructionComponent);});

      InputField inputField = instructionComponent.GetComponentInChildren<InputField>();

      inputField.onValidateInput += delegate (string s, int i, char c) { return char.ToUpper(c); };

      switch(instruction){
        case "Read":
          instructionComponent.image.color = new Color32(250, 140, 22, 150);
          instructionComponent.sprite.sprite = readSprite;

          inputField.placeholder.GetComponent<Text>().text = "Name";

          break;
        case "Write":
          instructionComponent.image.color = new Color32(138, 224, 49, 150);
          instructionComponent.sprite.sprite = writeSprite;

          inputField.placeholder.GetComponent<Text>().text = "Value";

          break;
        case "Left":
          instructionComponent.image.color = new Color32(22, 189, 250, 150);
          instructionComponent.sprite.sprite = leftSprite;

          inputField.gameObject.SetActive(false);

          break;
        case "Right":
          instructionComponent.image.color = new Color32(223, 35, 250, 150);
          instructionComponent.sprite.sprite = rightSprite;

          inputField.gameObject.SetActive(false);

          break;
      }
    }
}
