using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FurniturePlacementManager : MonoBehaviour
{
    [Header("AR Components")]
    public ARSessionOrigin sessionOrigin;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    [Header("Furniture")]
    public GameObject SpawnableFurniture;

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private GameObject placedObject;

    private bool isPlacingEnabled = true;

    void Update()
    {
        if (Input.touchCount == 0 || !isPlacingEnabled || SpawnableFurniture == null)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        // Block placement if touching UI
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (raycastManager.Raycast(touch.position, raycastHits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = raycastHits[0].pose;

            // Place only if not already placed
            if (placedObject == null)
            {
                placedObject = Instantiate(SpawnableFurniture, hitPose.position, hitPose.rotation);
                isPlacingEnabled = false;

                TogglePlanes(false); // optional: hide planes after placing
            }
        }
    }

    public void SwitchFurniture(GameObject newFurniture)
    {
        // Remove existing object if any
        if (placedObject != null)
        {
            Destroy(placedObject);
            placedObject = null;
        }

        SpawnableFurniture = newFurniture;
        isPlacingEnabled = true;
        TogglePlanes(true);
    }

    public void ResetPlacement()
    {
        if (placedObject != null)
        {
            Destroy(placedObject);
            placedObject = null;
        }

        isPlacingEnabled = true;
        TogglePlanes(true);
    }

    void TogglePlanes(bool active)
    {
        planeManager.enabled = active;

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(active);
        }
    }
}
