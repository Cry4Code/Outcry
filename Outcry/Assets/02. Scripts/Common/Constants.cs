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
        public static readonly int Running = Animator.StringToHash("Running");
        public static readonly int IsDead = Animator.StringToHash("IsDead");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int Stomp = Animator.StringToHash("Stomp");
        public static readonly int MetalBladeHash = Animator.StringToHash("MetalBlade");
        public static readonly int UpperSlash = Animator.StringToHash("UpperSlash");
        public static readonly int Earthquake = Animator.StringToHash("Earthquake");
    }

    public static class MonsterAnimation
    {
        public const string Idle = "Idle";
        public const string Run = "Run";
        public const string Hit = "Hit";
        public const string Stomp = "Stomp";
        public const string UpperSlash = "UpperSlash";
        public const string Earthquake = "Earthquake";
    }
}
