using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController cc;

    public float moveSpeed = 1f;

    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
      canMove = true;
      if(cc == null){
        cc = this;
      }
    }

    // Update is called once per frame
    void Update()
    {
      if(canMove){
        var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * moveSpeed * Time.deltaTime;
      }
    }
}
