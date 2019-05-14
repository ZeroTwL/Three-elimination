using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public float devHeight = 9.6f;
    public float devWidth = 6.4f;
     void Start()
    {
        float screenHeight = Screen.height;
        float orthographicSize = this.GetComponent<Camera>().orthographicSize;
        float aspectRation = Screen.width * 1.0f / screenHeight;
        float cameraWidth = orthographicSize * 2 * aspectRation;
        if (cameraWidth<devWidth)
        {
            orthographicSize = devWidth / (2 * aspectRation);
            this.GetComponent<Camera>().orthographicSize = orthographicSize;
        }

    }
}
