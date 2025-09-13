using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Analytics;
using Firebase.Extensions; // ContinueWithOnMainThread를 위해 필수
using System;
using System.Collections;
using System.Threading.Tasks;

public class FirebaseManager : Singleton<FirebaseManager>
{
    // 파이어베이스가 제공하는 모든 기능에 접근 가능하도록 하는 클래스
    private FirebaseApp app;

    // 인증 관련
    private FirebaseAuth auth;
    private FirebaseUser user;
    private bool bIsAuthInit = false;

    // 애널리틱스 관련
    private bool bIsAnalyticsInit = false;

    private void Awake()
    {
        StartCoroutine(InitFirebaseServiceCo());
    }

    /// Firebase 서비스 초기화 확인
    public bool IsInit()
    {
        return bIsAuthInit && bIsAnalyticsInit;
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

                // 파이어베이스 서비스 초기화
                InitAuth();
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

    #region AUTH
    /// <summary>
    /// Firebase Auth 서비스 초기화
    /// </summary>
    private void InitAuth()
    {
        auth = FirebaseAuth.DefaultInstance;
        // 사용자의 로그인 상태가 바뀔 때마다(로그인, 로그아웃) OnAuthStateChanged 함수를 호출하도록 등록
        auth.StateChanged += OnAuthStateChanged;
        bIsAuthInit = true;
        Debug.Log("Firebase Auth initialized.");
    }

    /// <summary>
    /// 로그인 상태 변경 시 호출되는 이벤트 핸들러
    /// </summary>
    private void OnAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = (auth.CurrentUser != null);
            if (signedIn)
            {
                user = auth.CurrentUser;
                Debug.Log($"Firebase User Signed In: {user.UserId}");

                // 사용자의 로그인 유형에 따라 꼬리표를 붙임
                if (user.IsAnonymous)
                {
                    FirebaseAnalytics.SetUserProperty("login_type", "anonymous");
                }
                else
                {
                    FirebaseAnalytics.SetUserProperty("login_type", "email");
                }

                // TODO: 로그인 성공 후 필요한 작업 (예: 유저 데이터 로드)을 여기서 시작?
            }
            else
            {
                Debug.Log("Firebase User Signed Out.");
                user = null;
            }
        }
    }

    /// <summary>
    /// 익명으로 Firebase에 로그인합니다. (게스트 로그인)
    /// </summary>
    /// <returns>로그인 성공 여부 Task</returns>
    public async Task<bool> SignInAnonymouslyAsync()
    {
        if (!IsInit() || user != null)
        {
            Debug.LogWarning("Firebase not initialized or user already signed in.");
            return false;
        }

        try
        {
            // 익명 로그인 시도
            // AuthResult 객체로 결과를 받음
            AuthResult result = await auth.SignInAnonymouslyAsync();
            // 결과 객체 안의 User 프로퍼티 사용
            FirebaseUser newUser = result.User;
            Debug.Log($"Anonymous sign-in successful. User ID: {newUser.UserId}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Anonymous sign-in failed with exception: {e}");
            return false;
        }
    }

    /// <summary>
    /// 이메일과 비밀번호로 새 계정 생성(회원가입)
    /// </summary>
    public async Task<bool> SignUpWithEmailAsync(string email, string password)
    {
        if (!IsInit())
        {
            Debug.LogWarning("Firebase not initialized.");
            return false;
        }

        try
        {
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = result.User;
            Debug.Log($"Email sign-up successful. User ID: {newUser.UserId}");
            return true;
        }
        catch (FirebaseException e)
        {
            // Firebase 관련 에러 처리
            AuthError errorCode = (AuthError)e.ErrorCode;
            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    Debug.LogError("Email already in use.");
                    break;
                case AuthError.WeakPassword:
                    Debug.LogError("Password is too weak.");
                    break;
                default:
                    Debug.LogError($"Sign-up failed with Firebase exception: {e}");
                    break;
            }
            return false;
        }
        catch (Exception e)
        {
            // 기타 에러 처리
            Debug.LogError($"Sign-up failed with exception: {e}");
            return false;
        }
    }

    /// <summary>
    /// 이메일과 비밀번호로 로그인
    /// </summary>
    public async Task<bool> SignInWithEmailAsync(string email, string password)
    {
        if (!IsInit())
        {
            Debug.LogWarning("Firebase not initialized.");
            return false;
        }

        try
        {
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser signInuser = result.User;
            Debug.Log($"Email sign-in successful. User ID: {signInuser.UserId}");
            return true;
        }
        catch (FirebaseException e)
        {
            AuthError errorCode = (AuthError)e.ErrorCode;
            switch (errorCode)
            {
                case AuthError.WrongPassword:
                    Debug.LogError("Wrong password.");
                    break;
                case AuthError.UserNotFound:
                    Debug.LogError("User not found.");
                    break;
                default:
                    Debug.LogError($"Sign-in failed with Firebase exception: {e}");
                    break;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Sign-in failed with exception: {e}");
            return false;
        }
    }

    /// <summary>
    /// 현재 사용자 로그아웃
    /// </summary>
    public void SignOut()
    {
        if (auth.CurrentUser != null)
        {
            Debug.Log("Signing out current user.");

            auth.SignOut();
        }
    }

    /// <summary>
    /// 현재 로그인된 익명 계정을 영구적인 이메일/비밀번호 계정으로 연동(업그레이드)
    /// </summary>
    public async Task<bool> LinkEmailToCurrentUserAsync(string email, string password)
    {
        // 반드시 익명 유저가 로그인한 상태여야 함
        if (user == null || !user.IsAnonymous)
        {
            Debug.LogError("No anonymous user is currently signed in to link.");
            return false;
        }

        try
        {
            // 이메일/비밀번호로 자격 증명(Credential) 생성
            Credential credential = EmailAuthProvider.GetCredential(email, password);

            // 현재 익명 유저에게 새 자격 증명을 연결
            await user.LinkWithCredentialAsync(credential);

            Debug.Log("Anonymous account successfully upgraded to an Email account.");
            return true;
        }
        catch (FirebaseException e)
        {
            AuthError errorCode = (AuthError)e.ErrorCode;
            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    Debug.LogError("This email is already associated with another account.");
                    break;
                case AuthError.CredentialAlreadyInUse:
                    Debug.LogError("This credential is already linked to another user.");
                    break;
                default:
                    Debug.LogError($"Account linking failed with Firebase exception: {e}");
                    break;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Account linking failed with exception: {e}");
            return false;
        }
    }
    #endregion

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

    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= OnAuthStateChanged;
            auth = null;
        }
    }
}