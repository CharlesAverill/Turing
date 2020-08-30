using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tape : MonoBehaviour
{

    public GameObject cellPrefab;
    public int index;

    public string[] customTapeValues;

    public AudioSource click;
    public AudioSource pencilScratch;
    public AudioSource zoom;

    private Cell[] tape;

    // Start is called before the first frame update
    void Start()
    {
      index = 0;
      tape = makeTapeFromArray(customTapeValues);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator reinitialize(){
      transform.position = new Vector3(transform.position.x + index, transform.position.y, transform.position.z);

      yield return new WaitForSeconds(.25f);

      foreach(Cell c in tape){
        Destroy(c.gameObject);
      }

      index = 0;
      tape = makeTapeFromArray(customTapeValues);
    }

    public IEnumerator executeInstructions(Instruction[] instructions){
      yield return StartCoroutine(reinitialize());
      for(int j = 0; j < instructions.Length; j++){
        Instruction i = instructions[j];

        bool breakLoop = false;

        switch(i.instructionType){
          case "Goto":
            zoom.Play();
            int newIndex = Int32.Parse(i.userContent);

            if(newIndex >= 0 && newIndex < instructions.Length){
              j = newIndex - 2;
            }
            else{
              breakLoop = true;
            }

            break;
          case "Write":
            pencilScratch.Play();
            write(i.userContent);
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
        yield return new WaitForSeconds(.5f);
        if(breakLoop){
          break;
        }
      }
    }

    public bool write(string newVal){
      if(index >= 0 && index < tape.Length){
        tape[index].setVal(newVal);
        return true;
      }
      else{
        return false;
      }
    }

    public string read(){
      return tape[index].val;
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
        Vector3 pos = new Vector3(i, transform.position.y, transform.position.z);

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
        Vector3 pos = new Vector3(i, transform.position.y, transform.position.z);

        GameObject cellI = (GameObject)Instantiate(cellPrefab, pos, transform.rotation);
        cellI.transform.SetParent(transform, false);
        cellI.name = "Cell " + (i);

        Cell cellComponent = cellI.GetComponent<Cell>();
        cellComponent.setVal(arr[i]);

        output[i] = cellComponent;
      }
      return output;
    }
}
