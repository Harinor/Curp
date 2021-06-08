/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArManager : MonoBehaviour
{
    [SerializeField] ARSessionOrigin arOrigin;
    [SerializeField] ARRaycastManager arRaycastManager;
    [SerializeField] GameObject placementIndicator;
    [SerializeField] GameObject objectToPlace;
    [SerializeField] Camera arCamera;
    Pose placementPose;
    bool validPose;

    private void Awake()
    {
        LoaderUtility.Initialize();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        if (validPose && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0,5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        validPose = hits.Count > 0;
        if (validPose)
        {
            placementPose = hits[0].pose;

            var cameraDir = arCamera.transform.forward;
            var cameraBearing = new Vector3(cameraDir.x, 0, cameraDir.y).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (validPose)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}
*/