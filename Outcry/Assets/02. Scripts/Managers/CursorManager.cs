using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : Singleton<CursorManager>
{
    #region 커서들

    [SerializeField] private RectTransform uiCursor; // UI용
    [SerializeField] private Transform inGameCursor; // 인게임용

    #endregion

    private Camera mainCam;
    public bool IsInGame { get; set; } = false;

    private PlayerInputs playerInputs;
    public Vector3 mousePosition;

    private void Awake()
    {
        mainCam = Camera.main;
        Cursor.visible = false;

        if (uiCursor == null || inGameCursor == null)
        {
            Debug.LogWarning($"커서가 지정되지 않음.");
        }

        SetInGame(true); // 테스트용
    }

    private void Start()
    {
        PlayerManager.Instance.player.Mover.Inputs.Player.Look.performed += OnLook;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.player.Mover.Inputs.Player.Look.performed -= OnLook;
    }

    /// <summary>
    /// 커서 토글용
    /// </summary>
    /// <param name="isInGame">전투중이면 True 부르면 됨</param>
    public void SetInGame(bool isInGame)
    {
        this.IsInGame = isInGame;
        uiCursor.gameObject.SetActive(!isInGame);
        inGameCursor.gameObject.SetActive(isInGame);
    }


    /// <summary>
    /// 마우스 이동 시 자동 실행 됨
    /// </summary>
    /// <param name="context"></param>
    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mousePos = context.ReadValue<Vector2>();

        // 전투 중
        if (IsInGame)
        {
            mousePosition = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
            mousePosition.z = 0f;
            inGameCursor.position = mousePosition;
        }

        // UI 중
        else
            uiCursor.position = mousePos;
    }

}
