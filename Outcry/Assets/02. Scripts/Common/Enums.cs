
namespace SoundEnums
{
    public enum EVolumeType
    {
        Master,
        BGM,
        SFX
    }

    public enum EBGM
    {
        None,
        Title = 104000,
        InGame = 104001,
    }

    public enum ESFX
    {
        None,
        Sword = 104101,
        Parry = 104102,
        Fury = 104103,
    }
}

namespace StageEnums
{
    public enum EStageState
    {
        Ready,      // 전투 시작 연출
        InProgress, // 전투 진행 중
        Paused,     // 일시정지
        Finished    // 전투 종료
    }

    public enum EBossType
    {
        GoblinKing = 102001,
        Temp1 = 102002,
        Temp2 = 102003,
    }
}