using System;
using NaughtyAttributes;
using UnityEngine;

public class ManualCameraRender : MonoBehaviour
{
    public Camera targetCamera;

    private void Start()
    {
        if (targetCamera == null) targetCamera = GetComponent<Camera>();
    }

    [Button]
    public void RenderCameraNow()
    {
        if (targetCamera != null)
            targetCamera.Render();
    }
}

