using System.Collections;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    [SerializeField] Vector2 ON_time = new Vector2(5, 10);
    [SerializeField] Vector2 OFF_time = new Vector2(0.01f, 0.1f);
    [SerializeField] Light light;
    [SerializeField] GameObject gameObject;
    void Start()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            light.enabled = true;
            if(gameObject) gameObject.SetActive(true);
            yield return new WaitForSeconds(Random.Range(ON_time.x, ON_time.y));
            light.enabled = false;
            if(gameObject) gameObject.SetActive(false);
            yield return new WaitForSeconds(Random.Range(OFF_time.x, OFF_time.y));
        }
    }
}
