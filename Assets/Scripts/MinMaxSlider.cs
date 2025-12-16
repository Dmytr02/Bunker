using UnityEngine;

public class MinMaxSlider : PropertyAttribute
{
    public float min, max;
    public MinMaxSlider(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}