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

    public string instructionType;
    public string userContent;
    public string extraUserContent;

    public int index;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setContent(Text newContent){
      userContent = newContent.text;
    }

    public void setExtraContent(Text newContent){
      extraUserContent = newContent.text;
    }

    public void setContentString(string newString){
      first.text = newString;
      userContent = newString;
    }

    public void setExtraContentString(string newString){
      second.text = newString;
      extraUserContent = newString;
    }
}
