using UnityEngine;
using System.Collections;

public class goalTrigger : MonoBehaviour {

    //Attach this to a goal trigger

    public Transform gameManager;
    public int teamNumber = 0; //0 through 3

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void OnTriggerEnter2D (Collider2D col)
    {
        if(col.tag == "Ball")
        {
            GoalTrigger();
        }
    }

    public void GoalTrigger()
    {
        gameManager.SendMessage("Goal", teamNumber); //SendMessage used in the case of different scripts with a Goal function
    }
}
