using UnityEngine;
using System.Collections;
using InControl;

//Generic Control Script for Characters in-game

[RequireComponent(typeof(InGameControlEvents))]
public class InGameControl : MonoBehaviour {


    public Transform animBase;              //What will be rotated, must contain rigidbody2D
    public Transform playerBody;        //testing Rotation
    float shapeAngle;
    Rigidbody2D mainRigidBody;
    public Transform checkingVolume;        //Sphere that will check other people
    public Transform paddle;                //Paddle that needs to flip
    public GameObject powerPaddle;
    public GameObject hookPaddle;
    float initalPaddleScale = 1.0f;
    
    public float rStickPower = 0.0f;   //0 to 1 of the joystick velocity
    public float addingPowerSpeed = 2.8f;
    public float losingPowerSpeed = 1.25f;

    float angle;
    public float rightStickAngle = 0.0f;
    public bool spinningRight = false;
    float spinCheck = 0.0f;
    public float tempVelocity;

    //INPUT
    public int inputDeviceNumber;
    InputDevice inputDevice;

    //STATS
    Transform gameManager; //will only ever have transform, because gameplay and rules may differ between game manger tags
    characterSelectManager charSelectManager;
    playerStatManager stats;


    //MOVEMENT VARIABLES
    Vector3 moveDirection = Vector3.zero; 	//aka Vector3(0,0,0);
    
    public float turnSpeed = 1.5f;             //the turning speed of the paddle
    public float shapeTurningSpeed = 75.0f; //how fast the main body of the player turns in comparison to the turning speed of the paddle
    public bool disabledMovement = false;
    int speedMod = 1;
    public float speed = 10.0f;
    float initialSpeed = 10.0f;
    public float dashPower = 40.0f;
    public float dashDuration = 0.25f;
    public float dashCoolDown = 2.0f;
    bool dashing = false;
    
    //STICK INPUTS
    [HideInInspector]
    public float xL;
    [HideInInspector]
    public float yL;
    float xR;
    float yR;

    bool stopped = false;
    //float facingRotation = 0.0f;
    //Vector3 velocity = Vector3.zero;
    public bool knockingBack = false;
    bool invertedControls = false;

    public bool controlsInAction = false;
    InGameControlEvents controlEvents;

	// Use this for initialization
	void Start () {
        charSelectManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<characterSelectManager>();    //from main menu
        gameManager = GameObject.FindGameObjectWithTag("GameManager").transform;                                            //within same scene, may have varying scripts
        stats = charSelectManager.players[inputDeviceNumber];

        initalPaddleScale = paddle.localScale.y;
        initialSpeed = speed;
        mainRigidBody = animBase.GetComponent<Rigidbody2D>();

        hookPaddle.SetActive(false);
        controlEvents = gameObject.GetComponent<InGameControlEvents>();
    }
	
