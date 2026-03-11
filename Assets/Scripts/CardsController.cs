using System;
using UnityEngine;

public class CardsController : MonoBehaviour
{
    [SerializeField] RadialLayautGroup radialLayautGroup;
    [SerializeField] Vector2 distanceRange;
    public bool isOpend = false;
    private void Update()
    {
        int cameraAngel = ((int)Camera.main.transform.rotation.eulerAngles.x + 180) % 360 - 180;
        radialLayautGroup.distance = isOpend ? Mathf.Lerp(distanceRange.x, distanceRange.y, cameraAngel/60.0f) : 0;
    }
}
