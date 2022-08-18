using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
    public float amplitude =1f;
    public float speed =1f;
    private float baseHeight;
    // Start is called before the first frame update
    void Start()
    {
        baseHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 position = new Vector3(transform.position.x, baseHeight + (amplitude*Mathf.PingPong(Time.time*speed, 1)-amplitude/2), transform.position.z);
        transform.position = position;
    }
}
