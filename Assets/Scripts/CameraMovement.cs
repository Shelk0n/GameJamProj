using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float speed = 1f;
       
    private Vector3 dragOrigin;

    [SerializeField] private float zoomStep = 0.1f, minCamSize, MaxCamSize;

    void Update()
    {
        if (Screen.width - Input.mousePosition.x < 10 || Screen.width - Input.mousePosition.x > Screen.width - 10 ||
            Screen.height - Input.mousePosition.y < 10 || Screen.height - Input.mousePosition.y > Screen.height - 10)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), speed * Time.deltaTime);
        }
        PanCamera();
        Zoom();
            
    }
    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            gameObject.transform.position += difference;
        }

    }
    public void Zoom()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Debug.Log(Camera.main.orthographicSize);
            Camera.main.orthographicSize = Camera.main.orthographicSize - zoomStep;

        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize = 6;
        }
    }
}
