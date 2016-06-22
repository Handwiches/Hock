using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;  // needs to be imported 

public class charSelectUI : MonoBehaviour {
    public EventSystem eventSystem;
    
    //PRE-GAME ATTRIBUTES
    public characterSelectManager charSelectManager;

    public GameObject menuGroup; //current menu group to deactivate if backing out
    public GameObject backToGroup; //menu group to back up to
    public GameObject moveToGroup; //menu group to move to next when enough players are ready

    public GameObject backToSelected;
    public GameObject moveToSelected;

    public playerCharacterSelect[] playerSelectManagers;
    public bool charactersSelected = false;
    public bool inGame = false; //if the game has started yet

    // Use this for initialization
    void Start () {
        charSelectManager = transform.GetComponent<characterSelectManager>();

    }
	
	// Update is called once per frame
	void Update () {
        if (charSelectManager.playersJoined >= charSelectManager.minimumPlayersToStart && charactersSelected == false) //if enough players have Joined
        {
            if (charSelectManager.playersReady == charSelectManager.playersJoined && charactersSelected == false)  //enough players are ready
            {
                StartCoroutine(CharactersSelected());
            }
        }
    }

    public IEnumerator CharactersSelected()
    {
        charactersSelected = true;
        yield return new WaitForSeconds(0.25f);
        backToGroup.SetActive(false);
        moveToGroup.SetActive(true);
        menuGroup.SetActive(false);
        eventSystem.SetSelectedGameObject(moveToSelected);
    }

    public void backOut() //out of character selection
    {
        backToGroup.SetActive(true);
        menuGroup.SetActive(false);
        eventSystem.SetSelectedGameObject(backToSelected);
    }

    public void backToMenu() //back to character select menu
    {
        backToGroup.SetActive(false);
        moveToGroup.SetActive(false);
        menuGroup.SetActive(true);
        StartCoroutine(backToMenuFcn());
    }

    public IEnumerator backToMenuFcn()
    {
        yield return new WaitForSeconds(0.25f);
        //unready everybody
        for (int i = 0; i < playerSelectManagers.Length; i++)
        {
            if (playerSelectManagers[i].pState == playerCharacterSelect.PlayerState.Ready)
            {
                playerSelectManagers[i].ReadyUp(false);
            }
        }
        yield return new WaitForSeconds(0.25f);
        charactersSelected = false;
    }
}
