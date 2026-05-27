using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice; // Не забудьте добавить это пространство имен для FrameOut

public class RemoteVoiceIndicator : MonoBehaviourPun
{
    [SerializeField] private Speaker speaker;
    [SerializeField] float volumeSensitivity = 5f; 
    [SerializeField] Transform voiceIndicatorTransform;

    private float latestLoudness = 0f;
    private bool isSubscribed = false;

    void Awake()
    {
        if (speaker == null) speaker = GetComponent<Speaker>();
    }

    void Update()
    {
        if (photonView.IsMine) return;

        if (speaker != null)
        {
            if (speaker.IsLinked && speaker.RemoteVoice != null)
            {
                if (!isSubscribed)
                {
                    // Подписываемся на событие с новой сигнатурой
                    speaker.RemoteVoice.FloatFrameDecoded += OnAudioFrameDecoded;
                    isSubscribed = true;
                }
            }
            else if (isSubscribed)
            {
                Unsubscribe();
            }
        }

        // Анимация масштаба
        if (latestLoudness > 0.005f && speaker != null && speaker.IsPlaying)
        {
            float currentScale = voiceIndicatorTransform.localScale.x;
            float targetScale = Mathf.Lerp(currentScale, latestLoudness, Time.deltaTime * 15f);
            Debug.Log("targetScale - " + targetScale);
            voiceIndicatorTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
            
            latestLoudness = Mathf.Lerp(latestLoudness, 0f, Time.deltaTime * 5f);
        }
        else
        {
            Debug.Log("000");
            voiceIndicatorTransform.localScale = Vector3.Lerp(voiceIndicatorTransform.localScale, Vector3.zero, Time.deltaTime * 10f);
        }
    }

    // Новая сигнатура метода, соответствующая вашему событию
    private void OnAudioFrameDecoded(FrameOut<float> frame)
    {
        // Достаем массив сэмплов из свойства Buf контейнера FrameOut
        float[] buffer = frame.Buf;

        if (buffer == null || buffer.Length == 0) return;

        // Считаем RMS (среднеквадратичную громкость)
        float sum = 0f;
        for (int i = 0; i < buffer.Length; i++)
        {
            sum += buffer[i] * buffer[i];
        }
        float rms = Mathf.Sqrt(sum / buffer.Length);

        latestLoudness = Mathf.Clamp01(rms * volumeSensitivity);
    }

    private void Unsubscribe()
    {
        if (speaker != null && speaker.RemoteVoice != null && isSubscribed)
        {
            speaker.RemoteVoice.FloatFrameDecoded -= OnAudioFrameDecoded;
        }
        isSubscribed = false;
        latestLoudness = 0f;
    }

    void OnDisable() => Unsubscribe();
    void OnDestroy() => Unsubscribe();
}
