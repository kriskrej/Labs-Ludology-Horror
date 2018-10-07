using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Darkener : MonoBehaviour {
    PostProcessVolume volume;
    ColorGrading colorGrading;
    Vignette vignette;
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
        yield return TransitVignetIntensityToOne(0.3f);
        yield return ContrastToLowest(0.1f);
        yield return BackToNeutral(0.3f);
        SetVolumeDisabled();

    }

    void SetVolumeEnabledAndInvisible() {
        if (volume == null) volume = GetComponent<PostProcessVolume>();
        volume.enabled = true;
        
        volume.profile.TryGetSettings(out colorGrading);
        volume.profile.TryGetSettings(out vignette);
        colorGrading.enabled.value = true;
        vignette.enabled.value = true;
        colorGrading.contrast.value = 0;
        vignette.intensity.value = 0;
        vignette.smoothness.value = 0.2f;
    }

    IEnumerator TransitVignetIntensityToOne(float duration) {
        while (vignette.intensity < 1) {
            vignette.intensity.value += Time.deltaTime/duration;
            vignette.smoothness.value += Time.deltaTime/duration;
            colorGrading.contrast.value += Time.deltaTime * 100f/duration;
            yield return null;
        }
    }
    
    IEnumerator ContrastToLowest(float duration) {
        while (colorGrading.contrast > -100) {
            colorGrading.contrast.value -= Time.deltaTime * 100f / duration;
            yield return null;
        }
    }
    
    IEnumerator BackToNeutral(float duration) {
        vignette.intensity.value = 0;
        vignette.smoothness.value = 0.2f;
        while (colorGrading.contrast < -0.1f) {
            colorGrading.contrast.value += 100f*Time.deltaTime/duration; 
            yield return null;
        }
    }
    
    void SetVolumeDisabled() {
        volume.enabled = false;
    }
}
