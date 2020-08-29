using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
      transform.position += move * moveSpeed * Time.deltaTime;
    }
}
