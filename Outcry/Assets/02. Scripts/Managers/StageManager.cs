using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public enum EStageState
{
    Loading,    // 리소스 로딩
    Ready,      // 전투 시작 연출
    InProgress, // 전투 진행 중
    Paused,     // 일시정지
    Finished    // 전투 종료
}

public class StageManager : Singleton<StageManager>
{
    public EStageState CurrentState { get; private set; }

    [Header("스폰 위치")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform bossSpawnPoint;

    // UI Manager를 통해 그려줄 예정
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text timerText;
    [SerializeField] private Text resultText;

    // 내부 변수
    private StageData currentStageData;
    private float stageTimer;

    // 로드된 프리팹 원본
    private GameObject mapPrefab;
    private GameObject playerPrefab;
    private GameObject bossPrefab;

    // 생성된 플레이어, 몬스터 인스턴스(조작 제어 및 AI 용)?

    // 이 스테이지에서 로드한 리소스 주소 목록(언로드용)
    private List<string> loadedAssetKeys = new List<string>();

    // 이벤트
    public static event Action<int> OnBossDefeated; // 보스 처치 시 호출

    private async void Start()
    {
        // 상태 변경 및 데이터 로드
        CurrentState = EStageState.Loading;
        // GameManager 등에서 현재 스테이지 데이터 받아옴
        // _currentStageData = GameManager.Instance.GetCurrentStageData();

        // 테스트용 임시 데이터 로드(데이터테이블에서 받아옴)
        //currentStageData = await ResourceManager.Instance.LoadAssetAddressableAsync<StageData>("");
        if (currentStageData == null)
        {
            Debug.LogError("StageData 로드 실패!");
            return;
        }
        loadedAssetKeys.Add(""); // StageData도 언로드 대상

        // 스테이지에 필요한 모든 에셋(프리팹) 로드
        //await LoadStageAssets();

        // 모든 로딩이 끝나면 스테이지 시작
        StartCoroutine(StageFlowRoutine());
    }

    private void Update()
    {
        if (CurrentState != EStageState.InProgress) return;

        UpdateTimer();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void OnEnable()
    {
        // 이벤트 구독?
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제?

    }

    private void OnDestroy()
    {
        // 씬 전환 시 이 스테이지에서 로드했던 모든 리소스를 해제
        foreach (var key in loadedAssetKeys)
        {
            ResourceManager.Instance.UnloadAddressableAsset(key);
        }
        loadedAssetKeys.Clear();
    }

    #region 스테이지 흐름 코루틴
    // 스테이지의 전체적인 흐름을 관리하는 메인 코루틴
    private IEnumerator StageFlowRoutine()
    {
        // 스폰 및 등장 연출
        yield return StartCoroutine(IntroRoutine());

        // 전투 시작
        CurrentState = EStageState.InProgress;
        //stageTimer = currentStageData.timeLimit;
        Debug.Log("전투 시작!");

        // 이벤트(플레이어/보스 사망)에 의해 다음 단계(Victory/Defeat)로 넘어감?
    }

    // 스폰 및 등장 연출 코루틴
    private IEnumerator IntroRoutine()
    {
        CurrentState = EStageState.Ready;

        // 맵 생성
        if (mapPrefab != null) Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);

        // 플레이어, 보스 스폰
        GameObject playerGo = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        GameObject bossGo = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        // TODO: 스테이지 시작 UI?

        // TODO: 등장 연출 동안 조작 비활성화
        //_playerInstance.SetControllable(false);
        //_currentBoss.SetActiveAI(false);

        Debug.Log("보스 등장 연출...");
        yield return new WaitForSeconds(3.0f);

        // TODO: 승리 및 패배 시 플레이어 몬스터 조작 비활성화
        //_playerInstance.SetControllable(true);
        //_currentBoss.SetActiveAI(true);
    }

    // 승리 처리 코루틴
    private IEnumerator VictoryRoutine()
    {
        CurrentState = EStageState.Finished;
        //_playerInstance.SetControllable(false);

        if (currentStageData != null)
        {
            // StageData에 보스 ID
            //OnBossDefeated?.Invoke(currentStageData.bossId);
        }

        Debug.Log("스테이지 클리어!");
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(2.0f);
        Time.timeScale = 1.0f;

        resultText.text = "VICTORY";
        resultPanel.SetActive(true);

        yield return new WaitForSeconds(5.0f);

        // TODO: 로비 씬으로 이동
    }

    // 패배 처리 코루틴
    private IEnumerator DefeatRoutine()
    {
        CurrentState = EStageState.Finished;
        //if (_currentBoss != null)
        //{
        //    _currentBoss.SetActiveAI(false);
        //}

        Debug.Log("스테이지 실패!");
        yield return new WaitForSeconds(2.0f);

        resultText.text = "DEFEAT";
        resultPanel.SetActive(true);

        yield return new WaitForSeconds(5.0f);

        // TODO: 로비 씬으로 이동?
    }
    #endregion

    #region 에셋, 이벤트 핸들러
    // StageData에 명시된 모든 프리팹을 로드
    //private async Task LoadStageAssets()
    //{
    //    // 로드할 에셋들의 키(주소)를 리스트에 추가
    //    var keysToLoad = new List<string>
    //    {
    //        currentStageData.mapAddress,
    //        currentStageData.playerAddress,
    //        currentStageData.bossAddress
    //    };
    //    loadedAssetKeys.AddRange(keysToLoad);

    //    // 병렬 로딩을 위한 Task 리스트 생성
    //    var loadTasks = new List<Task>();
    //    loadTasks.Add(ResourceManager.Instance.LoadAssetAddressableAsync<GameObject>(currentStageData.mapAddress).ContinueWith(task => mapPrefab = task.Result));
    //    loadTasks.Add(ResourceManager.Instance.LoadAssetAddressableAsync<GameObject>(currentStageData.playerAddress).ContinueWith(task => playerPrefab = task.Result));
    //    loadTasks.Add(ResourceManager.Instance.LoadAssetAddressableAsync<GameObject>(currentStageData.bossAddress).ContinueWith(task => bossPrefab = task.Result));

    //    // 모든 로딩 작업이 끝날 때까지 대기
    //    await Task.WhenAll(loadTasks);
    //}

    // 플레이어 사망 이벤트가 방송되면 실행될 핸들러
    private void OnPlayerDiedHandler()
    {
        if (CurrentState == EStageState.InProgress)
        {
            StartCoroutine(DefeatRoutine());
        }
    }

    // 보스 사망 이벤트가 방송되면 실행될 핸들러
    private void OnBossDiedHandler()
    {
        if (CurrentState == EStageState.InProgress)
        {
            StartCoroutine(VictoryRoutine());
        }
    }

    #endregion

    #region 게임 플레이 로직
    private void UpdateTimer()
    {
        stageTimer -= Time.deltaTime;
        if (timerText != null)
        {
            timerText.text = $"TIME: {Mathf.Max(0, stageTimer):F0}";
        }

        if (stageTimer <= 0)
        {
            // 타임오버 시 플레이어 사망 처리
            OnPlayerDiedHandler();
        }
    }

    public void TogglePause()
    {
        if (CurrentState == EStageState.InProgress)
        {
            CurrentState = EStageState.Paused;
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else if (CurrentState == EStageState.Paused)
        {
            CurrentState = EStageState.InProgress;
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }
    #endregion
}
