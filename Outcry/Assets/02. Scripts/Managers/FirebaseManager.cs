using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions; // ContinueWithOnMainThread를 위해 필수
using System;
using System.Collections;

public class FirebaseManager : Singleton<FirebaseManager>
{
    // 파이어베이스가 제공하는 모든 기능에 접근 가능하도록 하는 클래스
    private FirebaseApp app;

    // Analytics 초기화 완료 여부 플래그
    private bool bIsAnalyticsInit = false;

    private void Awake()
    {
        StartCoroutine(InitFirebaseServiceCo());
    }

    /// Firebase 서비스 초기화 확인
    public bool IsInit()
    {
        // 현재 Analytics만 사용하므로 이 플래그 하나만 확인
        return bIsAnalyticsInit;
    }

    // Firebase 서비스 초기화를 진행하는 코루틴
    private IEnumerator InitFirebaseServiceCo()
    {
        // Firebase 종속성 확인 및 수정
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("FirebaseApp initialization success.");

                // FirebaseApp 인스턴스 초기화
                app = FirebaseApp.DefaultInstance;

                // Analytics 서비스 초기화 시작
                InitAnalytics();
            }
            else
            {
                Debug.LogError($"FirebaseApp initialization failed. DependencyStatus:{dependencyStatus}");
            }
        });

        float elapsedTime = 0f;
        const float INIT_TIMEOUT = 10.0f; // 초기화 타임아웃 10초로 설정

        // IsInit()가 true가 되거나, 타임아웃 시간이 될 때까지 대기
        while (!IsInit() && elapsedTime < INIT_TIMEOUT)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 루프 종료 후 최종 결과 확인
        if (IsInit())
        {
            Debug.Log($"{GetType()} initialization success.");
        }
        else
        {
            Debug.LogError($"{GetType()} initialization failed within the timeout period.");
        }
    }

    #region ANALYTICS

    /// <summary>
    /// Firebase Analytics 서비스 초기화
    /// </summary>
    private void InitAnalytics()
    {
        // 애널리틱스 데이터 수집 활성화
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        bIsAnalyticsInit = true; // 초기화 완료 플래그 설정
        Debug.Log("Firebase Analytics initialized.");
    }

    /// <summary>
    /// 스테이지 시작 시 호출
    /// </summary>
    /// <param name="stageName">스테이지 이름 (예: "Stage-1", "Boss-Dungeon")</param>
    public void LogStageStart(string stageName)
    {
        if (!IsInit())
        {
            Debug.LogWarning("Firebase is not initialized. LogStageStart event was ignored.");
            return;
        }

        FirebaseAnalytics.LogEvent("stage_start", new Parameter("stage_name", stageName));
        Debug.Log($"Analytics: Stage Start logged - {stageName}");
    }

    /// <summary>
    /// 스테이지 결과(성공/실패) 기록
    /// </summary>
    /// <param name="stageName">스테이지 이름</param>
    /// <param name="isClear">클리어 성공 여부</param>
    /// <param name="playTime">플레이한 시간(초)</param>
    public void LogStageResult(string stageName, bool isClear, float playTime)
    {
        if (!IsInit())
        {
            Debug.LogWarning("Firebase is not initialized. LogStageResult event was ignored.");
            return;
        }

        string result = isClear ? "clear" : "fail";
        Parameter[] stageParams = {
            new Parameter("stage_name", stageName),
            new Parameter("result", result),
            new Parameter("play_time", playTime)
        };

        FirebaseAnalytics.LogEvent("stage_result", stageParams);
        Debug.Log($"Analytics: Stage Result logged - {stageName}, Result: {result}, Time: {playTime}");
    }

    /// <summary>
    /// 스킬 사용 기록
    /// </summary>
    /// <param name="skillName">스킬 이름 (예: "Fireball", "Heal")</param>
    public void LogSkillUsage(string skillName)
    {
        if (!IsInit())
        {
            Debug.LogWarning("Firebase is not initialized. LogSkillUsage event was ignored.");
            return;
        }

        FirebaseAnalytics.LogEvent("skill_use", new Parameter("skill_name", skillName));
        Debug.Log($"Analytics: Skill Usage logged - {skillName}");
    }
    #endregion
}