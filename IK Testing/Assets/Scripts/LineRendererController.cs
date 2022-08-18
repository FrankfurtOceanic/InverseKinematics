using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points;
    // Start is called before the first frame update
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
 
    public void SetUpLine(Transform[] positions)
    {
        lr.positionCount = positions.Length;
        this.points = positions;
    }

    private void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }
}
