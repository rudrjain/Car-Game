using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class PlaneManager : MonoBehaviour
{
    public static PlaneManager instance;
    public GameObject detectedPlanePrefab;
    private List<DetectedPlane> _newPlanes = new List<DetectedPlane>();
    private List<DetectedPlaneVisualizer> _allPlanes = new List<DetectedPlaneVisualizer>();

    public bool shouldShowVisualizers = true;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);

        instance = this;
    }

    public void DestroyPlaneRenderer(DetectedPlaneVisualizer visualizer)
    {
        _allPlanes.Remove(visualizer);
        Destroy(visualizer.gameObject);
    }

    public void SetDetectedPlaneVisualizerActive(bool active)
    {
        shouldShowVisualizers = active;

        foreach (DetectedPlaneVisualizer visualizer in _allPlanes)
        {
            visualizer.enabled = active;

        }
    }

    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
            return;

        Session.GetTrackables<DetectedPlane>(_newPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < _newPlanes.Count; i++)
        {
            GameObject planeObject = Instantiate(detectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);

            DetectedPlaneVisualizer visualizer = planeObject.GetComponent<DetectedPlaneVisualizer>();

            visualizer.Initialize(_newPlanes[i]);
            _allPlanes.Add(visualizer);

            visualizer.m_MeshRenderer.enabled = shouldShowVisualizers;
        }
    }
}
