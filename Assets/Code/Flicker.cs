using System.Collections;
using UnityEngine;

public class Flicker : MonoBehaviour {
    Light lightSource;
    float baseIntensity;

    IEnumerator Start() {
        lightSource = GetComponent<Light>();
        baseIntensity = lightSource.intensity;
        
        while (true) {
            yield return WaitRandomTime();
            yield return Blink();
        }
    }

    IEnumerator Blink() {
        float newIntensity = Random.Range(0, baseIntensity);
        lightSource.intensity = newIntensity;
        yield return new WaitForSeconds(Random.Range(0f, 0.3f));
        lightSource.intensity = baseIntensity;
    }

    IEnumerator WaitRandomTime() {
        var timeToWait = Random.Range(0f, 1f);
        yield return new WaitForSeconds(timeToWait);
    }
}