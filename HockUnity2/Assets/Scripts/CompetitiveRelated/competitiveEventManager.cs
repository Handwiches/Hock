using UnityEngine;
using System.Collections;

//Handles Audio/Visual Elements for Goals, comebacks, timers, etc.

[RequireComponent(typeof(competitiveGameManager))]
public class competitiveEventManager : MonoBehaviour {


    public cameraShake camShake;
    public float goalCamShakeAmount = 2.5f;

    public Animator uiAnimation;
    public Transform ballStartParticle;

    public competitiveGameManager gameManager;

    //AUDIO ELEMENTS
    [FMODUnity.EventRef]
    public string musicSound = "event:/MX_Downtempo";
    FMOD.Studio.EventInstance mxManager;                //eventInstance
    FMOD.Studio.ParameterInstance gamePlaying;
    FMOD.Studio.ParameterInstance pointsScored;

    [FMODUnity.EventRef]
    public string matchTimer = "event:/SFX_MatchTimer";
    FMOD.Studio.EventInstance sfxMatchTimer;                //eventInstance

    [FMODUnity.EventRef]
    public string goalScore = "event:/SFX_GoalScore";
    FMOD.Studio.EventInstance sfxGoalScore;                //eventInstance


    void Awake()
    {
        //Audio
        mxManager = FMODUnity.RuntimeManager.CreateInstance(musicSound);
        mxManager.getParameter("gamePlaying", out gamePlaying);
        mxManager.getParameter("pointsScored", out pointsScored);
        //mxManager.start();

        sfxMatchTimer = FMODUnity.RuntimeManager.CreateInstance(matchTimer);
        sfxMatchTimer.start();

        sfxGoalScore = FMODUnity.RuntimeManager.CreateInstance(goalScore);
        //AUDIO END

        gamePlaying.setValue(1.0f);

        //Referencing
        gameManager = gameObject.GetComponent<competitiveGameManager>();
    }
    
    void Start () {
	    
	}
	
	void Update () {
	
	}

    public void Goal(int teamNumber)
    {
        StartCoroutine(Goal1(teamNumber));
    }
    

    public IEnumerator Goal1(int teamNumber) //teamNumber = team that scored
    {
        print("GOal??");
        camShake.Shake(goalCamShakeAmount);
        //adding together team scores to adjust intensity
        int teamPointCount = 0; //for audio
        for (int i = 0; i < gameManager.teamScores.Count; i++)
        {
            teamPointCount += gameManager.teamScores[i];
        }

        sfxGoalScore.start();
        pointsScored.setValue(teamPointCount); //for audio
        print("team point count: " + teamPointCount);

        yield return new WaitForSeconds(0.2f);
    }

    public void CountdownToGo() //321 GO!
    {
        StartCoroutine(CoCountdownToGo());
        
    }

    public IEnumerator CoCountdownToGo()
    {
        gameManager.ball.SetActive(false);
        uiAnimation.SetTrigger("321");
        yield return new WaitForSeconds(1.0f);
        sfxMatchTimer.start();

        yield return new WaitForSeconds(gameManager.playerFreezeTime - 1f);
        Instantiate(ballStartParticle, gameManager.ballStartPosition.position, Quaternion.identity);
        gameManager.ball.SetActive(true);
    }
}
