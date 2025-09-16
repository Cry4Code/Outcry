using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public enum VolumeType
{
    Master,
    BGM,
    SFX
}

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Mixer & Sources")]
    [SerializeField] private AudioMixer gameAudioMixer;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioSource bgmSource;

    [Header("Object Pooling")]
    [SerializeField] private AudioPlayer audioPlayerPrefab;
    [SerializeField] private int poolSize = 10;

    [Header("Sound Container")]
    [Tooltip("게임에서 사용할 모든 사운드 정보를 담은 ScriptableObject")]
    [SerializeField] private SoundContainer[] soundContainers;

    private Queue<AudioPlayer> audioPlayerPool = new Queue<AudioPlayer>();
    private Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();
    private Coroutine duckingCoroutine;

    // 믹서 파라미터 이름
    private const string MASTER_VOLUME_PARAM = "Master_Volume";
    private const string BGM_VOLUME_PARAM = "BGM_Volume";
    private const string SFX_VOLUME_PARAM = "SFX_Volume";

    private void Awake()
    {
        InitializePool();
        InitializeSoundContainer();
        ResetVolumes();
    }

    // 지정된 크기만큼 AudioPlayer 오브젝트를 미리 생성하여 풀에 저장
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioPlayer newPlayer = Instantiate(audioPlayerPrefab, transform);
            newPlayer.gameObject.SetActive(false);
            audioPlayerPool.Enqueue(newPlayer);
        }
    }

    // SoundContainer의 사운드들을 딕셔너리에 등록
    private void InitializeSoundContainer()
    {
        if (soundContainers == null || soundContainers.Length == 0)
        {
            Debug.LogError("SoundContainer array is not assigned or empty.");
            return;
        }

        foreach (SoundContainer container in soundContainers)
        {
            if (container == null || container.sounds == null)
            {
                continue;
            }

            foreach (Sound sound in container.sounds)
            {
                // 모든 컨테이너에 걸쳐 사운드 이름 고유해야 함
                if (!soundDict.ContainsKey(sound.name))
                {
                    soundDict.Add(sound.name, sound);
                }
                else
                {
                    // 어떤 컨테이너에서 중복이 발생했는지 알려주면 디버깅에 용이합니다.
                    Debug.LogWarning($"Duplicate sound name '{sound.name}' found in container '{container.name}'. Skipping this entry.");
                }
            }
        }
    }

    private AudioPlayer GetAudioPlayerFromPool()
    {
        if (audioPlayerPool.Count > 0)
        {
            AudioPlayer player = audioPlayerPool.Dequeue();
            player.gameObject.SetActive(true);
            return player;
        }
        else
        {
            AudioPlayer newPlayer = Instantiate(audioPlayerPrefab, transform);
            newPlayer.GetComponent<AudioSource>().outputAudioMixerGroup = sfxMixerGroup;
            return newPlayer;
        }
    }

    public void ReturnAudioPlayerToPool(AudioPlayer player)
    {
        player.gameObject.SetActive(false);
        audioPlayerPool.Enqueue(player);
    }

    // 지정된 이름의 BGM 재생
    public async void PlayBGM(string name, bool loop = true)
    {
        if(!soundDict.TryGetValue(name, out Sound sound))
        {
            Debug.LogWarning($"BGM sound '{name}' not found!");
            return;
        }

        AudioClip clip = await ResourceManager.Instance.LoadAssetAddressableAsync<AudioClip>(sound.address);
        if(clip == null)
        {
            Debug.LogWarning($"Failed to load BGM clip from address: {sound.address}");
            return;
        }

        bgmSource.clip = clip;
        bgmSource.volume = sound.volume;
        bgmSource.pitch = sound.pitch;
        bgmSource.loop = loop;
        bgmSource.outputAudioMixerGroup = bgmMixerGroup;
        bgmSource.Play();
    }

    // 지정된 이름의 SFX를 특정 위치에서 재생
    public async void PlaySFX(string name, Vector3 position)
    {
        if (!soundDict.TryGetValue(name, out Sound sound))
        {
            Debug.LogWarning($"SFX sound '{name}' not found!");
            return;
        }

        AudioClip clip = await ResourceManager.Instance.LoadAssetAddressableAsync<AudioClip>(sound.address);
        if (clip == null)
        {
            Debug.LogWarning($"Failed to load SFX clip from address: {sound.address}");
            return;
        }

        AudioPlayer player = GetAudioPlayerFromPool();
        if(player != null)
        {
            player.transform.position = position;
            player.gameObject.SetActive(true);
            player.Play(clip, sound.volume, sound.pitch);
        }
    }

    // 볼륨 조절용
    public void SetVolume(VolumeType type, float volume)
    {
        // 어떤 볼륨을 조절할지 결정
        string param = type == VolumeType.Master ? MASTER_VOLUME_PARAM :
                       type == VolumeType.BGM ? BGM_VOLUME_PARAM : SFX_VOLUME_PARAM;

        // 0~1 사이의 선형 볼륨 값을 로그 스케일(dB)로 변환(UI 값을 오디오 엔진 값으로 변환)
        float volumeDB = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20f;

        // 변환된 값으로 실제 오디오 믹서의 볼륨 조절
        gameAudioMixer.SetFloat(param, volumeDB);
    }

    public float GetVolume(VolumeType type)
    {
        string param = type == VolumeType.Master ? MASTER_VOLUME_PARAM :
                       type == VolumeType.BGM ? BGM_VOLUME_PARAM : SFX_VOLUME_PARAM;

        gameAudioMixer.GetFloat(param, out float value);

        return Mathf.Pow(10f, value / 20f);
    }

    // 설정 초기화용
    public void ResetVolumes()
    {
        SetVolume(VolumeType.Master, 1f);
        SetVolume(VolumeType.BGM, 1f);
        SetVolume(VolumeType.SFX, 1f);
    }

    #region 사운드 에셋 관리
    // 지정된 이름의 사운드 클립들을 미리 로드하여 캐싱 (예: 로딩 화면에서 호출)
    public async Task LoadSFX(params string[] names)
    {
        List<Task> loadTasks = new List<Task>();
        foreach (string name in names)
        {
            if (soundDict.TryGetValue(name, out Sound sound))
            {
                loadTasks.Add(ResourceManager.Instance.LoadAssetAddressableAsync<AudioClip>(sound.address));
            }
            else
            {
                Debug.LogWarning($"SFX sound '{name}' not found!");
            }
        }
        await Task.WhenAll(loadTasks);
        Debug.Log($"[AudioManager] {names.Length} SFX preloaded.");
    }

    // 지정된 이름의 사운드 클립들을 메모리에서 해제 (예: 스테이지 종료 시 호출)
    public void UnloadSFX(params string[] names)
    {
        foreach (string name in names)
        {
            if (soundDict.TryGetValue(name, out Sound sound))
            {
                ResourceManager.Instance.ReleaseAddressableAsset(sound.address);
            }
            else
            {
                Debug.LogWarning($"SFX sound '{name}' not found!");
            }
        }

        Debug.Log($"[AudioManager] {names.Length} SFX unloaded.");
    }
    #endregion

    #region 믹서 컨트롤
    // BGM 볼륨을 일시적으로 줄였다가 복원하는 효과 실행
    public void TriggerBgmDucking()
    {
        if (duckingCoroutine != null)
        {
            StopCoroutine(duckingCoroutine);
        }

        duckingCoroutine = StartCoroutine(BgmDuckingCoroutine());
    }

    private IEnumerator BgmDuckingCoroutine()
    {
        yield return FadeMixerVolume(BGM_VOLUME_PARAM, 0.1f, -15f); // 0.1초 동안 볼륨 -15dB 감소
        yield return new WaitForSeconds(0.3f); // 0.3초 대기
        yield return FadeMixerVolume(BGM_VOLUME_PARAM, 0.5f, 0f); // 0.5초 동안 볼륨 원래대로 복구
    }

    private IEnumerator FadeMixerVolume(string parameter, float duration, float targetValue)
    {
        float currentTime = 0;
        gameAudioMixer.GetFloat(parameter, out float startValue);

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime; // Time.timeScale에 영향받지 않음
            float newValue = Mathf.Lerp(startValue, targetValue, currentTime / duration);
            gameAudioMixer.SetFloat(parameter, newValue);
            yield return null;
        }
        gameAudioMixer.SetFloat(parameter, targetValue);
    }
    #endregion
}
