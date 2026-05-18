using UnityEngine;

public class CutScene : MonoBehaviour
{
    public void Trigger()
    {
        if(TutorialGameManager.Instance) TutorialGameManager.Instance.Trigger = true;
    }
}
