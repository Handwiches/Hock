using UnityEngine;
using System.Collections;
using System.Collections.Generic; //allows using lists

public class competitiveCamera : MonoBehaviour
{
    //Attach to camera


    Vector3 defaultLocation;
    public float minDistanceLimit;                              //how small largestDistance needs to be to have influence on zoom
    public float zoomLimit = -10.0f;                            //how close the camera can get when the largest distance is small
    public float zoomValue;                                     //zDepth value of the camera

    public List<Transform> targets = new List<Transform>();     //puck and players
    public Vector3 averagePosition;                             //average position of all targets
    public float largestDistance;                               //largest distance between any 2 targets

    public float updateSpeed = 0.5f;
    Vector3 currentPosition;
    Vector3 desiredPosition;


    // Use this for initialization
    void Start()
    {
        defaultLocation = transform.position;
        zoomValue = defaultLocation.z;

        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize() //waits for CompetitiveGameManager to setup the game, should get recalled every score
    {
        yield return new WaitForSeconds(0.5f);

        for (int j = 0; j < targets.Count; j++)
        {
            if (targets[j].parent.gameObject.activeSelf == false)
            {
                targets.RemoveAt(j);
                j = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        averagePosition = SumVector3List();
        currentPosition = transform.position;
        desiredPosition = new Vector3(averagePosition.x, averagePosition.y, zoomValue);
        transform.position = Vector3.Lerp(currentPosition, desiredPosition, Time.deltaTime * updateSpeed);
        largestDistance = GreatestDistance();

        if (largestDistance < minDistanceLimit)
        {
            zoomValue = Mathf.Lerp(zoomLimit, defaultLocation.z, largestDistance/minDistanceLimit);
        }
        else
        {
            zoomValue = defaultLocation.z;
        }
    }

    public Vector3 SumVector3List()
    {
        Vector3 sum = new Vector3(0, 0, 0);
        int targetCount = targets.Count;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].parent.gameObject.activeSelf == true) //if this object is active
            {
                sum += targets[i].position;
            }
            else
            {
                targetCount -= 1;
            }
        }
        sum = (sum / targetCount);
        sum = new Vector3((sum.x + defaultLocation.x) / 2, (sum.y + defaultLocation.y) / 2, sum.z);
        return sum;
    }

    public float GreatestDistance()
    {
        float maxDistance;
        List<float> distances = new List<float>();

        for (int i = 0; i < targets.Count; i++)
        {

            for (int j = 0; j < targets.Count; j++)
            {
                if (targets[j].parent.gameObject.activeSelf == true)
                {
                    float dist = Vector3.Distance(targets[i].position, targets[j].position);
                    distances.Add(dist);
                }
                else
                {
                   // print("inactive parent");
                }
            }
            
        }

        distances.Sort();
        maxDistance = distances[distances.Count - 1];
        return maxDistance;
    }
}
