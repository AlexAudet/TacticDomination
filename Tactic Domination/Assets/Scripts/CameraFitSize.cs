using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitSize : MonoBehaviour
{
    Camera cam;
    public bool orthographique;

    public float sceneWidth = 10;
    public float horizontalFoV = 90.0f;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (!orthographique)
        {
            float halfWidth = Mathf.Tan(0.5f * horizontalFoV * Mathf.Deg2Rad);

            float halfHeight = halfWidth * Screen.height / Screen.width;

            float verticalFoV = 2.0f * Mathf.Atan(halfHeight) * Mathf.Rad2Deg;

            cam.fieldOfView = verticalFoV;
        }
        else
        {
            float unitsPerPixel = sceneWidth / Screen.width;

            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            cam.orthographicSize = desiredHalfHeight;
        }
    }
}
