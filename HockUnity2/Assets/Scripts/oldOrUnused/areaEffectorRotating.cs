using UnityEngine;
using System.Collections;

public class areaEffectorRotating : MonoBehaviour {

    public AreaEffector2D areaEffector;
    public Transform rotationTarget;


	// Use this for initialization
	void Start () {
	   
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        areaEffector.forceAngle = (rotationTarget.rotation.z * 180.0f);

    }
}
