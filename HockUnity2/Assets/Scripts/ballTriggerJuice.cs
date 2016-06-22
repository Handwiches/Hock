using UnityEngine;
using System.Collections;
using FMOD.Studio;

public class ballTriggerJuice : MonoBehaviour {

    public float scaleSpeed = 0.6f;
    public float initialScale = 0.7f;
    float currentScale = 1.0f;
    public float scaleUpHits = 1.5f;

    public Transform ball;
    bool hitting = false; //stops unecessary functions from running


    public float hitCounter = 0.0f;
    float timeBetweenHits = 1.2f;   //if the ball is dead for this much time, the hit counter is reset
    float hitTimer = 0.0f;           //timer

    [FMODUnity.EventRef]
    public string inputSound = "event:/SFX_Hit";
    EventInstance sfxHit; 
    ParameterInstance velHit; //eventInstance
    ParameterInstance timesHit2;

    // Use this for initialization
    void Start () {
        initialScale = ball.localScale.z;

        //rollingEv = FMODUnity.RuntimeManage.CreateInstance(rolling);
        //rollingEv.getParameter ("speed", out rollingParam);
        //rollingEv.start();
        
        sfxHit = FMODUnity.RuntimeManager.CreateInstance(inputSound);
        sfxHit.getParameter("velocityHit", out velHit);
        sfxHit.getParameter("timesHit ", out timesHit2);
        //sfxHit.start();
        

    }
	
	// Update is called once per frame
	void Update () {
        ball.localScale = new Vector3(Mathf.Lerp(currentScale, initialScale, Time.deltaTime * scaleSpeed), Mathf.Lerp(currentScale, initialScale, Time.deltaTime * scaleSpeed), 0.6f);
        currentScale = Mathf.Lerp(currentScale, initialScale, Time.deltaTime * scaleSpeed);

        if (hitting == false)
        {
            hitTimer += Time.deltaTime;
        }
        if (timeBetweenHits < hitTimer) //if time is up, hitCounter resets
        {
            hitCounter = 0;
        }

        //timesHit2.setValue(hitCounter); //0++, 1++, etc
    }

    public void ScaleUp()
    {
        currentScale = scaleUpHits;
    }

    public IEnumerator timeSlow()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1.0f;
    }


    public void OnTriggerEnter2D(Collider2D col)
    {
        
        //print("wall");
        if (col.tag == "Paddle" && hitting == false)
        {
            col.transform.SendMessage("HitBall"); //sends message to update stat
            ScaleUp();
            StartCoroutine(timeSlow());

            hitting = true;
            hitTimer = 0.0f;
            hitCounter++;

            StartCoroutine(clearHitting());
            float hitStrength1 = col.gameObject.GetComponent<bumper>().gameController.rStickPower; //0 to 1
            float hitCounter1 = hitCounter;
            
            velHit.setValue(hitStrength1); //0 to 1
            timesHit2.setValue(hitCounter); //0++, 1++, etc
            
            sfxHit.start();
            
        }
    }

    public IEnumerator clearHitting()
    {
        yield return new WaitForSeconds(0.1f);
        hitting = false;
    }
}



/*

    #pragma strict
//Audio
var wallSound 				: AudioClip;
var paddleSound 			: AudioClip;
var hitCount				: int = 0;
var timeToResetHitCount 	: float = 1.5;
var pitchStartLevel			: float = 0.8;
var pitchLevel				: float = 0.8;
var pitchIncrements			: float = 0.1; //will add to pitch level
var audio1					: AudioSource;
var audio2					: AudioSource;

var paddleParticle : Transform;
var wallParticle : Transform;

var parent : Transform;
var anim : Animator;
var initialScale : float = 0.7;
var scaleUpHits : float = 1.2;
var scaleSpeed = 0.6;

var currentScale : float = 0.7;

var recentlyHit : boolean = false;	// this will turn on/off the gravity pull  weight
var recentHitResetTime : float = 0.25;
var gravitySpeed : float = 1.0;

//GameSubManager
var playerLastWithPuck : int = 0;
var teamLastWithPuck : int = 0; //Blue is 1, Red is 2

private var initialPos : Vector3;
var hitTimer : float = 0.0;

function Awake () {
	initialPos = transform.localPosition;
	parent = transform.parent;
	audio1.pitch = pitchStartLevel;
	audio2.pitch = pitchStartLevel;
	audio1.clip = wallSound;
	audio2.clip = paddleSound;
}

function Update () {
	//transform.localPosition = initialPos;
	if (hitTimer > 0.0){
		pitchLevel = (pitchStartLevel + (hitCount * pitchIncrements));
		hitTimer -= 1 * Time.deltaTime;
	}
	else{
		pitchLevel = pitchStartLevel;
		hitCount = 0;
	}
	audio1.pitch = pitchLevel;
	audio2.pitch = pitchLevel;
	
	transform.localScale = Vector3(Mathf.Lerp(currentScale, initialScale, Time.deltaTime * scaleSpeed), Mathf.Lerp(currentScale, initialScale, Time.deltaTime * scaleSpeed), 0.6);
	currentScale = Mathf.Lerp(currentScale, initialScale, Time.deltaTime * scaleSpeed);
}

function OnTriggerEnter2D (other : Collider2D){
	if (other.tag == "Wall"){
		Instantiate (wallParticle, transform.position, Quaternion.identity);
		ScaleUp ();
		print ("wall");
	}
	if (other.tag == "Paddle"){
		ScaleUp ();
		print ("Paddle");
		RecentHit("Paddle", other.gameObject);
	}
}

function ScaleUp (){
	currentScale = scaleUpHits;
}

function RecentHit(hitWith : String, object : GameObject){
	if (recentlyHit == false){
		anim.SetTrigger("hit");
		recentlyHit = true;
		hitTimer = timeToResetHitCount;
		hitCount ++;
		if (hitWith == "Wall"){
			audio1.Play();
		}
		if (hitWith == "Paddle"){
			audio2.Play();
			playerLastWithPuck = object.GetComponent(BumperScript).playerNumber;
			teamLastWithPuck = object.GetComponent(BumperScript).teamNumber;
		}
	}
	
	
	yield WaitForSeconds(recentHitResetTime);
	recentlyHit = false;
}

    */
