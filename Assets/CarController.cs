using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{


    [Header("Input")]
    public float motorInput;
    public float steerInput;

    public float maxMotorTorque;
    public float maxSteeringAngle;

    public float currentMotorInput;
    public float currentSteeringInput;

    public bool isAddingPositiveTorque;
    public bool isAddingNegativeTorque;

    [Header("References")]
    public List<AxleInfo> axleInfos = new List<AxleInfo>();

    private Rigidbody _rigidbody;
    public Transform centreOfMass;

    //    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centreOfMass.localPosition;
    }

    public void SetPositiveTorque(bool torque)
    {
        isAddingPositiveTorque = torque;
    }

    public void SetNegativeTorque(bool torque)
    {
        isAddingNegativeTorque = torque;
    }

    public void CheckInput()
    {
//#if UNITY_ANDROID
        currentMotorInput = maxMotorTorque * motorInput;
        currentSteeringInput = maxSteeringAngle * steerInput;
//#endif
//#if UNITY_EDITOR
//        currentMotorInput = maxMotorTorque * Input.GetAxis("Vertical");
//        currentSteeringInput = maxSteeringAngle * Input.GetAxis("Horizontal");
//#endif
    }

    //    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAddingPositiveTorque && !isAddingNegativeTorque)
            motorInput = Mathf.Lerp(motorInput, 1, 2 * Time.deltaTime);
        else if (!isAddingPositiveTorque && isAddingNegativeTorque)
            motorInput = Mathf.Lerp(motorInput, -1, 2 * Time.deltaTime);
        else if (isAddingPositiveTorque && isAddingNegativeTorque)
            motorInput = Mathf.Lerp(motorInput, 0, 4 * Time.deltaTime);
        else
            motorInput = Mathf.Lerp(motorInput, 0, 4 * Time.deltaTime);

        CheckInput();

        foreach (AxleInfo info in axleInfos)
        {
            if (info.isMotor)
            {
                info.rightwheel.motorTorque = currentMotorInput;
                info.leftwheel.motorTorque = currentMotorInput;
            }

            if (info.isSteering)
            {
                info.rightwheel.steerAngle = currentSteeringInput;
                info.leftwheel.steerAngle = currentSteeringInput;
            }

            MoveVisualWheels(info.rightwheel, info.visualRightWheel);
            MoveVisualWheels(info.leftwheel, info.visualLeftWheel);
        }
    }

    public void MoveVisualWheels(WheelCollider wheelCollider, Transform WheelTransform)
    {
        Vector3 position;
        Quaternion rotation;

        wheelCollider.GetWorldPose(out position, out rotation);

        WheelTransform.transform.position = position;
        WheelTransform.transform.rotation = rotation;
    }
}
[System.Serializable]
public class AxleInfo
{
    public WheelCollider rightwheel;
    public WheelCollider leftwheel;

    public Transform visualRightWheel;
    public Transform visualLeftWheel;

    public bool isMotor;
    public bool isSteering;
}