using UnityEngine;
using System.Collections;
using System.Collections.Generic; //allows using lists
using UnityEngine.UI;
using InControl;

//Selecting a level will lead to this 

public class playerTeamSelect : MonoBehaviour {

    public int inputDeviceNumber;
    InputDevice inputDevice;

    public characterSelectManager charSelectManager;
    public int team = -1; //number of team

    bool inputGiven = false;

    // Use this for initialization
    void Start () {
        charSelectManager = GameObject.FindGameObjectWithTag("CharacterManager").GetComponent<characterSelectManager>();
    }
	
	// Update is called once per frame
	void Update () {
        //if true								  this,					if false, this.
        inputDevice = (InputManager.Devices.Count > (inputDeviceNumber)) ? InputManager.Devices[(inputDeviceNumber)] : null;
        //if (InputManager.Devices[(inputDeviceNumber)] != null)
        //print("device: " + inputDevice.Name); //gives info on PS4, Xbox, etc.

        if (inputDevice.LeftStickX.Value > 0.7f || inputDevice.DPadRight.IsPressed == true)     //Switch character on input
            TeamMove(true);
        if (inputDevice.LeftStickX.Value < -0.7f || inputDevice.DPadLeft.IsPressed == true)
            TeamMove(false); //PreviousCharacter();
    }

    void TeamMove(bool right)   //team = 0 is the left team, team = 1 is the right team, and team = -1 is the middle (undecided
    {
        if (inputGiven == false)
        {
            inputGiven = true;
            StartCoroutine(ResetInputGiven());

            //print("teamMove");
            int currentTeam = team;
            if (right && currentTeam == 0)
            {
                team = -1;
            }
            if (right && currentTeam == -1)
            {
                team = 1;
            }


            if (!right && currentTeam == 1)
            {
                team = -1;
            }
            if (!right && currentTeam == -1)
            {
                team = 0;
            }
        }

    }

    public IEnumerator ResetInputGiven()
    {
        yield return new WaitForSeconds(0.15f);
        inputGiven = false;
    }
}
