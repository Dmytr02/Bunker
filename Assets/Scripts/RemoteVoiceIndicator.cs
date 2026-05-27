using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;

public class RemoteVoiceIndicator : MonoBehaviourPun
{
    [SerializeField] private Speaker speaker;
    [SerializeField] float volumeSensitivity = 2f;
    [SerializeField] Transform voiceIndicatorTransform;

    private float latestLoudness = 0f;
    private readonly object lockObject = new object();

    void Awake()
    {
        if (speaker == null) speaker = GetComponent<Speaker>();
    }

    // Этот метод автоматически вызывается движком Unity в аудио-потоке,
    // когда AudioSource (даже управляемый Photon) воспроизводит данные.
    void OnAudioFilterRead(float[] data, int channels)
    {
        // Если спикер не играет, сбрасываем громкость
        if (speaker == null || !speaker.IsPlaying)
        {
            lock (lockObject) { latestLoudness = 0f; }
            return;
        }

        // Считаем среднюю громкость (RMS) текущего аудио-кадра
        float sum = 0f;
        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i] * data[i];
        }
        float rms = Mathf.Sqrt(sum / data.Length);

        // Записываем результат в потокобезопасную переменную
        lock (lockObject)
        {
            latestLoudness = Mathf.Clamp01(rms * volumeSensitivity);
        }
    }

    void Update()
    {
        // Не обрабатываем собственный микрофон
        if (photonView.IsMine) return;

        float currentLoudness;
        lock (lockObject)
        {
            currentLoudness = latestLoudness;
        }

        if (speaker != null && speaker.IsPlaying && currentLoudness > 0.005f)
        {
            // Плавно масштабируем индикатор от 0 до 1 в зависимости от громкости
            float currentScale = voiceIndicatorTransform.localScale.x;
            float targetScale = Mathf.Lerp(currentScale, currentLoudness, Time.deltaTime * 15f);
            
            voiceIndicatorTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
        }
        else
        {
            // Если игрок молчит, плавно убираем индикатор в 0
            voiceIndicatorTransform.localScale = Vector3.Lerp(voiceIndicatorTransform.localScale, Vector3.zero, Time.deltaTime * 10f);
        }
    }
}