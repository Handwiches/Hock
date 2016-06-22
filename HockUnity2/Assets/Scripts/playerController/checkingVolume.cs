using UnityEngine;
using System.Collections;

/*
    Should be small enough to handle the functionality and Audio/Visual
    Attached to an object below InGameControl

    TO DO:
    [] Color the checked particle based on the team, from the competitiveGameManager team colors
    [] Hook up audio to this
    [] Make sure the to have a friendly checking toggle under the GameManager
*/

public class checkingVolume : MonoBehaviour {

    public Transform attachedPlayer; //making sure you don't check yourself, or wreck yourself
    public float checkPower = 10.0f;
    public float checkedTime = 5.0f;

    public InGameControl playerControl;

    //Visual
    public Transform checkedParticle;

    //Audio
    [FMODUnity.EventRef]
    public string checkSound = "event:/SFX_Check";
    FMOD.Studio.EventInstance checkManager;                //eventInstance

    // Use this for initialization
    void Start () {
        //Audio
        checkManager = FMODUnity.RuntimeManager.CreateInstance(checkSound);

        if (playerControl == null)
        {
            print("no playerControl on checkingVolume found");
            transform.root.GetComponent<InGameControl>();
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && col.transform != attachedPlayer)
        {
            Check(col);
        }
    }

    public void Check(Collider2D col)
    {
        StartCoroutine(CoCheck(col));

    }

    public IEnumerator CoCheck(Collider2D col)
    {
        playerControl.SendMessage("Check");//STAT
        checkManager.start(); //Plays Audio
        print("CHECKED");
        Transform playerChecked = col.transform;
        Vector2 checkDirection;
        checkDirection = new Vector2((attachedPlayer.position.x - playerChecked.position.x), (attachedPlayer.position.y - playerChecked.position.y));
        playerChecked.parent.GetComponent<InGameControl>().Checked(checkedTime);

        Vector3 checkedPosition = attachedPlayer.position - (attachedPlayer.position - playerChecked.position);
        Transform checkParticle = Instantiate(checkedParticle, checkedPosition, Quaternion.identity) as Transform;
        checkParticle.parent = playerChecked;

        yield return new WaitForSeconds(0.05f);
        col.GetComponent<Rigidbody2D>().AddForce(checkDirection);
    }
}
