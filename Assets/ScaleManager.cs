using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ScaleManager : MonoBehaviour
{

    public static ScaleManager instance;
    private Camera _camera;
    public Vector3 pointOfInterest;
    public Transform rootTransform;

    public float animationTime = 0.5f;
    public AnimationCurve animationCurve;



    private Quaternion m_Rotation = Quaternion.identity;
    private Quaternion m_InvRotation = Quaternion.identity;
    public Quaternion rotation
    {
        get { return m_Rotation; }
        set
        {
            m_Rotation = value;
            m_InvRotation = Quaternion.Inverse(rotation);
            var poiInRootSpace = rootTransform.InverseTransformPoint(pointOfInterest);

            rootTransform.localPosition = m_InvRotation * (-poiInRootSpace * scale) + pointOfInterest;
            rootTransform.localRotation = m_InvRotation;

        }
    }

    private float _scale = 1f;
    public float scale
    {
        set
        {
            _scale = value;

            var poiInRootSpace = rootTransform.InverseTransformPoint(pointOfInterest);
            rootTransform.localPosition = m_InvRotation * (-poiInRootSpace * _scale) + pointOfInterest;
        }

        get { return _scale; }
    }


    private void OnEnable()
    {
        Application.onBeforeRender += OnBeforeRender;
    }

    void OnDisable()
    {
        Application.onBeforeRender -= OnBeforeRender;
    }

    void OnBeforeRender()
    {
        _camera.transform.localPosition = Frame.Pose.position;
        _camera.transform.localRotation = Frame.Pose.rotation;

        rootTransform.localScale = Vector3.one * scale;
    }


    public void AlignWithPointOfInterest(Vector3 position)
    {

        var poiInRootSpace = rootTransform.InverseTransformPoint(position - pointOfInterest);
        rootTransform.localPosition = m_InvRotation * (-poiInRootSpace * scale);

    }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);


        instance = this;
        _camera = Camera.main;
    }


    public void InitializeStartScale()
    {
        scale = 5f / 0.10f;
    }

    public void SetScale(float scale)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateScale(scale, animationTime));
    }

    IEnumerator AnimateScale(float targetScale, float time)
    {
        float startScale = this.scale;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            this.scale = Mathf.Lerp(startScale, targetScale, animationCurve.Evaluate(elapsedTime / time));
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        this.scale = targetScale;
    }
}
