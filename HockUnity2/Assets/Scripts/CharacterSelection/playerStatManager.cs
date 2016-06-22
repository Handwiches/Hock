using UnityEngine;
using System.Collections;

//Holds stats on each player that will be used to inform the competitiveGameManager that is specific to the game

public class playerStatManager : MonoBehaviour {
    
    //Put in first scene that only gets loaded once every time somebody starts the game
    // OR nest this under the characterSelectManager gameObject

    public int player = 0;
    public bool playing = false;
    public int team = -1;
    public int character = -1;      //characterChosen
    public Sprite uiSymbol;         //for this specific player

    public int timesHit = 0;
    public int checks = 0;
    public int timesChecked = 0;
    public int goals = 0;
    public int selfGoals = 0;
    public int assists = 0;
    public int dashes = 0;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
	    

	}
}
