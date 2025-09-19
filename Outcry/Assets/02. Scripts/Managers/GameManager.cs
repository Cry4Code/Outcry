using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

public class UserData
{
    public string Nickname;
    public List<int> ClearedBossIds; // 클리어한 보스 ID 목록
    public bool IsTutorialCleared;

    public UserData(string nickname)
    {
        Nickname = nickname;
        ClearedBossIds = new List<int>();
        IsTutorialCleared = false;
    }
}

// 게임 전체의 상태를 나타내는 열거형
public enum EGameState
{
    Initializing, // 초기화 중
    MainMenu,     // 메인 메뉴 (로그인 전)
    Lobby,        // 로비 (캐릭터/보스 선택)
    InGame,       // 게임 플레이 중 (보스 전투)
    LoadingScene  // 씬 로딩 중
}

public class GameManager : Singleton<GameManager>
{
    public EGameState CurrentGameState { get; private set; }
    public UserData CurrentUserData { get; private set; }

    // StageManager에 전달할 현재 선택된 스테이지의 모든 정보
    public StageData SelectedStageData { get; private set; }
    public string CurrentUserUID { get; private set; }

    private void Awake()
    {
        InitializeCoreSystems();
    }

    private void Start()
    {
        // 오디오 테스트
        //AudioManager.Instance.PlayBGM((int)SoundEnums.EBGM.Title);
        //AudioManager.Instance.PlaySFX((int)SoundEnums.ESFX.Parry);

        // 스테이지 테스트
        StartBossBattle(SelectedStageData.Monster_id[0]); // TEST
    }

    // 게임 시작 시 단 한번만 실행되어야 하는 초기화 로직
    private void InitializeCoreSystems()
    {
        CurrentGameState = EGameState.Initializing;
        // Application.targetFrameRate = 60;

        // TODO: ResourceManager, AudioManager 등 다른 핵심 시스템 초기화 호출
        // StartCoroutine(WaitForFirebaseAndInitialize());

        Debug.Log("GameManager Initialized.");
    }

    private void OnEnable()
    {
        // StageManager가 보스 처치 시 발생시키는 이벤트를 구독
        StageManager.OnBossDefeated += HandleBossDefeated;
    }

    private void OnDisable()
    {
        // 구독 해제
        StageManager.OnBossDefeated -= HandleBossDefeated;
    }

    public void CreateNewGame(string nickname)
    {
        CurrentUserData = new UserData(nickname);
        // TODO: 새 데이터 저장 요청
        SaveGame(); 

        // 튜토리얼용 보스 전투 시작?
        StartBossBattle(0); // 튜토리얼 보스 ID를 0으로 약속
    }

    public void LoadGame(UserData data)
    {
        CurrentUserData = data;
        GoToLobby();
    }

    // 로비 씬으로 이동
    public void GoToLobby()
    {
        CurrentGameState = EGameState.Lobby;
        //SceneManager.LoadScene("LobbyScene");
    }

    /// <summary>
    /// 로비에서 보스 선택 시 호출.
    /// bossId를 StageData로 변환하여 전투 씬 로드
    /// </summary>
    public async void StartBossBattle(int bossId)
    {
        CurrentGameState = EGameState.LoadingScene;

        // 데이터 테이블에서 로드할 스테이지 데이터 주소 가져옴
        string stageDataAddress = $"StageData_Boss_{bossId}"; // 예시: "StageData_Boss_101"

        if (SelectedStageData == null)
        {
            Debug.LogError($"{stageDataAddress} 로드에 실패했습니다. 전투를 시작할 수 없습니다.");
            GoToLobby(); // 로비로 복귀
            return;
        }

        // StageData 로드가 완료되면 전투 씬으로 이동
        CurrentGameState = EGameState.InGame;
        // SceneLoadManager.Instance.LoadScene("BossBattleScene");
        SceneManager.LoadScene("TestStageScene"); // 임시
    }

    // StageManager가 보스 처치 이벤트를 발생시키면 실행
    private void HandleBossDefeated(int bossId)
    {
        if (CurrentUserData == null) return;

        Debug.Log($"보스 ID: {bossId} 처치! 유저 데이터 업데이트 및 저장.");

        // 클리어한 보스 목록에 추가 (중복 방지)
        if (!CurrentUserData.ClearedBossIds.Contains(bossId))
        {
            CurrentUserData.ClearedBossIds.Add(bossId);
        }

        // 튜토리얼 클리어 처리
        if (bossId == 0 && !CurrentUserData.IsTutorialCleared)
        {
            CurrentUserData.IsTutorialCleared = true;
        }

        // 변경된 데이터 저장
        SaveGame();
    }

    // 현재 유저 데이터를 저장하는 기능
    public void SaveGame()
    {
        if (CurrentUserData == null || string.IsNullOrEmpty(CurrentUserUID))
        {
            return;
        }

        Debug.Log("게임을 저장합니다...");
        // TODO: FirebaseManager에 데이터 저장 요청
        // FirebaseManager.Instance.SaveUserData(CurrentUserUID, CurrentUserData);?
    }
}
