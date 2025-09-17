using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private EVolumeType volumeType;
    private TMP_Text label;
    private Slider slider;

    public EVolumeType VolumeType => volumeType;

    private void Awake()
    {
        label = GetComponentInChildren<TMP_Text>();
        slider = GetComponentInChildren<Slider>();

        if (label != null)
        {
            label.text = volumeType.ToString();
        }
    }

    private void OnEnable()
    {
        AudioManager.OnVolumeSettingsChanged += SyncSliderWithVolume;

        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnValueChanged);
            // UI가 활성화될 때 현재 값으로 한 번 동기화
            SyncSliderWithVolume();
        }
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위해 구독 해제
        AudioManager.OnVolumeSettingsChanged -= SyncSliderWithVolume;

        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    // 현재 볼륨 값을 AudioManager로부터 가져와 슬라이더에 반영
    public void SyncSliderWithVolume()
    {
        if (slider == null || AudioManager.Instance == null)
        {
            return;
        }

        // 이 함수가 onValueChanged 이벤트를 다시 발생시키지 않도록 리스너를 잠시 제거/추가
        slider.onValueChanged.RemoveListener(OnValueChanged);
        slider.value = AudioManager.Instance.GetVolume(volumeType);
        slider.onValueChanged.AddListener(OnValueChanged);
    }

    // 슬라이더 값이 변경될 때 호출되는 이벤트 리스너
    public void OnValueChanged(float value)
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        // 이 로그가 콘솔에 찍히는지 확인 (슬라이더를 움직일 때마다)
        Debug.Log($"[VolumeSlider] {volumeType} Slider Value Changed: {value}");

        AudioManager.Instance.SetVolume(volumeType, value);
    }
}
