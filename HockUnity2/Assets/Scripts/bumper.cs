using UnityEngine;
using System.Collections;


/* NOTES

    Perhaps use the pong angle-offshoot to give the ball the tiniest bit of influence on the normal from the ContactPoint2D
    If you dash while collisionOffAndOn is running, the paddle collider doesn't come back on

*/

public class bumper : MonoBehaviour
{

    public float force = 5.0f;
    public Vector2 minMaxForce = new Vector2(5000.0f, 8500.0f);
    public float ignoreDampingThreshold = 999999.0f;
    public bool adjustForce = false;                                //adjusts force to player's swing velocity
    public bool flipOnAndOffOnHit = false;
    public float flipOffDelay = 0.2f;
    public float flipOffTime = 0.2f;
    public Transform objectToFlipOff;
    public InGameControl gameController;
    float rotation = 0.0f;
    Quaternion initialRotation;
    Vector3 initialPosition = Vector3.zero;

    public float velocityDamping = 2.8f;
    public float velocityPreThresholdDamping = 2.8f; //Needs damping to avoid Unity's auto-collision making it hit the ball harder than it should

    bool hitting = false;
    bool left = false;

    Collider2D selfCol;

    // Use this for initialization
    void Start()
    {
        initialRotation = transform.localRotation;
        initialPosition = transform.localPosition;
        selfCol = transform.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.localPosition = initialPosition;
        //transform.localRotation = initialRotation;

        if (adjustForce == true)
        {
            force = Mathf.Lerp(minMaxForce.x, minMaxForce.y, gameController.rStickPower);
        }
    }
    

    void OnCollisionEnter2D (Collision2D col)   //used to be the same function, but ontriggerenter
    {
        if (col.collider.tag == "Ball" && hitting == false)
        {
            ContactPoint2D contact;
            contact = col.contacts[0];
           Hit(col.collider, contact);
        }
    }


    void Hit(Collider2D col, ContactPoint2D contact)
    {
        if (hitting == false)
        {
            //col.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //this causes the ball to sort of stick to the paddle when the paddle moves w/ it
            if (force < ignoreDampingThreshold)
                col.GetComponent<Rigidbody2D>().velocity = (col.GetComponent<Rigidbody2D>().velocity / velocityPreThresholdDamping); //velocity damping
            else   //if there's enough charge, ignore how fast puck is going and SLAP SHOT
                col.GetComponent<Rigidbody2D>().velocity = (col.GetComponent<Rigidbody2D>().velocity / velocityDamping); //velocity damping

            //col.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f); //kill ball's velocity        //used to be the line under else
            
            col.GetComponent<Rigidbody2D>().AddForce(-contact.normal * force);  //AddForce towards the normal of the contact point

            
            if (flipOnAndOffOnHit == true)  //if true, the collider will turn on and off based on flipOffTime
                StartCoroutine (collisionOffAndOn(flipOffTime));
            
        }
    }

    public IEnumerator collisionOffAndOn(float waitTime)
    {
        yield return new WaitForSeconds(flipOffDelay);
        hitting = true;
        selfCol.enabled = false;
        //objectToFlipOff.gameObject.SetActive(false);

        yield return new WaitForSeconds(waitTime);
        selfCol.enabled = true;
        //objectToFlipOff.gameObject.SetActive(true);
        hitting = false;
    }

    void WallHit(Collider2D col)    //Function replicating regular PONG-like hitting
    {
        if (hitting == false)
        {
            Vector3 dir = (col.transform.position - transform.position);
            Vector3 dirRound = new Vector3(Mathf.Round(dir.x), Mathf.RoundToInt(dir.y), Mathf.RoundToInt(dir.z));
            print("dirRound " + dirRound);
            //if (left == true) {
            col.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            col.GetComponent<Rigidbody2D>().AddForce(dirRound * force);
        }
    }

}