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
        public static readonly int Dead = Animator.StringToHash("Dead");
        public static readonly int Stun = Animator.StringToHash("Stun");
        public static readonly int Stomp = Animator.StringToHash("Stomp");
        public static readonly int MetalBladeHash = Animator.StringToHash("MetalBlade");
        public static readonly int UpperSlash = Animator.StringToHash("UpperSlash");
        public static readonly int Earthquake = Animator.StringToHash("Earthquake");
        public static readonly int HeavyDestroyer = Animator.StringToHash("HeavyDestroyer");
        public static readonly int IsArrived = Animator.StringToHash("IsArrived");
        public static readonly int ThreePoint = Animator.StringToHash("ThreePoint");
        public static readonly int WhirlWind = Animator.StringToHash("WhirlWind");
        public static readonly int NormalAttack = Animator.StringToHash("NormalAttack");
        public static readonly int Shark = Animator.StringToHash("Shark");
        public static readonly int RumbleOfRuin = Animator.StringToHash("RumbleOfRuin");
        public static readonly int IsTired = Animator.StringToHash("IsTired");

    }

    public static class MonsterAnimation
    {
        public const string Idle = "Idle";
        public const string Run = "Run";
        public const string Stun = "Stun";
        public const string Stomp = "Stomp";
        public const string UpperSlash = "UpperSlash";
        public const string Earthquake = "Earthquake";
        public const string HeavyDestroyerStart = "HeavyDestroyerStart";
        public const string HeavyDestroyerLoop = "HeavyDestroyerLoop";
        public const string HeavyDestroyerEnd = "HeavyDestroyerEnd";
        public const string Spawn = "Spawn";
        public const string Shark = "Shark";
        public const string WhirlWind = "WhirlWind";
        public const string NormalAttack = "NormalAttack";
    }
}