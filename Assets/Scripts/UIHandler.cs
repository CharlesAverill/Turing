using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{

    public List<string> instructions;
    public GameObject instructionObject;
    public Transform instructionListSpawnPoint;
    public RectTransform content;

    public Sprite readSprite;
    public Sprite writeSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    // Start is called before the first frame update
    void Start()
    {
      instructions = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void read(){
      Debug.Log("read");
      addInstructionToList("Read");
    }

    public void write(){
      Debug.Log("write");
      addInstructionToList("Write");
    }

    public void left(){
      Debug.Log("left");
      addInstructionToList("Left");
    }

    public void right(){
      Debug.Log("right");
      addInstructionToList("Right");
    }

    public void addInstructionToList(string instruction){
      instructions.Add(instruction);

      content.sizeDelta = new Vector2(0, 60 * instructions.Count);

      float spawnY = -1 * (485 - (65 * instructions.Count));
      float spawnX = 0f;
      Vector3 pos = new Vector3(spawnX, -spawnY, instructionListSpawnPoint.position.z);
      GameObject spawnedInstruction = (GameObject)Instantiate(instructionObject, pos, instructionListSpawnPoint.rotation);
      spawnedInstruction.transform.SetParent(instructionListSpawnPoint, false);

      Instruction instructionComponent = spawnedInstruction.GetComponent<Instruction>();
      instructionComponent.text.text = instruction;
      switch(instruction){
        case "Read":
          instructionComponent.image.color = new Color32(250, 140, 22, 150);
          instructionComponent.sprite.sprite = readSprite;
          break;
        case "Write":
          instructionComponent.image.color = new Color32(138, 224, 49, 150);
          instructionComponent.sprite.sprite = writeSprite;
          break;
        case "Left":
          instructionComponent.image.color = new Color32(22, 189, 250, 150);
          instructionComponent.sprite.sprite = leftSprite;
          break;
        case "Right":
          instructionComponent.image.color = new Color32(223, 35, 250, 150);
          instructionComponent.sprite.sprite = rightSprite;
          break;
      }
    }
}
