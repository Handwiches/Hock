using UnityEngine;
using System.Collections;


//Audio/Visual components tied to the player's control in-game

[RequireComponent(typeof(InGameControl))]
public class InGameControlEvents : MonoBehaviour {


    //Effects lined to the paddle powering up
    public Renderer paddleRenderer;
    public float minEmission = 0.15f;
    public float maxEmission = 2.0f;

    //Effects lined to dashing
    public Transform dashParticle;

    InGameControl control;

	// Use this for initialization
	void Start () {
        control = gameObject.GetComponent<InGameControl>();
	}
	
	// Update is called once per frame
	void Update () {

        paddleRenderer.material.SetColor("_EmissionColor", new Color(0.2f + (control.rStickPower * 2), 0.2f + (control.rStickPower * 2), 0.2f + (control.rStickPower * 2)));
        // DynamicGI.SetEmissive(paddleRenderer, new Color(0 + rStickPower, 0 + rStickPower, 0 + rStickPower)); //Mathf.Lerp(minEmission, maxEmission, rStickPower)
    }

    public void Dash()
    {
        //AUDIO/VISUAL
        Vector3 lookDirection = new Vector3(0, 0, Mathf.Atan2(control.yL, control.xL) * 180 / Mathf.PI); //yL and xL is the left stick Vector2
        Transform dashParticleClone = Instantiate(dashParticle, control.animBase.position, Quaternion.Euler(new Vector3(0, 0, lookDirection.z))) as Transform;
        dashParticleClone.parent = control.animBase; //so that the trail will follow the player
    }
}
