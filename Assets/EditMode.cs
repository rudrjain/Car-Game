using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditMode : Mode
{
    private float _currentScale;

    private float _previousSliderValue;

    public Slider scaleSlider;

    public TextMeshProUGUI measurementText;

    public AnimationCurve animationCurve;

    [Header("Ramp Properties")]
    public GameObject currentRamp;

    public LayerMask rampLayerMask;
    public LayerMask planeLayerMask;

    public TMP_Dropdown rampDropDown;

    public bool isPlacingRamp;

    [Header("Buttons")]
    public Button planeButton;
    public Button acceptButton;
    public GameObject rampPlaceButtons;
    public GameObject rampEditButtons;

    private void Start()
    {
        rampDropDown.ClearOptions();
        rampDropDown.AddOptions(RampManager.instance.GetRampNames());
        SetEditButtons(false);
        SetPlaceButtons(false);
    }

    public void OnAccept()
    {
        uiManager.instance.SetMode(ModState.PlayMode);
    }

    public void BackToPlaneMode()
    {
        uiManager.instance.SetMode(ModState.PlaneMode);
    }

    public void ChangeScale(bool value)
    {
        if (!value)
        {
            CheckScale(scaleSlider.value);
            ScaleManager.instance.SetScale(_currentScale);
        }
    }

    public void CheckScale(float scale)
    {
        scaleSlider.interactable = false;

        if (scale != _previousSliderValue)
        {
            _previousSliderValue = scale;
            Invoke("ActiveSlider", ScaleManager.instance.animationTime);
        }

        else
            ActivateSlider();

        switch (scale)
        {
            case 5:
                _currentScale = 1f;
                break;

            case 4:
                _currentScale = 5f / 1f;
                break;

            case 3:
                _currentScale = 5f / 0.5f;
                break;

            case 2:
                _currentScale = 5f / 0.25f;
                break;

            case 1:
                _currentScale = 5f / 0.10f;
                break;
            default:
                break;
        }
    }

    public void SetMeasurementText(float value)
    {
        switch (value)
        {
            case 5:
                measurementText.text = "5 M";
                break;

            case 4:
                measurementText.text = "1 M";
                break;

            case 3:
                measurementText.text = "50 CM";
                break;

            case 2:
                measurementText.text = "25 CM";
                break;

            case 1:
                measurementText.text = "10 CM";
                break;
            default:
                break;
        }

    }

    public void ActivateSlider()
    {
        scaleSlider.interactable = true;
    }

    public void OnStartPlacingRamp(int index)
    {
        currentRamp = RampManager.instance.GetAndActivateRamp(index);
        OnEditRamp();
    }

    public void OnEditRamp()
    {
        isPlacingRamp = true;
        SetEditButtons(false);
        SetPlaceButtons(true);

    }

    public void OnPlaceRamp()
    {
        isPlacingRamp = false;
        currentRamp = null;
        SetPlaceButtons(false);
    }

    public void OnRemoveRamp()
    {
        isPlacingRamp = false;
        StartCoroutine(ScaleAndRemoveRamp(currentRamp, 0.75f));
        SetEditButtons(false);
        SetPlaceButtons(false);
        currentRamp = null;
    }

    public void SetEditButtons(bool value)
    {
        rampEditButtons.SetActive(value);
        planeButton.interactable = !value;
        acceptButton.interactable = !value;
    }

    public void SetPlaceButtons(bool value)
    {
        rampPlaceButtons.SetActive(value);
        planeButton.interactable = !value;
        acceptButton.interactable = !value;
    }

    public void Update()
    {
        if (Input.touchCount <= 0)
            return;
        Touch touch = Input.GetTouch(0);

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if(!isPlacingRamp && Physics.Raycast(ray, out hit, float.MaxValue, rampLayerMask) && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            currentRamp = hit.transform.gameObject;
            SetEditButtons(true);
        }

        else if ( currentRamp!=null && !isPlacingRamp && Physics.Raycast(ray, out hit, float.MaxValue, ~rampLayerMask) && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            currentRamp = null;
            SetEditButtons(false);
        }

        if (!isPlacingRamp)
            return;

        DetectTouchMovement.Calculate();

        if(Physics.Raycast(ray, out hit, float.MaxValue, planeLayerMask) && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            currentRamp.transform.position = hit.point;

            if(Mathf.Abs(DetectTouchMovement.turnAngleDelta) > 0)
            {
                Vector3 rotationDeg = Vector3.zero;
                rotationDeg.y = -DetectTouchMovement.turnAngleDelta * 2;
                currentRamp.transform.rotation *= Quaternion.Euler(rotationDeg);
            }
        }
    }

    public IEnumerator ScaleAndRemoveRamp(GameObject ramp, float time)
    {
        float elapsedTime = 0;
        Vector3 startScale = ramp.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while(elapsedTime<time)
        {
            ramp.transform.localScale = Vector3.Lerp(startScale, targetScale, animationCurve.Evaluate(elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        RampManager.instance.DeactiveRamp(ramp);

    }

}
