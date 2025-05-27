using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class RollComponent : MonoBehaviour
{
    [SerializeField] float turnSpeed = 30;
    [SerializeField] float smoothTime = 0.02f;
    [SerializeField] float maxBankAngle = 20f;

    [SerializeField] private Transform model;


    float prevYaw;          
    float currentBank;      
    float bankVelocity;    


    float turnInput;       

    void Update()
    {
        float yaw = transform.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(prevYaw, yaw);
        prevYaw = yaw;

        float t = Mathf.Clamp(deltaYaw / (turnSpeed * Time.deltaTime), -1f, 1f);
        float targetBank = -t * maxBankAngle;

        currentBank = Mathf.SmoothDampAngle(
            currentBank,
            targetBank,
            ref bankVelocity,
            smoothTime
        );

        if (model != null)
        {
            var e = model.localEulerAngles;
            e.z = currentBank;
            model.localEulerAngles = e;
        }
    }

    public void SetTurnInput(Vector2 input)
    {
        turnInput = Mathf.Clamp(input.x, -1f, 1f);
    }
}
