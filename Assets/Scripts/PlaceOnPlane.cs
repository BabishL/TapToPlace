using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : PressInputBase
{
   
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
     GameObject placedPrefab;

    [SerializeField]
    private TextMeshProUGUI furnitureNameText;

    GameObject spawnedObject;
    bool isPressed;

    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    protected override void Awake()
    {
        base.Awake();
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Pointer.current == null || isPressed == false ||placedPrefab == null )
            return;

        // Store the current touch position.
        var touchPosition = Pointer.current.position.ReadValue();

        // Check if the raycast hit any trackables.
        if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means the closest.
            var hitPose = hits[0].pose;

            // Check if there is already spawned object. If there is none, instantiated the prefab.
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {

                Destroy(spawnedObject);
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            }

            Vector3 lookPos = Camera.main.transform.position - spawnedObject.transform.position;
            lookPos.y = 0;
            spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
            UpdateFurnitureName();
            
        }
    }
    public void SetPrefab(GameObject newPrefab)
    {
        placedPrefab = newPrefab;
        Debug.Log($"New prefab selected: {newPrefab.name}");
    }

    private void UpdateFurnitureName()
    {
        if (furnitureNameText != null && placedPrefab != null)
        {
            furnitureNameText.text = placedPrefab.name;
        }
    }

    public bool IsButtonPressed()
    {
        if(EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    protected override void OnPress(Vector3 position) => isPressed = true;

    protected override void OnPressCancel() => isPressed = false;
}
