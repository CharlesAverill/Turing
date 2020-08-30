using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarLimiter : MonoBehaviour
{

    Scrollbar bar;

    // Start is called before the first frame update
    void Start()
    {
      bar = GetComponent<Scrollbar>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void validateValue(){
        if(bar.value > 1){
          bar.value = 1;
        }
    }
}
