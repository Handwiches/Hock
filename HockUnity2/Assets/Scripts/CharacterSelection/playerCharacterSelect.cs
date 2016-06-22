using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InControl;

//Informs characterSelectManager.cs of custom player selections and participation

public class playerCharacterSelect : MonoBehaviour {

    public int inputDeviceNumber;
    InputDevice inputDevice;

    public GameObject noControllerObject;
    public GameObject joinedObjectGroup;
    public GameObject yetToJoinObject;
    public GameObject readyObject;
    public Image charIconSlot;
    public characterSelectManager charSelectManager;
    public charSelectUI charSelectMenuManager;
    public int currentSelectedCharacter = 0;
    bool inputGiven = false;

    public enum PlayerState {Unplugged, PluggedIn, Joined, Ready};
    public PlayerState pState; //state this player is currently in

    // Use this for initialization
    void Start () {
       
        readyObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if true								  this,					if false, this.
        inputDevice = (InputManager.Devices.Count > (inputDeviceNumber)) ? InputManager.Devices[(inputDeviceNumber)] : null;
        //if (InputManager.Devices[(inputDeviceNumber)] != null)
        //print("device: " + inputDevice.Name); //gives info on PS4, Xbox, etc.
        
        if (inputDevice == null)
        {
            if (pState == PlayerState.Joined)
                Join(false);
            if (pState == PlayerState.Ready)
            {
                ReadyUp(false);
                Join(false);
            }

            pState = PlayerState.Unplugged;
        }
        if (inputDevice != null && pState == PlayerState.Unplugged)
        {
            pState = PlayerState.PluggedIn;
            //print("yes");
        }

        if (pState == PlayerState.Ready)
            charSelectManager.players[inputDeviceNumber].playing = true; //helps inform teamManager exactly which players are playing
        else
            charSelectManager.players[inputDeviceNumber].playing = false; //helps inform teamManager exactly which players are playing

        switch (pState)
        {
            case PlayerState.Unplugged:
                //print("No Controller Detected");
                noControllerObject.SetActive(true);
                yetToJoinObject.SetActive(false);
                joinedObjectGroup.SetActive(false);
                readyObject.SetActive(false);
                break;
            case PlayerState.PluggedIn: //AKA Unjoined
                                        //print("Controller Detected");
                noControllerObject.SetActive(false);
                yetToJoinObject.SetActive(true);
                joinedObjectGroup.SetActive(false);
                readyObject.SetActive(false);
                if (inputGiven == false) //Player has not joined yet
                {
                    if (inputDevice.Action1.IsPressed == true) //pressed X
                        Join(true);

                    if (inputDevice.Action2.IsPressed == true) //pressed O //BACK OUT to main menu
                        charSelectMenuManager.backOut();
                }
                break;
            case PlayerState.Joined:
                //print("Player Joined");
                noControllerObject.SetActive(false);
                yetToJoinObject.SetActive(false);
                joinedObjectGroup.SetActive(true);
                readyObject.SetActive(false);
                if (inputGiven == false)  //PLAYER HAS JOINED 
                {
                    yetToJoinObject.SetActive(false);
                    //CHARACTER SWITCHING___________________________________________________________
                    if (charSelectManager.characterAvailability[currentSelectedCharacter] == false)         //Auto-switch upon character being unavailable
                        NextCharacter(true);

                    if (inputDevice.LeftStickX.Value > 0.7f || inputDevice.DPadRight.IsPressed == true)     //Switch character on input
                        NextCharacter(true);
                    if (inputDevice.LeftStickX.Value < -0.7f || inputDevice.DPadLeft.IsPressed == true)
                        NextCharacter(false); //PreviousCharacter();

                    if (inputDevice.Action1.IsPressed == true) //pressed X                                  //Ready or Back up
                        ReadyUp(true);  //choose character
                    if (inputDevice.Action2.IsPressed == true) //pressed O
                        Join(false);    //cancel join

                    //Update visual info
                    charIconSlot.sprite = charSelectManager.characterIcons[currentSelectedCharacter];
                    charSelectManager.players[inputDeviceNumber].uiSymbol = charIconSlot.sprite; 
                }
                break;
            case PlayerState.Ready:
                noControllerObject.SetActive(false);
                yetToJoinObject.SetActive(false);
                joinedObjectGroup.SetActive(false);
                readyObject.SetActive(true);
                //print("Player Ready");
                if (inputDevice.Action2.IsPressed == true && inputGiven == false)//pressed O
                    ReadyUp(false);
                break;
        }
    }

    public void Join (bool joinStatus)
    {
        StartCoroutine(inputGivenTimer(0.25f)); //reducing multiple input issues, set longer to avoid readying up right away
        if (joinStatus == false) //leaving
        { 
            pState = PlayerState.PluggedIn; //Unjoined
            charSelectManager.playersJoined--;
        }
        else //joining
        {
            pState = PlayerState.Joined;
            charSelectManager.playersJoined++;
        }
    }

    public void NextCharacter(bool next) 
    {
        
        inputGivenTimer(); //reducing multiple input issues
        int tempSelect = currentSelectedCharacter;
        if (next == true)
            tempSelect++; //next character needed to evaluate
        if (next == false)
            tempSelect--;

        for (int i = 0; i < charSelectManager.characterAvailability.Count; i++)
        {
            if (next == true)
            {
                //if next selection is more than the last one in the list
                if (tempSelect > (charSelectManager.characterAvailability.Count - 1)) //-1 for the fact that .Count is higher than value needed
                {   //reset selection to zero
                    tempSelect = 0;
                }

                if (charSelectManager.characterAvailability[tempSelect] == false && next == true) //if character is unavailable
                {
                    tempSelect++; //check the next one in the list on the next for loop sequence
                }
            }

            if (next == false)
            {
                if (tempSelect < 0)
                {   //reset selection to zero
                    tempSelect = (charSelectManager.characterAvailability.Count - 1);   //-1 for the fact that .Count is higher than value needed
                }

                if (charSelectManager.characterAvailability[tempSelect] == false && next == false) //if character is unavailable
                {
                    tempSelect--; //check the next one in the list on the next for loop sequence
                }
            }
        }
        //print("yo: " + tempSelect);
        currentSelectedCharacter = tempSelect;
    }

    public void inputGivenTimer()
    {
        StartCoroutine(inputGivenTimer(0.15f));
    }

    public IEnumerator inputGivenTimer(float timeToWait)
    {
        inputGiven = true;
        yield return new WaitForSeconds(timeToWait);
        inputGiven = false;
    }

    public void ReadyUp(bool readyStatus)
    {
        int currentChar = currentSelectedCharacter;
        
        //if ready and a DOUBLE CHECK on if the character is available so that selecting the same character is much harder
        if (readyStatus == true && charSelectManager.characterAvailability[currentSelectedCharacter] == true)
        {
            //print("characterSelect");
            inputGivenTimer(); //reducing multiple input issues
            pState = PlayerState.Ready;
            charSelectManager.characterAvailability[currentSelectedCharacter] = false;
            charSelectManager.playersReady++;

            charSelectManager.players[inputDeviceNumber].character = currentSelectedCharacter; //give manager char info
        }
        else //unready
        {
            //print("unready " + currentChar);
            StartCoroutine(inputGivenTimer(0.25f));
            charSelectManager.characterAvailability[(currentChar)] = true; //makes character available again
            pState = PlayerState.Joined;
            charSelectManager.playersReady--;
        }
    }
}
