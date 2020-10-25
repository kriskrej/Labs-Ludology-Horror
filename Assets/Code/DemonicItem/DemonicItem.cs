using System.Collections.Generic;
using Code.GameManagers;
using Sirenix.OdinInspector;
using UnityEngine;

public class DemonicItem : MonoBehaviour {
    [ReadOnly, ShowInInspector] int visitCount;
    [ReadOnly, ShowInInspector] VisibilityStatus visibility;

    [ReadOnly, ShowInInspector, HideIf("lastSureStatus", VisibilityStatus.MaybeVisible)]
    VisibilityStatus lastSureStatus = VisibilityStatus.MaybeVisible;

    Camera playerCamera;
    Bounds bounds;
    public List<DemonicItemActionSet> actions = new List<DemonicItemActionSet>();

    enum VisibilityStatus {
        VisibleForSure,
        MaybeVisible,
        HiddenForSure
    }

    bool IsHitByRaycast() {
        return !Physics.Linecast(playerCamera.transform.position, bounds.center, Consts.Layers.wallsFloorsCeilingsMask,
            QueryTriggerInteraction.Ignore);
    }

    bool IsInCameraDirection() {
        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    bool initialised;
    void Update() {
        if (!initialised)
            TryToInitialise();
        else
            UpdateVisiblilityStatus();
    }

    void TryToInitialise() {
        if(!GameManager.Exists()) return;
        visitCount = 0;
        bounds = CalculateBounds();
        playerCamera = GameManager.Instance.player.playerCamera;
        initialised = true;
    }

    Bounds CalculateBounds() {
        Bounds bounds = GetComponentInChildren<MeshRenderer>().bounds;
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>()) {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    void UpdateVisiblilityStatus() {
        visibility = GetVisibilityStatus();
        if (ItemJustBecomeVisible()) 
            PerformVisitStartedActions();
         
        if (ItemJustBecomeInvisible()) 
            PerformVisitCompleteActions();
       
        if (IsSure(visibility)) lastSureStatus = visibility;
    }

    bool ItemJustBecomeVisible() {
        return (lastSureStatus != VisibilityStatus.VisibleForSure && visibility == VisibilityStatus.VisibleForSure);
    }
    
    bool ItemJustBecomeInvisible() {
        return (lastSureStatus == VisibilityStatus.VisibleForSure && visibility == VisibilityStatus.HiddenForSure);
    }

    void PerformVisitStartedActions() {
        Debug.Log($"Player has just seen {gameObject.name}. It's the {visitCount} time!\n", gameObject);
        foreach (var action in actions) {
                    if (action.whichVisit == visitCount+1 && action.when == DemonicItemActionSet.When.Before)
                        action.Perform(gameObject, playerCamera.gameObject);
                }
    }

    void PerformVisitCompleteActions() {
        visitCount++;
        Debug.Log($"Player has just stop seeing {gameObject.name}. It's the {visitCount} time!\n", gameObject);
        foreach (var action in actions) {
            if (action.whichVisit == visitCount && action.when == DemonicItemActionSet.When.After)
                action.Perform(gameObject, playerCamera.gameObject);
        }
    }

    VisibilityStatus GetVisibilityStatus() {
        if (!IsInCameraDirection()) return VisibilityStatus.HiddenForSure;
        if (IsHitByRaycast()) return VisibilityStatus.VisibleForSure;
        return VisibilityStatus.MaybeVisible;
    }

    static bool IsSure(VisibilityStatus status) {
        return status == VisibilityStatus.HiddenForSure || status == VisibilityStatus.VisibleForSure;
    }
}