using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestStage1Button : MonoBehaviour
{
    [SerializeField] private Button stage1Btn;

    private void Start()
    {
        stage1Btn.onClick.AddListener(OnStage1ButtonClicked);
    }

    private void OnStage1ButtonClicked()
    {
        GameManager.Instance.StartStage(106001);
    }
}
