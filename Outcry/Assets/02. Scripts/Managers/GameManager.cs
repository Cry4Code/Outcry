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

public class GameManager : Singleton<GameManager>, IStageDataProvider
{
    public EGameState CurrentGameState { get; private set; }
    public UserData CurrentUserData { get; private set; }
    public string CurrentUserUID { get; private set; }
    public SceneLoadPackage NextLoadPackage { get; private set; }

    private StageData currentStageData;
    public StageData GetStageData() => currentStageData;

    protected override void Awake()
    {
        base.Awake();

        InitializeCoreSystems();
    }

    private void Start()
    {
        // 오디오 테스트
        //AudioManager.Instance.PlayBGM((int)SoundEnums.EBGM.Title);
        //AudioManager.Instance.PlaySFX((int)SoundEnums.ESFX.Parry);

        // 스테이지 테스트
        //StartStage(106001); // GoblinKing TEST
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    // 게임 시작 시 단 한번만 실행되어야 하는 초기화 로직
    private void InitializeCoreSystems()
    {
        CurrentGameState = EGameState.Initializing;
        // Application.targetFrameRate = 60;

        DataTableManager.Instance.LoadCollectionData<StageDataTable>();
        DataTableManager.Instance.LoadCollectionData<SoundDataTable>();

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
        StartStage(0); // 튜토리얼 보스 ID를 0으로 약속
    }

    public void LoadGame(UserData data)
    {
        CurrentUserData = data;
        GoToLobby();
    }

    // 로비 씬으로 이동
    public void GoToLobby()
    {
        CurrentGameState = EGameState.LoadingScene;

        // 로비 이동을 위한 간단한 명세서 생성(미리 로드할 리소스 없음)
        var package = new SceneLoadPackage("LobbyScene");

        // 생성된 명세서 저장
        NextLoadPackage = package;

        // TODO: LoadingScene 로드
    }

    /// <summary>
    /// 로비에서 보스 선택 시 호출.
    /// bossId를 StageData로 변환하여 전투 씬 로드
    /// </summary>
    public void StartStage(int stageId)
    {
        currentStageData = DataTableManager.Instance.GetCollectionDataById<StageData>(stageId);
        if (currentStageData == null)
        {
            Debug.LogError($"ID: {stageId}에 해당하는 StageData를 찾을 수 없습니다. 로비로 돌아갑니다.");
            GoToLobby();
            return;
        }

        CurrentGameState = EGameState.LoadingScene;

        // 스테이지 시작을 위한 데이터 설정
        var package = new SceneLoadPackage("TestStageScene");
        package.AdditiveSceneNames.Add("StageManagerScene");
        package.ResourceAddressesToLoad.Add(currentStageData.Map_path);
        package.ResourceAddressesToLoad.Add(currentStageData.Boss_path);
        // TODO: 플레이어 프리팹 주소도 추가해야 함

        // 스테이지 데이터 저장
        NextLoadPackage = package;

        SceneLoadManager.Instance.LoadScene("LoadingScene");
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
