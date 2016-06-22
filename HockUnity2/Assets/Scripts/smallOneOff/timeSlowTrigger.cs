using UnityEngine;
using System.Collections;

public class timeSlowTrigger : MonoBehaviour {


    public string colliderTag = "Ball";
    public float timeCutFraction = 0.5f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == colliderTag)
        {
            Time.timeScale = timeCutFraction;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == colliderTag)
        {
            Time.timeScale = 1.0f;
        }
    }
}
