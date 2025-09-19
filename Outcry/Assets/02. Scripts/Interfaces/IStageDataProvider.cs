/// <summary>
/// StageManager에게 현재 스테이지 데이터를 제공하는 역할을 정의하는 인터페이스.
/// GameManager가 이 인터페이스를 구현하여 StageManager와의 직접적인 의존성을 낮춘다.
/// </summary>
public interface IStageDataProvider
{
    /// <summary>
    /// 현재 진행해야 할 스테이지의 데이터 반환
    /// </summary>
    StageData GetStageData();
}
