using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootDisplacement : MonoBehaviour
{
    public GameObject root;
    public GameObject target;
    public GameObject foot;
    public float stepDistance = 1f;
    public float footSpacing = 0f;
    public float stepHeight = 1f;
    public float stepSpeed = 1f;

    private float lerp = 0f;
    private Vector3 oldPosition;
    public Vector3 newPosition;

    //Fields for other leg
    private bool isMoving;
    public FootDisplacement oppositeChain;
    

    [SerializeField] private LineRendererController line;
    [SerializeField] private Transform[] LRTransforms;

    RaycastHit hit;
 
    // Start is called before the first frame update
    void Start()
    { 
        line.SetUpLine(LRTransforms);
        oldPosition = transform.position;
        newPosition = transform.position;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = new Ray(root.transform.position + (root.transform.right * footSpacing), Vector3.down);
        
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Terrain")))
            //Debug.Log("Raycast Hit");
        {
            //Debug.Log(Vector3.Distance(newPosition, foot.transform.position));
            if (Vector3.Distance(hit.point, foot.transform.position) > stepDistance)
            {
                
                //foot.transform.position = target.transform.position;
                newPosition = hit.point;
                if (oppositeChain != null)
                {
                    if (!oppositeChain.getIsMoving())
                    {
                        isMoving = true;
                        lerp = 0;
                    }
                }
                else
                {
                    lerp = 0;
                }
                
                //oldPosition = foot.transform.position;
                //Debug.Log("Distance bigger, lerp = " + lerp);
            }
        }
        //if (lerp < 1 && !isLerping) isLerping = true;

        
        if (lerp<1)
        {
            lerp += Time.deltaTime * stepSpeed;
            lerp = Mathf.Clamp(lerp, 0f, 1f);
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);

            footPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            Debug.Log("footPosition: " + footPosition);
            Debug.Log("old" + oldPosition);
            Debug.Log("new" + newPosition);
            Debug.Log("     lerp: " + lerp);
            foot.transform.position = footPosition;
            //oldPosition = footPosition;
            
        }
        else
        {
            isMoving = false;
            oldPosition = newPosition;
        }
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(root.transform.position + (root.transform.right * footSpacing), Vector3.down*100f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(newPosition, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.2f);
    }

    public bool getIsMoving()
    {
        return isMoving;
    }
}
