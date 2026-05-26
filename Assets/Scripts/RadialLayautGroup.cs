using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RadialLayautGroup : MonoBehaviour
{
    RectTransform rectTransform => transform as RectTransform;
    [SerializeField] [MinMaxSlider(-180.0f, 180.0f)]
    Vector2 angelRange;
    [SerializeField] public float distance;
    void Start()
    {
        
    }
    void Update()
    {
        int allWeighted = 0;
        float actualWeighted = 0;
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            if(!rectTransform.GetChild(i).gameObject.activeSelf) continue;
            if (rectTransform.GetChild(i).TryGetComponent(out IRadialLayautGroupWeighted weighted))
                allWeighted += weighted.Weight;
            else allWeighted += 1;
        }
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            if(!rectTransform.GetChild(i).gameObject.activeSelf) continue;
            RectTransform child = rectTransform.GetChild(i) as RectTransform;
            rectTransform.GetChild(i).TryGetComponent(out IRadialLayautGroupWeighted weighted);
            
            if (weighted != null) actualWeighted += weighted.Weight*0.5f;
            else actualWeighted += 0.5f;
            float angel = Mathf.Deg2Rad*Mathf.Lerp(angelRange.x, angelRange.y, ((float)actualWeighted) / ((float)allWeighted));
            if (weighted != null) actualWeighted += weighted.Weight*0.5f;
            else actualWeighted += 0.5f;
            
            child.anchoredPosition = Vector2.Lerp(child.anchoredPosition, new Vector2(Mathf.Sin(angel), Mathf.Cos(angel))*distance, 0.05f);
            child.rotation = Quaternion.LookRotation(rectTransform.position - child.position, -transform.forward) * Quaternion.Euler(90, 0, 0);
        }
    }
}

interface IRadialLayautGroupWeighted
{
    int Weight { get; }
}
