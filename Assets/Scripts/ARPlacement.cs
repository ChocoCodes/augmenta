using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacement : MonoBehaviour
{
    public GameObject arenaPrefab;
    public GameObject indicatorPrefab;

    private GameObject spawnedArena;
    private GameObject indicator;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool isPlacementValid = false;
    private Pose placementPose;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        if (indicatorPrefab != null)
        {
            indicator = Instantiate(indicatorPrefab);
            indicator.SetActive(false);
        }
    }

    void Update()
    {
        UpdatePlacementIndicator();

        bool isInputDetected = false;
        if (Input.GetMouseButtonDown(0)) isInputDetected = true;
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) isInputDetected = true;

        if (isInputDetected && isPlacementValid)
        {
            if (spawnedArena == null)
            {
                spawnedArena = Instantiate(arenaPrefab, placementPose.position, placementPose.rotation);
                RobotFighter newRobot = spawnedArena.GetComponentInChildren<RobotFighter>();
                FindFirstObjectByType<VoiceCommander>().robotFighter = newRobot;
            }
            else
            {
                spawnedArena.transform.position = placementPose.position;
                spawnedArena.transform.rotation = placementPose.rotation;
            }
        }
    }

    void UpdatePlacementIndicator()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            isPlacementValid = true;
            placementPose = hits[0].pose;

            if (indicator != null)
            {
                indicator.SetActive(true);
                indicator.transform.position = placementPose.position;
                indicator.transform.rotation = placementPose.rotation;
            }
        }
        else
        {
            isPlacementValid = false;
            if (indicator != null) indicator.SetActive(false);
        }
    }
}