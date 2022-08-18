using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IKSolver : MonoBehaviour
{
    public int chainLength = 0;

    public Transform target;
    public Transform hint; //also known as the pole

    public int iterations = 5; //number of iterations

    public float delta = 0.01f; //acceptable delta error


    //Gizmo settings
    private float distanceScale = 0.1f;

    //private fields
    private Transform[] bones; //bones go from root (bones[0]) to leaf (bones[chainLength]) 
    private float[] boneLengths;
    private Vector3[] positions;
    private float limbLength; //the length of the entire limb


    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        //initialize the bone arrays
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        boneLengths = new float[chainLength];
        limbLength = 0f;

        //populate bone arrays
        var current = transform;
        for(int i =bones.Length-1; i>=0; i--)
        {
           
            bones[i] = current;
            if (i == bones.Length - 1) //if it's the leaf
            {

            }
            else //calculate boneLength only if its not the leaf
            {
                boneLengths[i] = (bones[i + 1].position - bones[i].position).magnitude;
                limbLength += boneLengths[i]; 
            }
            current = current.parent;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        solveIK();
    }

    private void solveIK()
    {
        if (target == null) return; //handles case where there is no target

        if (boneLengths.Length != chainLength) Setup(); //case where the number of bones is changed


        //get bone positions//
        for(int i = 0; i<bones.Length; i++)
        {
            //Debug.Log(i);
            positions[i] = bones[i].position;
            
        }
        //Debug.Log(positions[0]);
        

        //perform calculations//

        //First. Can the target be reached?
        //Compare the distance from the root to the target with the length of the limb
        if((positions[0]- target.position).sqrMagnitude >= limbLength * limbLength) //use square instead because it is faster than calculating sqrt
        {
            //in this case the limb can't reach the target so it should just get as close as it can by stretching out in a line
            var direction = (target.position - positions[0]).normalized; //get direction from root to target
            for(int i = 1; i<positions.Length; i++)
            {
                positions[i] = positions[i - 1] + direction * boneLengths[i - 1];
            }
        }

        //If the target can be reached let's bend!!!
        else
        {
            for(int iterate =0; iterate < iterations; iterate++)
            {
                //backwards first
                //Set end effector to target
                positions[positions.Length-1] = target.position;
                for(int i = positions.Length -2; i>0; i--) //i>0 because we don't want to move the root
                {
                    positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * boneLengths[i]; //move the joint i  from the next joint on the chain in the direction of joint i with length of the bone
                }

                //then forwards
                //Set root to original position
                for (int i = 1; i <positions.Length; i++)
                {
                    positions[i] = positions[i - 1] + (positions[i] - positions[i-1]).normalized * boneLengths[i-1];
                }

                
                //if the end leaf is close enough (within the delta error), we can break
                if ((positions[positions.Length-1] - target.position).sqrMagnitude < delta * delta)
                {
                    break;
                }
                Debug.Log("Distance from leaf to target: " + (positions[positions.Length - 1] - target.position).magnitude);
            }
        }

        //if we are using a pole, orient each bone in the direction of the pole
        if(hint != null)
        {
            //Debug.Log("pole active");
            for(int i = 1; i < positions.Length-1; i++)
            {
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(hint.position); //position the pole on the plane
                var projectedBone = plane.ClosestPointOnPlane(positions[i]); // project the current bone onto the plane
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }


        //set bone positions//
        for (int i = 0; i < positions.Length; i++)
        {
            bones[i].position = positions[i];
        }
    }

    private void OnDrawGizmos()
    {
        var current = this.transform;
        for(int i = 0; i<chainLength && current != null && current.parent !=null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * distanceScale;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up *0.5f, Vector3.one);
            current = current.parent;
        }
    }

   
}
