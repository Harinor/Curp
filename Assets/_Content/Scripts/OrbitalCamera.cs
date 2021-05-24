using UnityEngine;
using UnityEngine.EventSystems;
using InputSource = UnityEngine.Input;

public class OrbitalCamera : MonoBehaviour
{

    public float xRot = 0f;
    public float yRot = 0f;

    public float distance = 5f;
    public float sensitivity = 1000f;
    Transform target;

    [SerializeField, Tooltip("Prevents carema from going below the ground level.")]
    private bool groundLevelClamp = true;
    public float heightOffset = 0.2f;

    [SerializeField, Tooltip("Time refuired for camera to enter the idle state.")]
    float inactivityLimit = 5f;
    float inactivityTimer = 0;
    float xIdleRot = 0;
    float yIdleRot = 0.01f;

    #region ---UnityCallbacks---
    private void Start()
    {
        UpdateActiveVehicle();
        //UpdateCameraPos();
    }

    private void Update()
    {
        UpdateActiveVehicle(); 

        if (InputSource.GetMouseButton(0))
        {
            SharedData.IsIdle = false;
            UpdateCameraPos();
            inactivityTimer = 0;
        }
        else
        {
            inactivityTimer += Time.deltaTime;
            if (inactivityTimer > inactivityLimit)
            {
                SharedData.IsIdle = true;
                UpdateCameraPos();
            }
        }
    }

    #endregion

    private void UpdateCameraPos()
    {
        xRot += ( (!SharedData.IsIdle) ? InputSource.GetAxis("Mouse Y") : xIdleRot) * sensitivity * Time.deltaTime;
        yRot += ( (!SharedData.IsIdle) ? InputSource.GetAxis("Mouse X") : yIdleRot) * sensitivity * Time.deltaTime;

        if (xRot > 90f)
        {
            xRot = 90f;
        }
        else if (xRot < -90f)
        {
            xRot = -90f;
        }

        transform.position = target.position + Quaternion.Euler(xRot, yRot, 0f) * (distance * -Vector3.back);

        if (groundLevelClamp && target.position.y + heightOffset > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, target.position.y + heightOffset, transform.position.z);
        }

        transform.LookAt(target.position, Vector3.up);
    }

    /// <summary>
    /// Sets the camera's target to the Active Vehicle.
    /// </summary>
    private void UpdateActiveVehicle()
    {
        target = SharedData.ActiveVehicle.transform;
    }
}