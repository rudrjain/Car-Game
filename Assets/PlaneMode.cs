using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlaneMode : Mode
{
    public GameObject sceneobject;

    private Anchor currentSceneAnchor;
    private Vector3 _previousAnchorPosition;

    [Header("References")]
    public Button acceptButton;

    public bool canRaycast = false;
    private bool _firstTime = true;

    public override void OnModeEnabled()
    {
        base.OnModeEnabled();
        PlaneManager.instance.SetDetectedPlaneVisualizerActive(true);
        canRaycast = true;
    }

    public override void OnModeDisabled()
    {
        base.OnModeDisabled();
        PlaneManager.instance.SetDetectedPlaneVisualizerActive(false);
        canRaycast = false;
    }

    public void Start()
    {
        acceptButton.interactable = false;
        sceneobject.SetActive(false);

    }

    public void PlaneIsSelected()
    {
        acceptButton.interactable = true;
    }

    public void OnAccept()
    {
        canRaycast = false;
        uiManager.instance.SetMode(ModState.EditMode);
    }

    public void UpdatePointOfInterest()
    {
        if (currentSceneAnchor != null)
        {
            if (_previousAnchorPosition != currentSceneAnchor.transform.position)
            {
                _previousAnchorPosition = currentSceneAnchor.transform.position;
                ScaleManager.instance.pointOfInterest = _previousAnchorPosition;
            }
        }
    }

    private void Update()
    {
        UpdatePointOfInterest();

        if (!canRaycast || Input.touchCount <= 0)
            return;
        Touch touch = Input.GetTouch(0);

        TrackableHit hit;

        TrackableHitFlags filter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, filter, out hit) && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            if ((hit.Trackable is DetectedPlane) && Vector3.Dot(Camera.main.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0)
            {
                return;
            }

            if ((hit.Trackable is DetectedPlane) && ((DetectedPlane)hit.Trackable).PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
            {

                Transform root = ScaleManager.instance.rootTransform;

                Vector3 hitPosition = hit.Pose.position;
                hitPosition.Scale(root.transform.localScale);
                root.localPosition = hitPosition * -1;


                sceneobject.transform.position = hit.Pose.position;

                ScaleManager.instance.pointOfInterest = sceneobject.transform.position;
                if (_firstTime)
                {
                    _firstTime = false;
                    ScaleManager.instance.InitializeStartScale();
                }

                ScaleManager.instance.AlignWithPointOfInterest(hit.Pose.position);
                if (sceneobject.activeSelf == false)
                    sceneobject.SetActive(true);

                if (touch.phase == TouchPhase.Ended)
                {
                    PlaneIsSelected();
                    Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    currentSceneAnchor = anchor;
                    sceneobject.transform.SetParent(anchor.transform);
                }
            }
        }
    }
}
