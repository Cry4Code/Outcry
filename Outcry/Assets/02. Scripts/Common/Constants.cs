using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Paths
{
    public static class Prefabs { public const string UI = "Prefabs/UI/";}
}

public static class EventBusKey
{
    public const string ChangeHealth = "ChangeHealth";
    public const string ChangeStamina = "ChangeStamina";
}

public static class AnimatorStrings
{
    public static class MonsterParameter
    {
        public const string Running = "Running";
        public const string NormalAttack = "NormalAttack";
        public const string Stomp = "Stomp";
    }

    public static class MonsterAnimation
    {
        public const string Idle = "Idle";
        public const string Run = "Run";
        public const string Stomp = "Stomp";
    }
}
