using UnityEngine;

public class TutorialCard2 : Card
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    public bool isTirigger {get; set;}
    protected override bool OnUse(RaycastHit hit)
    {
        if (hit.collider == null) return false;
        if (hit.collider.transform.parent == null) return false;
        if (hit.collider.transform.parent.TryGetComponent(out TutorialNotepad notepad))
        {
            audioSource.PlayOneShot(audioClip);
            TutorialGameManager.assistentLast = 2;
            if(isTirigger) TutorialGameManager.Instance.Trigger = true;
            return true;
        }
        return false;
    }
}
