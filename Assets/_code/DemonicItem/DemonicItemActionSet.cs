using System;
using Boo.Lang;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DemonicItemActionSet {
    public enum When {
        Before, After
    }

    public When when = When.After;
    public int whichVisit;
    
    public enum DemonicItemActionType {
        Enable, Disable, Play, LookAtPlayer, SwitchMaterial, RunFunction
    }
    
    
    public DemonicItemActionType type;
    
    [SerializeField, ShowIf("type", DemonicItemActionType.Play )] AudioClip play;
    [SerializeField, ShowIf("type", DemonicItemActionType.Enable )] GameObject enable;
    [SerializeField, ShowIf("type", DemonicItemActionType.Disable )] GameObject disable;
    [SerializeField, ShowIf("type", DemonicItemActionType.SwitchMaterial )] Material material;
    [SerializeField, ShowIf("type", DemonicItemActionType.RunFunction)] UnityEvent function;

    public void Perform(GameObject demonicItem, GameObject player) {
        Debug.Log("Performing action");
        switch (type) {
            case DemonicItemActionType.Enable:
                enable.SetActive(true);
                break;
            case DemonicItemActionType.Disable:
                disable.SetActive(false);
                break;
            case DemonicItemActionType.Play:
                var audioSource = demonicItem.GetComponent<AudioSource>();
                audioSource.PlayOneShot(play);
                break;
            case DemonicItemActionType.LookAtPlayer:
                MakeLookAtPlayer(demonicItem, player);
                break;
            case DemonicItemActionType.SwitchMaterial:
                demonicItem.GetComponent<MeshRenderer>().material = material;
                break;
            case DemonicItemActionType.RunFunction:
                function.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    static void MakeLookAtPlayer(GameObject demonicItem, GameObject player) {
        demonicItem.transform.LookAt(player.transform);
        var rot = demonicItem.transform.rotation.eulerAngles;
        demonicItem.transform.rotation = Quaternion.Euler(0, rot.y, 0);
    }
}