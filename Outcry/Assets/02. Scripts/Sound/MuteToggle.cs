using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SoundEnums;

public class MuteToggle : MonoBehaviour
{
    // 인스펙터에서 수동으로 타입을 지정할 수도 있고 자동으로 찾게 할 수도 있음
    [SerializeField] private EVolumeType volumeType;
    [Tooltip("체크하면 부모의 VolumeSlider에서 타입을 자동으로 찾아 설정합니다.")]
    [SerializeField] private bool autoDetectTypeFromParent = true;

    [SerializeField] private GameObject onImage;  // '켜짐' 상태일 때 활성화될 이미지
    [SerializeField] private GameObject offImage; // '꺼짐' 상태일 때 활성화될 이미지

    private Button button;

    private void Awake()
    {
        if (autoDetectTypeFromParent)
        {
            VolumeSlider parentSlider = GetComponentInParent<VolumeSlider>();
            if (parentSlider != null)
            {
                // 찾았다면 부모의 VolumeType으로 자신의 타입 설정
                volumeType = parentSlider.VolumeType;
            }
            else
            {
                Debug.LogWarning($"'{gameObject.name}'의 부모에서 VolumeSlider를 찾을 수 없습니다. VolumeType이 자동으로 설정되지 않았습니다.", this);
            }
        }

        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        // 이벤트 구독
        AudioManager.OnMuteStateChanged += HandleMuteStateChanged;
        // 현재 상태에 맞게 UI 초기화
        UpdateVisuals(AudioManager.Instance.GetMuteState(volumeType));
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        AudioManager.OnMuteStateChanged -= HandleMuteStateChanged;
    }

    // 버튼이 클릭되었을 때 AudioManager에 토글 요청
    private void OnClick()
    {
        AudioManager.Instance.ToggleMute(volumeType);

        // 버튼을 클릭한 후 이 버튼의 선택된 상태(포커스)를 즉시 해제
        EventSystem.current.SetSelectedGameObject(null);
    }

    // AudioManager의 음소거 상태가 변경되었다는 이벤트가 왔을 때 호출됨
    private void HandleMuteStateChanged(EVolumeType type, bool isMute)
    {
        // 이 토글 버튼이 해당되는 타입의 이벤트일 경우에만 UI 업데이트
        if (type == this.volumeType)
        {
            UpdateVisuals(isMute);
        }
    }

    // UI 이미지를 현재 상태에 맞게 업데이트
    private void UpdateVisuals(bool isMute)
    {
        if (onImage != null)
        {
            onImage.SetActive(!isMute);
        }

        if (offImage != null)
        {
            offImage.SetActive(isMute);
        }
    }
}
