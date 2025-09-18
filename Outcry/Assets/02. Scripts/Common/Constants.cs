using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Paths
{
    public static class Prefabs 
    {
        public const string UI = "Prefabs/UI/";
        public const string Projectile = "Prefabs/Projectile/";
    }
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
        public static readonly int MetalBladeHash = Animator.StringToHash("MetalBlade");
        public static readonly int UpperSlash = Animator.StringToHash("UpperSlash");
        public static readonly int Earthquake = Animator.StringToHash("Earthquake");
        public static readonly int HeavyDestroyer = Animator.StringToHash("HeavyDestroyer");
        public static readonly int IsArrived = Animator.StringToHash("IsArrived");

    }

    public static class MonsterAnimation
    {
        public const string Idle = "Idle";
        public const string Run = "Run";
        public const string Stomp = "Stomp";
        public const string UpperSlash = "UpperSlash";
        public const string Earthquake = "Earthquake";
        public const string HeavyDestroyerStart = "HeavyDestroyerStart";
        public const string HeavyDestroyerLoop = "HeavyDestroyerLoop";
        public const string HeavyDestroyerEnd = "HeavyDestroyerEnd";
    }
}
