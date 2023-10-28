using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class Darkener : MonoBehaviour {
    [SerializeField] Volume volume;
    //ColorGrading colorGrading;
    //Vignette vignette;
    public static Darkener instance;
    
    void Awake() {
        instance = this;
    }
    
    [Button]
    public void ShockThePlayer() {
        StartCoroutine(ShockThePlayerCoroutine());
    }

    IEnumerator ShockThePlayerCoroutine() {
        SetVolumeEnabledAndInvisible();
        yield return TweenWeightToOne(0.3f);
        yield return TweenWeightToZero(0.3f);
        SetVolumeDisabled();
    }

    void SetVolumeEnabledAndInvisible() {
        volume.enabled = true;
        volume.weight = 0;
    }
    
    IEnumerator TweenWeightToOne(float duration) {
        for (float time = 0; time < duration; time += Time.deltaTime) {
            volume.weight += Time.deltaTime/duration;
            yield return null;
        }
    }
    
    IEnumerator TweenWeightToZero(float duration) {
        for (float time = 0; time < duration; time += Time.deltaTime) {
            volume.weight -= Time.deltaTime/duration;
            yield return null;
        }
    }
    
    void SetVolumeDisabled() {
        volume.weight = 0;
    }
}
