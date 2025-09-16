using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private VolumeType volumeType;
    private TMP_Text label;
    private Slider slider;

    private void Awake()
    {
        label = GetComponentInChildren<TMP_Text>();
        slider = GetComponentInChildren<Slider>();

        if (label != null)
        {
            label.text = volumeType.ToString();
        }

        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnValueChanged);
            SyncSliderWithVolume();
        }
    }

    public void SyncSliderWithVolume()
    {
        if (slider == null)
        {
            return;
        }

        slider.value = AudioManager.Instance.GetVolume(volumeType);
    }

    public void OnValueChanged(float value)
    {
        AudioManager.Instance.SetVolume(volumeType, value);
    }

    public void SetVolumeType(VolumeType type)
    {
        volumeType = type;

        if (label != null)
        {
            label.text = type.ToString();
        }
    }
}
