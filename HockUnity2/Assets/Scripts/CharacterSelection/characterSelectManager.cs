using UnityEngine;
using System.Collections;
using System.Collections.Generic; //allows using lists
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Carries information between a competitive menu and competitive levels

public class characterSelectManager : MonoBehaviour {   //Start this on an object in the menu screen

    static characterSelectManager Instance;             //for Singleton

    //PRE AND POST GAME ATTRIBUTES
    public int playersJoined = 0;
    public int playersReady = 0;
    public int minimumPlayersToStart = 2;


    public List<Sprite> characterIcons = new List<Sprite>();
    public List<bool> characterAvailability = new List<bool>();

    public List<playerStatManager> players = new List<playerStatManager>();         //Children to this object
    public List<Transform> playerInstBank = new List<Transform>(new Transform[8]);  //list of prefabs for players to choose from, should match to the charIcons

    competitiveGameManager gameManager;
    
    void Start () {
	    if (Instance != null)   //Singleton Pattern
        {
            GameObject.Destroy(gameObject); //if instance exists, destroy this gameobject
        }
        else
        {
            GameObject.DontDestroyOnLoad(gameObject);
            Instance = this;
        }
	}
	
	void Update () {
	}

    public void LoadScene (string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        StartCoroutine(CheckLevel());
    }

    public IEnumerator CheckLevel()
    {
        yield return new WaitForSeconds(0.25f);
        if (SceneManager.GetActiveScene().name != "menuScene") //if we are not in the menu
        {
            //StartGame();
        }
    }

    public void StartGame()
    {
        //print("gameStarted");
        //GameObject.FindGameObjectWithTag("GameManager");
    }
    
}
