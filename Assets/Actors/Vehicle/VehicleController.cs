using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour {
  public PlayerInput playerInput;
  public List<WheelCollider> wheelColliders;
  public List<Transform> wheels;
  [Range(0, 90f)] public float maxSteerAngle = 30f;
  [Range(0, 1000f)] public float maxTorque = 125f;
  [Range(0, 10000f)] public float brakeTorque = 2000f;
  [Range(0, 1000f)] public float maxRpm = 250f;
  [Range(0, 5f)] public float boostModifier = 2.5f;

  [Range(0, 2f)] private bool boost = false;
  private float steerAngle = 0;
  private float torque = 0;


  void OnEnable() {
    // https://docs.unity3d.com/ScriptReference/WheelCollider.ConfigureVehicleSubsteps.html
    WheelCollider WheelColliders = GetComponentInChildren<WheelCollider>();
    WheelColliders.ConfigureVehicleSubsteps(2f, 8, 12); //(1000,20,20) = substeps fixed in 20
  }

  void SetTorque(float input) {
    input = Mathf.Clamp(input, -1f, 1f);
    torque = input * maxTorque * (boost ? boostModifier : 1f);
  }

  void SetSteer(float input) {
    input = Mathf.Clamp(input, -1f, 1f);
    steerAngle = input * maxSteerAngle;
  }

  void ApplyTorque() {
    foreach (WheelCollider wheel in wheelColliders) {
      if (Mathf.Abs(wheel.rpm) < maxRpm | boost) {
        wheel.motorTorque = torque;
      } else {
        wheel.motorTorque = 0;
      };

      if ((wheel.rpm > 0 & torque < 0) | (wheel.rpm < 0 & torque > 0)) {
        wheel.brakeTorque = brakeTorque;
      } else {
        wheel.brakeTorque = 0;
      }
    }
  }

  void ApplySteer() {
    for (int wheelIndex = 0; wheelIndex < 2; wheelIndex++) {
      WheelCollider wheelCollider = wheelColliders[wheelIndex];
      wheelCollider.steerAngle = steerAngle;
    }
  }

  public void OnMovement(InputAction.CallbackContext context) {
    Vector2 movement = context.ReadValue<Vector2>();
    SetSteer(movement.x);
    SetTorque(movement.y);
  }

  public void OnBoost(InputAction.CallbackContext context) {
    if (context.started) boost = true;
    if (context.canceled) boost = false;
  }

  void FixedUpdate() {
    ApplyTorque();
    ApplySteer();
  }

  void Update() {
    for (int wheelIndex = 0; wheelIndex < wheelColliders.Count; wheelIndex++) {
      WheelCollider wheelCollider = wheelColliders[wheelIndex];
      Transform wheel = wheels[wheelIndex];

      Vector3 wheelPosition;
      Quaternion wheelRotation;
      wheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
      wheel.position = wheelPosition;
      wheel.rotation = wheelRotation;
    }
  }
}
