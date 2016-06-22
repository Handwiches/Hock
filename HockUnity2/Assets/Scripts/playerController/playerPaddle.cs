using UnityEngine;
using System.Collections;

public class playerPaddle : MonoBehaviour {


    public InGameControl playerControl;
	// Use this for initialization
	void Start () {
	    if (playerControl == null)
        {
            print("no playerControl on playerPaddle found");
            transform.root.GetComponent<InGameControl>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HitBall()
    {
        playerControl.SendMessage("HitBall"); //sends message to update stat
    }
}
