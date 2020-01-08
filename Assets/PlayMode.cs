using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayMode : Mode
{
    public CarController carController;

    public Slider steeringSlider;
    public bool isTouchingSlider;

    private float _previousSteeringInput;


    public bool isPositiveTorque;
    private bool _isPositiveTorque;

    public bool isNegativeTorque;
    private bool _isNegativeTorque;

    public void ChangePositiveMotorTorque(bool value)
    {
        isPositiveTorque = value;

    }


    public void ChangeNegativeMotorTorque(bool value)
    {
        isNegativeTorque = value;
    }

    public void ChangeTouchSlider(bool value)
    {
        isTouchingSlider = value;
    }

    public void BackToEditMode(bool value)
    {
        uiManager.instance.SetMode(ModState.EditMode);
    }

    public void Update()
    {
        if (isPositiveTorque != _isPositiveTorque)
        {
            carController.SetPositiveTorque(isPositiveTorque);
            _isPositiveTorque = isPositiveTorque;
        }

        if (isNegativeTorque != _isNegativeTorque)
        {
            carController.SetNegativeTorque(isNegativeTorque);
            _isNegativeTorque = isNegativeTorque;
        }

        if (_previousSteeringInput != steeringSlider.value)
        {
            _previousSteeringInput = steeringSlider.value;
            carController.steerInput = steeringSlider.value / steeringSlider.maxValue;

        }
        if (!isTouchingSlider && steeringSlider.value != 0)
        {
            if (steeringSlider.value > -0.02f && steeringSlider.value < 0.2f)
                steeringSlider.value = 0;

            steeringSlider.value = Mathf.Lerp(steeringSlider.value, 0, 4 * Time.deltaTime);
        }
    }

}
