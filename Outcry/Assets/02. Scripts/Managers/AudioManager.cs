using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Audio;

public enum EVolumeType
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
    [Tooltip("풀이 가득 찼을 때 추가로 생성할 오디오 플레이어의 수")]
    [SerializeField] private int poolExpansionAmount = 10;

    [Header("Sound Container")]
    [Tooltip("게임에서 사용할 모든 사운드 정보를 담은 ScriptableObject")]
    [SerializeField] private SoundContainer[] soundContainers;

    private Queue<AudioPlayer> audioPlayerPool;
    private Dictionary<string, Sound> soundDict;
    private Coroutine duckingCoroutine;

    // 음소거 상태 및 이전 볼륨 저장을 위한 변수
    private bool isMasterMuted;
    private bool isBgmMuted;
    private bool isSfxMuted;

    private float lastMasterVolume;
    private float lastBgmVolume;
    private float lastSfxVolume;

    // 볼륨 설정이 외부 요인(초기화, 데이터 로드 등)에 의해 변경되었을 때 호출
    public static event Action OnVolumeSettingsChanged;
    // 음소거 상태가 변경될 때 UI에 알리기 위한 이벤트
    public static event Action<EVolumeType, bool> OnMuteStateChanged;

    // 믹서 파라미터 이름
    private const string MASTER_VOLUME_PARAM = "Master";
    private const string BGM_VOLUME_PARAM = "BGM";
    private const string SFX_VOLUME_PARAM = "SFX";

    private void Awake()
    {
        InitializePool();
        InitializeSoundContainer();
    }

    private void Start()
    {
        // 볼륨 기본값으로 초기화
        ResetToDefaultState();

        // TODO: 초기 볼륨 설정(파이어베이스에서 불러오기?)
    }

    #region 초기화
    // 지정된 크기만큼 AudioPlayer 오브젝트를 미리 생성하여 풀에 저장
    private void InitializePool()
    {
        audioPlayerPool = new Queue<AudioPlayer>();

        ExpandPool(poolExpansionAmount);
    }

    // SoundContainer의 사운드들을 딕셔너리에 등록
    private void InitializeSoundContainer()
    {
        if (soundContainers == null || soundContainers.Length == 0)
        {
            Debug.LogError("SoundContainer array is not assigned or empty.");
            return;
        }

        soundDict = new Dictionary<string, Sound>();

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

    private void ResetToDefaultState()
    {
        // 변수 초기화
        isMasterMuted = false;
        isBgmMuted = false;
        isSfxMuted = false;

        // 볼륨 초기화
        ResetVolumes();
    }
    #endregion

    #region 오브젝트 풀링
    // 지정된 개수만큼 오디오 플레이어를 생성하여 풀에 추가
    private void ExpandPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AudioPlayer newPlayer = Instantiate(audioPlayerPrefab, transform);

            AudioSource audioSource = newPlayer.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = sfxMixerGroup;
            }

            newPlayer.gameObject.SetActive(false);
            audioPlayerPool.Enqueue(newPlayer);
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
            ExpandPool(poolExpansionAmount);

            AudioPlayer newPlayer = audioPlayerPool.Dequeue();
            newPlayer.gameObject.SetActive(true);

            return newPlayer;
        }
    }

    public void ReturnAudioPlayerToPool(AudioPlayer player)
    {
        player.gameObject.SetActive(false);
        audioPlayerPool.Enqueue(player);
    }
    #endregion

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
        bgmSource.playOnAwake = false;
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

    #region 볼륨 조절
    // 볼륨 조절용
    public void SetVolume(EVolumeType type, float volume)
    {
        // 어떤 볼륨을 조절할지 결정
        string param = type == EVolumeType.Master ? MASTER_VOLUME_PARAM :
                       type == EVolumeType.BGM ? BGM_VOLUME_PARAM : SFX_VOLUME_PARAM;

        // 0~1 사이의 선형 볼륨 값을 로그 스케일(dB)로 변환(UI 값을 오디오 엔진 값으로 변환)
        float volumeDB = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20f;

        // 이 로그가 콘솔에 찍히는지 param과 volumeDB 값이 정상적인지 확인
        Debug.Log($"[AudioManager] Setting Mixer Param: '{param}' to {volumeDB} dB");

        // 변환된 값으로 실제 오디오 믹서의 볼륨 조절
        gameAudioMixer.SetFloat(param, volumeDB);
    }

    public float GetVolume(EVolumeType type)
    {
        string param = type == EVolumeType.Master ? MASTER_VOLUME_PARAM :
                       type == EVolumeType.BGM ? BGM_VOLUME_PARAM : SFX_VOLUME_PARAM;

        gameAudioMixer.GetFloat(param, out float value);

        return Mathf.Pow(10f, value / 20f);
    }

    // 음소거 토글
    public void ToggleMute(EVolumeType type)
    {
        // 현재 상태의 반대로 설정
        switch (type)
        {
            case EVolumeType.Master:
                SetMute(EVolumeType.Master, !isMasterMuted);
                break;

            case EVolumeType.BGM:
                SetMute(EVolumeType.BGM, !isBgmMuted);
                break;

            case EVolumeType.SFX:
                SetMute(EVolumeType.SFX, !isSfxMuted);
                break;

            default:
                Debug.LogWarning("Invalid volume type for mute operation.");
                return;
        }
    }

    public void SetMute(EVolumeType type, bool isMute)
    {
        string param = GetMixerParam(type);

        // 대상 타입에 따라 상태 변수 및 값 업데이트
        switch (type)
        {
            case EVolumeType.Master:
                if (isMasterMuted == isMute)
                {
                    return; // 상태 변경이 없으면 종료
                }
                isMasterMuted = isMute;

                if (isMute)
                {
                    gameAudioMixer.GetFloat(param, out lastMasterVolume); // 현재 볼륨 저장
                    gameAudioMixer.SetFloat(param, -80f); // 음소거
                }
                else
                {
                    gameAudioMixer.SetFloat(param, lastMasterVolume); // 저장된 볼륨 복원
                }
                break;

            case EVolumeType.BGM:
                if (isBgmMuted == isMute)
                {
                    return;
                }

                isBgmMuted = isMute;
                if (isMute)
                {
                    gameAudioMixer.GetFloat(param, out lastBgmVolume);
                    gameAudioMixer.SetFloat(param, -80f);
                }
                else
                {
                    gameAudioMixer.SetFloat(param, lastBgmVolume);
                }
                break;

            case EVolumeType.SFX:
                if (isSfxMuted == isMute)
                {
                    return;
                }

                isSfxMuted = isMute;
                if (isMute)
                {
                    gameAudioMixer.GetFloat(param, out lastSfxVolume);
                    gameAudioMixer.SetFloat(param, -80f);
                }
                else
                {
                    gameAudioMixer.SetFloat(param, lastSfxVolume);
                }
                break;
        }

        // UI 업데이트를 위해 이벤트 호출
        OnMuteStateChanged?.Invoke(type, isMute);
    }

    // 현재 음소거 상태 반환
    public bool GetMuteState(EVolumeType type)
    {
        switch (type)
        {
            case EVolumeType.Master: 
                return isMasterMuted;
            case EVolumeType.BGM: 
                return isBgmMuted;
            case EVolumeType.SFX: 
                return isSfxMuted;
            default: 
                return false;
        }
    }

    private string GetMixerParam(EVolumeType type)
    {
        switch (type)
        {
            case EVolumeType.Master: 
                return MASTER_VOLUME_PARAM;
            case EVolumeType.BGM: 
                return BGM_VOLUME_PARAM;
            case EVolumeType.SFX: 
                return SFX_VOLUME_PARAM;
            default: 
                return string.Empty;
        }
    }

    // 설정 초기화용
    public void ResetVolumes()
    {
        SetVolume(EVolumeType.Master, 1f);
        SetVolume(EVolumeType.BGM, 1f);
        SetVolume(EVolumeType.SFX, 1f);

        // 슬라이더 UI도 업데이트되어야 하므로 이벤트 호출
        OnVolumeSettingsChanged?.Invoke();
    }

    // 외부(Firebase 등)에서 불러온 설정 값으로 모든 볼륨 한 번에 적용
    public void ApplyAllVolumeSettings(float master, float bgm, float sfx)
    {
        SetVolume(EVolumeType.Master, master);
        SetVolume(EVolumeType.BGM, bgm);
        SetVolume(EVolumeType.SFX, sfx);

        // 외부에서 데이터가 로드되어 볼륨이 바뀌었으므로 UI 업데이트 이벤트 호출
        OnVolumeSettingsChanged?.Invoke();
    }
    #endregion

    #region 사운드 에셋 관리
    // 지정된 이름의 사운드 클립들을 미리 로드하여 캐싱 (예: 로딩 화면에서 호출)
    public async Task PreloadSFX(params string[] names)
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
