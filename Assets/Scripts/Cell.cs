using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{

    public Text text;

    public string val;

    // Start is called before the first frame update
    void OnEnable()
    {
      text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setVal(string newVal){
      val = newVal;
      text.text = val;
    }
}
