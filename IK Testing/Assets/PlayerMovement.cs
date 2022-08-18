using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f; //Controls velocity multiplier

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float xMove = Input.GetAxisRaw("Horizontal"); // d key changes value to 1, a key changes value to -1
        float zMove = Input.GetAxisRaw("Vertical"); // w key changes value to 1, s key changes value to -1
        transform.Translate( new Vector3(xMove, 0, zMove) * speed * Time.deltaTime); // Creates velocity in direction of value equal to keypress (WASD). rb.velocity.y deals with falling + jumping by setting velocity to y. 

    }
}
