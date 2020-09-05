using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tape : MonoBehaviour
{

    public GameObject cellPrefab;
    public int index;

    public bool fillWithRandoms = false;
    public string[] customTapeValues;

    public AudioSource click;
    public AudioSource pencilScratch;
    public AudioSource zoom;
    public AudioSource correct;
    public AudioSource incorrect;

    private Cell[] tape;
    public bool interruptFlag;

    public bool executing;

    // Start is called before the first frame update
    void OnEnable()
    {
      index = 0;
      executing = false;
      if(customTapeValues.Length > 0){
        tape = makeTapeFromArray(customTapeValues);
        if(fillWithRandoms){
          randomizeCellArray();
        }
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void interrupt(){
      interruptFlag = true;
    }

    public void generateTape(string[] arr){
      tape = makeTapeFromArray(arr);
      destroy();
    }

    public void destroy(){
      transform.position = new Vector3(transform.position.x + index, transform.position.y, transform.position.z);

      if(tape != null){
        foreach(Cell c in tape){
          Destroy(c.gameObject);
        }
      }

      index = 0;
    }

    public void reinitialize(){
      destroy();
      tape = makeTapeFromArray(customTapeValues);
      interruptFlag = false;
    }

    public IEnumerator executeInstructions(Instruction[] instructions){
      reinitialize();
      executing = true;

      bool breakLoop = false;

      if(!interruptFlag){
        for(int j = 0; j < instructions.Length; j++){

          if(interruptFlag){
            break;
          }

          Instruction i = instructions[j];
          Button b = i.GetComponent<Button>();

          ColorBlock cb = b.colors;
          cb.normalColor = new Color32(128, 230, 255, 150);
          b.colors = cb;

          switch(i.instructionType){
            case "Goto":
              zoom.Play();

              if(i.userContent.Length > 0){
                int newIndex = Int32.Parse(i.userContent);

                if(newIndex >= 0 && newIndex < instructions.Length){
                  j = newIndex - 1;
                }
                else{
                  breakLoop = true;
                }
              }
              else{
                breakLoop = true;
              }

              break;
            case "Comment":
              zoom.Play();
              break;
            case "GotoIf":
              zoom.Play();

              if(read() == i.extraUserContent){
                if(i.userContent.Length > 0){
                  int newIndex = Int32.Parse(i.userContent);

                  if(newIndex >= 0 && newIndex < instructions.Length){
                    j = newIndex - 1;
                  }
                  else{
                    breakLoop = true;
                  }
                }
                else{
                  breakLoop = true;
                }
              }
              break;
            case "Break":
              breakLoop = true;
              break;
            case "Write":
              pencilScratch.Play();

              write(i.userContent);

              break;
            case "Increment":
              pencilScratch.Play();
              increment();
              break;
            case "Decrement":
              pencilScratch.Play();
              decrement();
              break;
            case "Left":
              click.Play();

              shiftLeft();

              break;
            case "Right":
              shiftRight();

              click.Play();

              break;
          }
          yield return new WaitForSeconds(.1f);

          cb.normalColor = Color.white;
          b.colors = cb;

          if(breakLoop){
            break;
          }
        }
      }
      while(click.isPlaying || pencilScratch.isPlaying || zoom.isPlaying){
        yield return null;
      }
      if(!LevelHandler.lh.validateAnswer()){
        incorrect.Play();
        while(incorrect.isPlaying){
          yield return null;
        }
      }
      else{
        correct.Play();
        while(correct.isPlaying){
          yield return null;
        }
      }
      executing = false;
    }

    public string[] getValues(){
      string[] output = new string[tape.Length];
      for(int i = 0; i < output.Length; i++){
        output[i] = tape[i].val;
      }
      return output;
    }

    public void increment(){
      string val = read().ToUpper();

      if(val.Length < 1){
        return;
      }

      string nums = "0123456789";
      string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

      int nIndex = nums.IndexOf(val);
      int cIndex = chars.IndexOf(val);

      if(nIndex != -1){
        int newVal = (Int32.Parse(val) + 1);
        if(newVal > 9){
          newVal = 0;
        }
        write("" + newVal);
      }
      else if(cIndex != 1){
        if(cIndex + 1 >= chars.Length){
          write("" + chars[0]);
        }
        else{
          write("" + chars[cIndex + 1]);
        }
      }
    }

    public void decrement(){
      string val = read().ToUpper();

      if(val.Length < 1){
        return;
      }

      string nums = "0123456789";
      string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

      int nIndex = nums.IndexOf(val);
      int cIndex = chars.IndexOf(val);

      if(nIndex != -1){
        int newVal = (Int32.Parse(val) - 1);
        if(newVal < 0){
          newVal = 9;
        }
        write("" + newVal);
      }
      else if(cIndex != -1){
        if(cIndex - 1 < 0){
          write("" + chars[chars.Length - 1]);
        }
        else{
          write("" + chars[cIndex - 1]);
        }
      }
    }

    public bool write(string newVal){
      if(index >= 0 && index < tape.Length){
        tape[index].setVal(("" + newVal).ToUpper());
        return true;
      }
      else{
        return false;
      }
    }

    public string read(){
      try{
        return tape[index].val;
      }
      catch{
        return "%";
      }
    }

    public void shiftLeft(){
      index -= 1;
      Vector3 newPos = transform.position;
      newPos.x += 1;
      transform.position = newPos;
    }

    public void shiftRight(){
      index += 1;
      Vector3 newPos = transform.position;
      newPos.x -= 1;
      transform.position = newPos;
    }

    public Cell[] makeTapeOfLengthN(int n){
      Cell[] output = new Cell[n];
      for(int i = 0; i < n; i++){
        Vector3 pos = new Vector3(i, 0, transform.position.z);

        GameObject cellI = (GameObject)Instantiate(cellPrefab, pos, transform.rotation);
        cellI.transform.SetParent(transform, false);
        cellI.name = "Cell " + (i);

        Cell cellComponent = cellI.GetComponent<Cell>();
        cellComponent.setVal("0");

        output[i] = cellComponent;
      }
      return output;
    }

    public Cell[] makeTapeFromArray(string[] arr){
      Cell[] output = new Cell[arr.Length];
      for(int i = 0; i < arr.Length; i++){
        Vector3 pos = new Vector3(i, 0, transform.position.z);

        GameObject cellI = (GameObject)Instantiate(cellPrefab, pos, transform.rotation);
        cellI.transform.SetParent(transform, false);
        cellI.name = "Cell " + (i);

        Cell cellComponent = cellI.GetComponent<Cell>();
        cellComponent.setVal(arr[i]);

        output[i] = cellComponent;
      }
      return output;
    }

    public void randomizeCellArray(){
      string chars = "                  0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

      for(int i = 0; i < tape.Length; i++){
        tape[i].setVal("" + chars[UnityEngine.Random.Range(0, chars.Length - 1)]);
        Debug.Log(tape[i].val);
      }
    }
}
