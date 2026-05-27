using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    public void playSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
