using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrbitalCamera : MonoBehaviour
{
    public float xRot = 0f;
    public float yRot = 0f;

    public float distance = 5f;
    public float sensitivity = 1000f;
    [SerializeField] Transform target;

    [SerializeField, Tooltip("Prevents camera from going below the ground level.")]
    private bool groundLevelClamp = true;
    public float heightOffset = 0.2f;

    [SerializeField, Tooltip("Enables automnatic idle mode.")]
    private bool isIdleAllowed = true;

    [SerializeField, Tooltip("Time required for camera to enter the idle state.")]
    float inactivityLimit = 5f;
    float inactivityTimer = 0;
    float xIdleRot = 0;
    float yIdleRot = 0.01f;

    private bool rotationEnabled = true;

    #region ---UnityCallbacks---
    private void Start()
    {
        if (IsTouchScreen())
        {
            sensitivity = 5f;
            yIdleRot *= 100;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !IsPointerOverUIObject() && IsInValidScreenSection() && rotationEnabled)
        {
            ProcessInputActive();
        }
        else
        {
            ProcessInputIdle();
        } 
    }

    private void ProcessInputActive()
    {
        MasterManager.IsIdle = false;
        UpdateCameraPos();
        inactivityTimer = 0;
    }
    
    private void ProcessInputIdle()
    {
        inactivityTimer += Time.deltaTime;
        if ((inactivityTimer > inactivityLimit) && isIdleAllowed)
        {
            MasterManager.IsIdle = true;
            UpdateCameraPos();
        }
    }
    #endregion

    #region --- METHODS ---
    private void UpdateCameraPos()
    {
        if (IsTouchScreen())
        {
            xRot += ((!MasterManager.IsIdle) ? Input.GetTouch(0).deltaPosition.y : xIdleRot) * sensitivity * Time.deltaTime;
            yRot += ((!MasterManager.IsIdle) ? Input.GetTouch(0).deltaPosition.x : yIdleRot) * sensitivity * Time.deltaTime;
        }
        else
        {
            xRot += ((!MasterManager.IsIdle) ? Input.GetAxis("Mouse Y") : xIdleRot) * sensitivity * Time.deltaTime;
            yRot += ((!MasterManager.IsIdle) ? Input.GetAxis("Mouse X") : yIdleRot) * sensitivity * Time.deltaTime;
        }


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

    static bool IsInValidScreenSection()
    {
        if (Input.mousePosition.y < Screen.height / 2.0f && MasterManager.instance.cameraManager.primaryCamera.rect.y != 0)
            return false;

        //if (Input.GetTouch(0).position.y < Screen.height / 2.0f) return false;

        return true;
    }

    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        for (int touchIndex = 0; touchIndex < Input.touchCount; touchIndex++)
        {
            Touch touch = Input.GetTouch(touchIndex);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return true;
        }

        return false;
    } 

    private bool IsTouchScreen()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return true;
        }
        return false;
    }

    public void SetGroundLevelClamp(bool value)
    {
        groundLevelClamp = value;
    }

    public void SetIsIdleAllowed(bool value)
    {
        isIdleAllowed = value;
        if (value == false)
        {
            MasterManager.IsIdle = false;
        }
    }
    #endregion

}