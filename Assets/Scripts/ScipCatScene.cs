using UnityEngine;

public class ScipCatScene : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float duration;
    [SerializeField] KeyCode keyCode;
    
    float timer;
    
    void Update()
    {
        if(Input.GetKey(keyCode)) timer += Time.deltaTime;
        else timer = 0;
        if (timer >= duration) animator.SetTrigger("skip");        
    }
}