	// Update is called once per frame
	void Update () {
        //transform.Translate(0, 0, 0); //collision check every frame, when not moving
                                      //if true								  this,					if false, this.
        inputDevice = (InputManager.Devices.Count > (inputDeviceNumber)) ? InputManager.Devices[(inputDeviceNumber)] : null;
        //This is what assigns the player: InputManager.Devices[playerNum]
        //inputDevice = InputManager.ActiveDevice;
        if (inputDevice == null)
        {
            print("device: " + inputDeviceNumber + " not plugged in");
        }


        if (inputDevice.LeftTrigger.IsPressed && controlsInAction == false || inputDevice.RightTrigger.IsPressed && controlsInAction == false)
        {
            //StartCoroutine(FlipPaddle());
            hookPaddle.SetActive(true);
            powerPaddle.SetActive(false);
        }
        else
        {
            hookPaddle.SetActive(false);
            powerPaddle.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        //transform.Translate(0, 0, 0); //makes sure collision detection is constant

        if (inputDevice != null)
        {
            //MOVEMENT:
            if (knockingBack == false)  //if knocked back, this should keep player input from affecting velocity
            {
                mainRigidBody.velocity = moveDirection;
                
                if (inputDevice.Action1.IsPressed == false)
                {
                    //speed = initialSpeed; DO NOT PUT THIS IN UPDATE();
                }
                
            }
            else {
                
                
            }

            rightStickAngle = inputDevice.RightStick.Angle;

            xL = inputDevice.LeftStickX.Value * (invertedControls ? -1 : 1);
            yL = inputDevice.LeftStickY.Value * (invertedControls ? -1 : 1);
            xR = inputDevice.RightStickX.Value * (invertedControls ? -1 : 1);
            yR = inputDevice.RightStickY.Value * (invertedControls ? -1 : 1);

            Vector3 dir = new Vector3(xL, yL, 0.0f);

            
            moveDirection = new Vector3(xL, yL, 0);                         //allows input from axes
            moveDirection = transform.TransformDirection(moveDirection);    //tells it how to move

            moveDirection.x *= (speed * speedMod);                          //how fast to move
            moveDirection.y *= (speed * speedMod);                          //how fast to move
            speedMod = disabledMovement ? 0 : 1;                            //will stop or continue regular player movement
            

            //VELOCITY AND ACCELERATION 
            //precalculated
            float currentAngle = animBase.eulerAngles.z;
            float desiredAngle = ((Mathf.Atan2(yR, xR) * 180) / Mathf.PI) - 90;
            angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * turnSpeed);
            shapeAngle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * (turnSpeed * shapeTurningSpeed)); //TEST //moves the shape more accordingly to your stick
            tempVelocity = Mathf.Lerp(0.0f, 1.0f, (Mathf.Abs(Mathf.DeltaAngle(desiredAngle, angle))) * (5 * Time.deltaTime));

            

            if (xR != 0 || yR != 0)
            {
                //Right Stick Paddle
                animBase.eulerAngles = new Vector3(0, 0, angle); //actually changes stick angle
                playerBody.eulerAngles = new Vector3(0, 0, shapeAngle); //moves the shape more accordingly to your stick


                //Checking if player is rotating left or right
                spinCheck = (rightStickAngle - (angle - 360.0f));
                if (spinCheck < 1.0f)
                {
                    spinningRight = true;
                }
                else
                {
                    spinningRight = false;
                }

                if (tempVelocity > 0.45f && rStickPower < 1.0f) //0.45 is a dead zone where the velocity power should start turning down
                {
                    rStickPower += ((tempVelocity * addingPowerSpeed) * Time.deltaTime); //adding to the power
                }
                else
                {
                    rStickPower = Mathf.Lerp(rStickPower, 0.0f, Time.deltaTime * losingPowerSpeed); //turn down velocity power
                }
            }
            else
            {
                rStickPower = Mathf.Lerp(rStickPower, 0.0f, Time.deltaTime * losingPowerSpeed);
            }


            if (dir.x > 0.1 || dir.y > 0.1)
            { //moving	//audio.clip = running; //audio.Play();
            }
            else {  //audio.Stop();
                if (stopped == false)
                {
                    //speed = initialSpeed; DO NOT PUT THIS IN UPDATE();
                }
            }

            if (inputDevice.Action1.IsPressed && dashing == false)
            {
                if (knockingBack == false && dashing == false)
                {
                    //Dash(dashPower, moveDirection.x, moveDirection.y);
                    StartCoroutine(Dash());
                }
            }
            if (inputDevice.Action2.IsPressed && dashing == false)
            {
                if (knockingBack == false)
                {
                    //QuickShield();
                }
            }
        }
    }

    public IEnumerator Dash()
    {
        stats.dashes += 1;
        checkingVolume.gameObject.SetActive(true);
        paddle.gameObject.SetActive(false);
        
        
        dashing = true;             //set canBoost to false so that we can't keep boosting while boosting

        
        controlEvents.Dash();       //AUDIO/VISUAL

        // yield return new WaitForSeconds(0.1f); //Warning to other players you're about to dash
        float time = 0.0f;                 //create float to store the time this coroutine is operating


        while (dashDuration > time)     //we call this loop every frame while our custom boostDuration is a higher value than the "time" variable in this coroutine
         {
            //print("dash " + moveDirection);
            //print("dashinggg");
            time += Time.deltaTime;                                         //Increase our "time" variable
             //mainRigidBody.velocity = new Vector2 (dashPower * moveDirection.x, dashPower * moveDirection.y);      //set our rigidbody velocity to a custom velocity every frame
             //mainRigidBody.AddForce(new Vector2(dashPower * moveDirection.x, dashPower * moveDirection.y), ForceMode2D.Impulse);
             speed = (initialSpeed + dashPower);
             yield return 0; //go to next frame
         }



        speed = initialSpeed;
        checkingVolume.gameObject.SetActive(false);

        yield return new WaitForSeconds(dashCoolDown); //Cooldown time for being able to boost again, if you'd like.
        paddle.gameObject.SetActive(true);
        dashing = false; //set back to true so that we can boost again.

    }

    public void HitBall()       //sent message from ballTriggerJuice
    {
        gameManager.SendMessage("Touched", inputDeviceNumber);
        stats.timesHit += 1;
    }

    public void Check()         //sent message from checkingVolume via PlayerPaddle
    {
        stats.checks += 1;
    }

    public void Goal()
    {
        stats.goals += 1;
    }

    public void SelfGoal()
    {
        stats.selfGoals += 1;
    }

    public void Assist()
    {
        stats.assists += 1;
    }

    public IEnumerator FlipPaddle()
    {
            initalPaddleScale *= -1;
            print("flippp");
            controlsInAction = true;
            paddle.localScale = new Vector3(paddle.localScale.x, initalPaddleScale, paddle.localScale.z);
            yield return new WaitForSeconds(0.25f);
            controlsInAction = false;
    }

    public void Checked(float checkedTime){
        stats.timesChecked += 1;
        speed = 0.0f;
        dashing = true;                                 //disables dashing while checked
        StartCoroutine(CoChecked(checkedTime));
    }
    public IEnumerator CoChecked(float checkedTime)
    {
        knockingBack = true;
        yield return new WaitForSeconds(checkedTime);
        knockingBack = false;
        dashing = false;                                //renables dashing after checkedTime runs up
        speed = initialSpeed;
    }
}
