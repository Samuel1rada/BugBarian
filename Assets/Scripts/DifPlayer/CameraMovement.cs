using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    private Vector3 previousPos;

    void Start()
    {
        previousPos = cam.ScreenToViewportPoint(Input.mousePosition);
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {

        Vector3 dir = previousPos - cam.ScreenToViewportPoint(Input.mousePosition);

        cam.transform.position = target.position; //new Vector3();

        cam.transform.Rotate(new Vector3(1, 0, 0), dir.y * 180);
        cam.transform.Rotate(new Vector3(0, 1, 0), dir.x * 180, Space.World);
        cam.transform.Translate(new Vector3(0, 0, -4));

        previousPos = cam.ScreenToViewportPoint(Input.mousePosition);
    }

}
