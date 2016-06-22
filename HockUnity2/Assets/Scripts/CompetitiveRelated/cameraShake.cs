using UnityEngine;
using System.Collections;

public class cameraShake : MonoBehaviour
{
    //Attach to Main Camera

    public float magnitude = 0.5f;
    public float duration = 0.25f;
    public Camera mainCamera;
   

    public void Shake()
    {
        Shake(1.0f);
    }

    public void Shake(float shakeMultiplier)
    {
        StartCoroutine(ShakeCamera(shakeMultiplier));
    }

    public IEnumerator ShakeCamera(float shakeMultiplier)
    {
        print("shakee");

        float elapsed = 0.0f;
        float newDuration = (duration * (shakeMultiplier * 1.5f));

        Vector3 originalCamPos = mainCamera.transform.position;

        while (elapsed < duration)
        {
            //print("SHAKE");

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = (Random.value * 2.0f - 1.0f) * shakeMultiplier;
            float y = (Random.value * 2.0f - 1.0f) * shakeMultiplier;
            x *= magnitude * damper;
            y *= magnitude * damper;

            mainCamera.transform.position = new Vector3(x, y, originalCamPos.z);

            yield return null;
        }

        mainCamera.transform.position = originalCamPos;
    }

}
