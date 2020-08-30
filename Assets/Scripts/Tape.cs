using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tape : MonoBehaviour
{

    public GameObject cellPrefab;
    public int index;

    public Cell[] tape;

    // Start is called before the first frame update
    void Start()
    {
      index = 0;
      tape = makeTapeFromArray(new string[]{"0", "1", "A", "B", "9"});
    }

    // Update is called once per frame
    void Update()
    {

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
