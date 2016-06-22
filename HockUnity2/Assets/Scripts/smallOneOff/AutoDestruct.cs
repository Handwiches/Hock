using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {

    public float timeUntilDestruction = 1.0f;

    // Use this for initialization
    void Start () {
        StartCoroutine(Destruct());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator Destruct()
    {
        yield return new WaitForSeconds(timeUntilDestruction);
        Destroy(gameObject);
    }
}
