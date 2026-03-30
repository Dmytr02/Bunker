using UnityEngine;

public class TutorialCard2 : Card
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    protected override bool OnUse(RaycastHit hit)
    {
        if (hit.collider.transform.parent == null) return false;
        if (hit.collider.transform.parent.parent == null) return false;
        if (hit.collider.transform.parent.parent.parent == null) return false;
        if (hit.collider.transform.parent.parent.parent.TryGetComponent(out TutorialNotepad notepad))
        {
            audioSource.PlayOneShot(audioClip);
            return true;
        }
        return false;
    }
}
