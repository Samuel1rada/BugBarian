using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;

    private Vector3 previousPos;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            previousPos = cam.ScreenToViewportPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0)) 
        {
            Vector3 dir = previousPos - cam.ScreenToViewportPoint(Input.mousePosition);

            cam.transform.position = target.position; //new Vector3();

            cam.transform.Rotate(new Vector3(1, 0, 0), dir.y * 180);
            cam.transform.Rotate(new Vector3(0, 1, 0), dir.x * 180, Space.World);
            cam.transform.Translate(new Vector3(0,0,-10));


            previousPos = cam.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}
