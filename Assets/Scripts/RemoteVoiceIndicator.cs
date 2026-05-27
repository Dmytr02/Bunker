using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;

public class RemoteVoiceIndicator : MonoBehaviourPun
{
    [SerializeField] private Speaker speaker;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] float volumeSensitivity = 2f;
    [SerializeField] Transform voiceIndicatorTransform;
    
    // Массив для буфера аудиоданных (256 сэмплов обычно достаточно для замера)
    private float[] audioSamples = new float[256];

    void Awake()
    {
        speaker = GetComponent<Speaker>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (photonView.IsMine) return;

        if (speaker != null && speaker.IsPlaying && audioSource.isPlaying)
        {
            // Берем текущий кусочек аудиоданных из AudioSource
            audioSource.GetOutputData(audioSamples, 0);

            // Считаем среднюю громкость (RMS)
            float sum = 0f;
            for (int i = 0; i < audioSamples.Length; i++)
            {
                sum += audioSamples[i] * audioSamples[i];
            }
            float rms = Mathf.Sqrt(sum / audioSamples.Length);

            // rms выдает значения примерно от 0 до 0.5 при обычном разговоре. 
            // Нормализуем его (умножаем на коэффициент чувствительности, например, 2)
            float loudness = Mathf.Clamp01(rms * volumeSensitivity);

            // Визуализация: меняем размер объекта
            float targetScale = Mathf.Lerp(0f, 1f, loudness);
            voiceIndicatorTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
        }
        else
        {
            // Если игрок молчит, плавно возвращаем его к обычному размеру
            voiceIndicatorTransform.localScale = Vector3.Lerp(voiceIndicatorTransform.localScale, Vector3.zero, Time.deltaTime * 10f);
        }
    }
}