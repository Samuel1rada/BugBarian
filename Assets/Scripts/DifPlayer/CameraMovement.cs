using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target; 
    [SerializeField] private Transform orientation; 
    [SerializeField] private float distanceFromTarget = 4.0f;
    [SerializeField] private float sensitivity = 80.0f;
    [SerializeField] private float verticalClampAngle = 80f;

    private float currentX = 0f; 
    private float currentY = 0f; 

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        currentX += mouseX;
        currentY -= mouseY;

        currentY = Mathf.Clamp(currentY, -verticalClampAngle, verticalClampAngle);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        Vector3 direction = new Vector3(0, 0, -distanceFromTarget);
        cam.transform.position = target.position + rotation * direction;

        cam.transform.LookAt(target);

 
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0;
        orientation.rotation = Quaternion.LookRotation(cameraForward);

        if (orientation != null)
        {
            orientation.rotation = Quaternion.LookRotation(cameraForward);
        }
    }
}

