using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{

    public Text text;

    public Image image;
    public Image sprite;

    public InputField first;
    public InputField second;

    public InstructionType InstructionType { get; set; }

    public string UserContent
    {
      get => userContent;
      set
      {
        userContent = value;
        first.text = value;
      }
    }

    public string ExtraUserContent
    {
      get => extraUserContent;
      set
      {
        extraUserContent = value;
        second.text = value;
      }
    }
    
    private string extraUserContent;
    public int index { get; set; }
    private bool canMove;
    private string userContent;

    // Start is called before the first frame update
    void Start()
    {
      canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
      if(first.isFocused || second.isFocused){
        canMove = false;
        CameraController.cc.canMove = false;
      }
      else if(!canMove){
        canMove = true;
        CameraController.cc.canMove = true;
      }
    }
}
