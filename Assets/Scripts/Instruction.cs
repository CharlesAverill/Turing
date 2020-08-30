using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{

    public Text text;
    public Image image;
    public Image sprite;
    public string instructionType;
    public string userContent;
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
}
