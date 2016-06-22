using UnityEngine;
using System.Collections;
using System.Collections.Generic; //allows using lists
using UnityEngine.UI;


//Should set up player/computer character combinations, assign teams, and handle basic information for competitiveEventManager to translate into Audio/Visual feedback

[RequireComponent(typeof(competitiveEventManager))]
public class competitiveGameManager : MonoBehaviour {
    /*Structure:
        characterSelectManager  :   never destroyed
        playerManager           :   under charSelect, never destroyed

        competitiveGameManager  :   per level
        InGameControl           :   consistent, in every level

        playeCharacterSelect    :   per level/in menu

    */

    //put this script on an object inside a competitive level

    public List<int> teamScores = new List<int>();
    public Color[] teamColors;        //4
    public List<Text> scoreTexts = new List<Text>();
    public Text timerText;
    public float timeLimit = 300.0f;        //seconds
    public bool timerStop = false;          //when true, this will stop the timeLimit from ticking down
    public float playerFreezeTime = 3.0f;   //the amount of time players are frozen at the beginning of a match and after players score
    public Transform[] team0startPoints;    //where to start/reset positions to
    public Transform[] team1startPoints;    //where to start/reset positions to

    public List<Transform> playerInstBank = new List<Transform>(new Transform[8]); //list of prefabs for players to choose from, should match to the charIcons
    public List<Transform> playerBank = new List<Transform>(new Transform[4]); //characters preloaded into game
    List<InGameControl> playerControlBank = new List<InGameControl>();
    public List<InGameControl> controlBank = new List<InGameControl>(new InGameControl[4]); //characters preloaded into game

    public GameObject ball;
    public Transform ballStartPosition;
    
    bool goalRunning = false;

    //STATS
    public int secondLastToTouch; //assist purposes
    public int lastToTouch;

    characterSelectManager charSelectManager;
    competitiveEventManager eventManager;

    void Awake()
    {
        charSelectManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<characterSelectManager>();
        eventManager = gameObject.GetComponent<competitiveEventManager>();
        for (int i = 0; i < playerBank.Count; i++) //setup players
        {
            //playerControlBank.Add(playerBank[i].gameObject.GetComponent<I) 
        }
    }

	// Use this for initialization
	void Start () {


        StartCoroutine(ResetPlayerPosition(playerFreezeTime));

        for (int i = 0; i < playerBank.Count; i++) //setup players
        {
            if (charSelectManager.playersReady >= i) //player is joined and ready
            {
                playerBank[i].gameObject.SetActive(true);
                Transform shapeInstance = Instantiate(playerInstBank[charSelectManager.players[i].character], controlBank[i].playerBody.position, Quaternion.identity) as Transform;
                shapeInstance.parent = controlBank[i].playerBody;
                //set standard shader emissive color to teamColor[x] based on player's standing with team[x]

                for (var j = 0; j < shapeInstance.GetComponent<MeshRenderer>().materials.Length; j++)
                {
                    shapeInstance.GetComponent<MeshRenderer>().materials[j].SetColor("_EmissionColor", teamColors[charSelectManager.players[i].team]);
                }
                //optimally the above would look like:
                // shapeInstance.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", teamColors[charSelectManager.players[i].team]);
                //but, because the mesh has 2 materials on it, this is a fool-proof method for the time being, please fix eventually for optimization
            }
            else  //player is not joined, nor ready
            {
                playerBank[i].gameObject.SetActive(false);
            }
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if (!timerStop)
            timeLimit -= 1 * Time.deltaTime;
        float minutes = Mathf.FloorToInt(timeLimit / 60.0f);
        float seconds = Mathf.FloorToInt(timeLimit - (minutes * 60.0f));
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
    }

    public void Touched(int player)
    {
        //print("touched?");
        if (player != lastToTouch)
        {
            secondLastToTouch = lastToTouch;
        }

        lastToTouch = player;
    }

    public void Goal(int teamNumber)
    {
        if (!goalRunning)
        {
            print("goal");
            StartCoroutine(Goal1(teamNumber));
            eventManager.Goal1(teamNumber);
        }
    }

    public IEnumerator Goal1 (int teamNumber) //team that scored
    {
        StartCoroutine(ResetPlayerPosition(playerFreezeTime));
        print("reset player pos");
        goalRunning = true;
        
        ball.transform.position = ballStartPosition.position;

        //later change this to: if (teams == 2){add point to opposite number of teamNumber}
        //if (teams > 2 && last hit from player not on team teamNumber) {add point to team who last hit ball}, else {subtract point from team teamNumber}
        //because it was hit by someone on that team
        
        teamScores[teamNumber] += 1;
        scoreTexts[teamNumber].text = "" + teamScores[teamNumber]; //update UI text score
            
        if (charSelectManager.players[lastToTouch].team == teamNumber)
             controlBank[lastToTouch].Goal();
        else
             controlBank[lastToTouch].SelfGoal();

        //Check for an assist
        //if last to touch is on the scoring team and the 2nd last to touch is on the same team
        if (charSelectManager.players[lastToTouch].team == teamNumber && charSelectManager.players[lastToTouch].team == charSelectManager.players[secondLastToTouch].team)
        {   //award assist
            controlBank[secondLastToTouch].Assist();
        }
        
        yield return new WaitForSeconds(0.2f);
        goalRunning = false;
    }

    public IEnumerator ResetPlayerPosition(float timeToFreezePlayers)
    {
        int team0SlotsFilled = 0;
        int team1SlotsFilled = 0;
        eventManager.CountdownToGo();
        timerStop = true;

        Time.timeScale = 1.0f;

        for (int i = 0; i < playerBank.Count; i++) //setup players
        {

            if (charSelectManager.players[i].team == 0)
            {
                //playerBank[i].position = team0startPoints[team0SlotsFilled].position;
                controlBank[i].animBase.position = team0startPoints[team0SlotsFilled].position;
                team0SlotsFilled++;
            }

            if (charSelectManager.players[i].team == 1)
            {
                controlBank[i].animBase.position = team1startPoints[team1SlotsFilled].position;
                team1SlotsFilled++;
            }
            controlBank[i].disabledMovement = true;
        }
        

        yield return new WaitForSeconds(timeToFreezePlayers); //time to wait before unlocking player mobility again and get the clock going

        timerStop = false;
        for (int i = 0; i < playerBank.Count; i++) //setup players
        {
            controlBank[i].disabledMovement = false;
        }
   }
}
