using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static Paths;

public class GameManager : Singleton<GameManager>
{
    private async void Start()
    {
        await Init();
    }

    private async Task Init()
    {
        // Firebase 초기화를 기다리는 코루틴 시작
        //StartCoroutine(WaitForFirebaseAndInitialize());

        AudioManager.Instance.PlayBGM("Title");

        StartCoroutine(ChangeBGMCoroutine());

        Task playerSFX = AudioManager.Instance.PreloadSFX("Fury", "Sword");
        await Task.WhenAll(playerSFX);

        AudioManager.Instance.PlaySFX("Fury");
    }

    private void Update()
    {
        // 스페이스바를 누를 때마다
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("스페이스바 입력 감지! SFX를 재생합니다.");

            AudioManager.Instance.PlaySFX("Sword");
        }
    }

    private IEnumerator ChangeBGMCoroutine()
    {
        yield return new WaitForSeconds(10f); // 10초 대기
        AudioManager.Instance.PlayBGM("InGame"); // BGM 변경
    }

    private IEnumerator WaitForFirebaseAndInitialize()
    {
        // FirebaseManager.Instance.IsInit()가 true가 될 때까지 매 프레임 기다림
        while (!FirebaseManager.Instance.IsInit())
        {
            yield return null;
        }

        // 이 시점은 Firebase 초기화가 100% 완료되었음이 보장됨
        InitializeGame();
    }

    // 게임의 실제 로직을 처리하는 메서드
    private void InitializeGame()
    {
        // Firebase 초기화가 완료되었음이 보장되는 시점
        Debug.Log("GameManager: Firebase is confirmed to be initialized. Starting game logic.");

        //FirebaseManager.Instance.LogStageStart("Test");
    }
}
