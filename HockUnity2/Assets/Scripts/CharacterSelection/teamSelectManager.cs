using UnityEngine;
using System.Collections;
using System.Collections.Generic; //allows using lists
using UnityEngine.UI;
using InControl;

//Selecting a level will lead to this. Closely connected to UI.

/*
to do: get this prototyped out and working 
get it to actually swap out the colors of a mesh in-game

more of a gameModeManager (competitive) thing, but get that to spawn players based on teams
*/

public class teamSelectManager : MonoBehaviour {

    public Color[] teamColors;  //have 3, last one is neutral

    public List<RectTransform> characterIcons = new List<RectTransform>();  //Icons that will move
    public List<RectTransform> iconTeamPosition = new List<RectTransform>(); //what will move the icons over to the left or right of the screen (should be 3 of them)
    List<Image> characterIconsUI = new List<Image>();  //Icons that will move

    public List<playerTeamSelect> playerTeamSelects = new List<playerTeamSelect>();  //Icons that will move

    List<playerStatManager> playerStats = new List<playerStatManager>();  //Icons that will move
    characterSelectManager charSelectManager;

    InputDevice inputDevice;                                              //for a simple initiate game or back out
    public string levelToLoad = "TestRoom";
    public Transform uiGroupToBackUpTo;                                    //If the player hits circle, this will back them up to this UI group
    //this script should be attached to the UIgroup parent
    int playersOnATeam = 0;

    // Use this for initialization
    void Start () {
        charSelectManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<characterSelectManager>();
        playerStats = charSelectManager.players;
        for (int i = 0; i < characterIcons.Count; i++)
        {
            characterIconsUI[i] = characterIcons[i].GetComponent<Image>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        inputDevice = InputManager.ActiveDevice;
        if (inputDevice.Action1 && playersOnATeam >= charSelectManager.playersReady)
                LoadLevel();

        //make it so that in only loads the level if there is one player on each of 2 teams

        if (inputDevice.Action2)
            BackOut();
            

        for (int i = 0; i < playerStats.Count; i++)
        {
            //print("move");
            //characterIconsUI[i].sprite = playerStats[i].uiSymbol; //set the icon to the correct UI element from each player
            playerStats[i].team = playerTeamSelects[i].team;

            if (playerStats[i].playing)
            {                         //if player is playing
                characterIcons[i].gameObject.SetActive(true);   //have team select ui on
                playersOnATeam = playerTeamCheck();
                //adjust team selection UI player indicators to the shape they have chosen
                characterIcons[i].GetComponent<Image>().sprite = playerStats[i].uiSymbol;
            }
            else {
                characterIcons[i].gameObject.SetActive(false);
            }

            if (playerStats[i].team != -1)
            {
                characterIcons[i].anchoredPosition = new Vector2(iconTeamPosition[playerStats[i].team].anchoredPosition.x, characterIcons[i].anchoredPosition.y); //move player icon to team
                characterIcons[i].GetComponent<Image>().color = teamColors[playerStats[i].team];
            }
            else
            {
                characterIcons[i].anchoredPosition = new Vector2(iconTeamPosition[iconTeamPosition.Count - 1].anchoredPosition.x, characterIcons[i].anchoredPosition.y); //if team number is invalid, move to the last team number
                characterIcons[i].GetComponent<Image>().color = teamColors[teamColors.Length - 1];
            }
        }
	}

    int playerTeamCheck()
    {
        int numberCheck = 0;
        for (int i = 0; i < playerStats.Count; i++)
        {
            if (playerStats[i].team != -1)
                numberCheck++;
        }
        return numberCheck;
    }

    public void LoadLevel()
    {
        charSelectManager.LoadScene(levelToLoad);
    }

    public void AssignUIGroupBack(Transform thisOne)    //Assigned from a UI button upon entering this group
    {
        uiGroupToBackUpTo = thisOne;
    }

    public void BackOut()
    {
        if (uiGroupToBackUpTo == null)
            print("No uiGroupToBackUpTo");

        uiGroupToBackUpTo.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
