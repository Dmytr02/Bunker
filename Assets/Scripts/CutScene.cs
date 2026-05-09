using UnityEngine;

public class CutScene : MonoBehaviour
{
    public void Trigger()
    {
        TutorialGameManager.Instance.Trigger = true;
    }
}
