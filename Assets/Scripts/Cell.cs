using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{

    private Text text;
    private string val;

    public string Val
    {
        get => val;
        set
        {
            val = value;
            text.text = val;
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
      text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
