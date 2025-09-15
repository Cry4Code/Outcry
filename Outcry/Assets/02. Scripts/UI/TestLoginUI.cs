using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Analytics;

public class TestLoginUI : UIBase
{
    [Header("UI Panels")]
    public GameObject startPanel; // "게스트 로그인", "이메일 로그인" 버튼이 있는 패널
    public GameObject emailLoginPanel; // 이메일, 비번 입력창과 버튼이 있는 패널

    [Header("Email Login Panel Fields")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button signUpButton;
    public Button signInButton;
    public Button guestLoginButton;
    public Button emailLoginButton; // 이메일 로그인 패널을 여는 버튼
    public Button SignOutButton;
    public Button LinkButton;

    private string email;
    private string password;

    private void Start()
    {
        // 버튼에 기능 연결
        emailLoginButton.onClick.AddListener(OnEmailLoginButtonClicked);
        guestLoginButton.onClick.AddListener(OnGuestLoginClicked);
        SignOutButton.onClick.AddListener(() => FirebaseManager.Instance.SignOut());
        signUpButton.onClick.AddListener(OnSignUpClicked);
        signInButton.onClick.AddListener(OnSignInClicked);
        LinkButton.onClick.AddListener(OnLinkAccountClicked);

        ShowStartPanel();
    }

    private void ShowStartPanel()
    {
        startPanel.SetActive(true);
        emailLoginPanel.SetActive(false);
    }

    private void ShowEmailLoginPanel()
    {
        startPanel.SetActive(false);
        emailLoginPanel.SetActive(true);
    }

    // 게스트 로그인 버튼 클릭 시
    private async void OnGuestLoginClicked()
    {
        Debug.Log("Attempting Guest Login...");
        bool success = await FirebaseManager.Instance.SignInAnonymouslyAsync();
        if (success)
        {
            Debug.Log("Guest Login Success!");

            FirebaseAnalytics.LogEvent("login_anonymous"); // 게스트 로그인 이벤트 로깅

            ShowEmailLoginPanel();
            // SceneLoader.Instance.LoadScene(ESceneType.Lobby);
        }
    }

    private void OnEmailLoginButtonClicked()
    {
        ShowEmailLoginPanel();
    }

    // 회원가입 버튼 클릭 시
    private async void OnSignUpClicked()
    {
        Debug.Log("Attempting Sign Up...");
        bool success = await FirebaseManager.Instance.SignUpWithEmailAsync(emailInput.text, passwordInput.text);
        if (success)
        {
            Debug.Log("Sign Up Success!");
            // SceneLoader.Instance.LoadScene(ESceneType.Lobby);
        }
    }

    // 로그인 버튼 클릭 시
    private async void OnSignInClicked()
    {
        Debug.Log("Attempting Sign In...");
        bool success = await FirebaseManager.Instance.SignInWithEmailAsync(emailInput.text, passwordInput.text);
        if (success)
        {
            Debug.Log("Sign In Success!");

            FirebaseAnalytics.LogEvent("login_email"); // 이메일 로그인 이벤트 로깅

            // SceneLoader.Instance.LoadScene(ESceneType.Lobby);
        }
    }

    // 계정 연동 UI 예시(게임 내 설정(Options) 메뉴에 있을 기능?)
    public async void OnLinkAccountClicked()
    {
        email = emailInput.text;
        password = passwordInput.text;

        // 비밀번호 유효성 검사
        if (password.Length < 6)
        {
            Debug.LogError("Password must be at least 6 characters long.");
            // TODO: 사용자에게 보여줄 UI 메시지 띄우기
            // 예: 비밀번호는 6자 이상으로 입력해주세요
            return;
        }

        // 유저가 이메일/비번 입력 후 계정 저장 버튼을 눌렀다고 가정
        bool success = await FirebaseManager.Instance.LinkEmailToCurrentUserAsync(email, password);
        if (success)
        {
            Debug.Log("Account linked successfully! Your progress is now saved.");
            // UI에 "계정 연동 완료" 메시지 표시
        }
    }
}