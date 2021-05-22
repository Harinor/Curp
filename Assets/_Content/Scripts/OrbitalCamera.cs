using UnityEngine;
using InputSource = UnityEngine.Input;

public class OrbitalCamera : MonoBehaviour
{

    public float xRot = 0f;
    public float yRot = 0f;

    public float distance = 5f;
    public float sensitivity = 1000f;
    public Transform target;

    public bool groundLevelClamp = true;
    public float heightOffset = 0.2f;


    #region ---UnityCallbacks---
    private void Start()
    {
        UpdateCameraPos();
    }

    private void Update()
    {
        if (InputSource.GetMouseButton(0))
        {
            UpdateCameraPos();
        }
    }

    #endregion

    private void UpdateCameraPos()
    {
        xRot += InputSource.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        yRot += InputSource.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

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
}