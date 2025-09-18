using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsUI : MonoBehaviour
{
    [SerializeField] private VolumeSlider[] volumeSliders;
    [SerializeField] private Button ExitBtn;

    private void Awake()
    {
        if (ExitBtn != null)
        {
            ExitBtn.onClick.AddListener(OnExitButtonClicked);
        }
    }

    private void OnExitButtonClicked()
    {
        gameObject.SetActive(false); // test
        //UIManager.Instance.Hide<VolumeSettingsUI>();
    }
}
