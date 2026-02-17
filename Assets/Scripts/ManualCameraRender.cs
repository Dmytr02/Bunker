using UnityEngine;

public class ManualCameraRender : MonoBehaviour
{
    public Camera targetCamera;

    public void RenderCameraNow()
    {
        if (targetCamera != null)
            targetCamera.Render();
    }
}

