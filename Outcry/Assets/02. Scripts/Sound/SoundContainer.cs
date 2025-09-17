using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundContainer", menuName = "Audio/SoundContainer")]
public class SoundContainer : ScriptableObject
{
    public Sound[] sounds;
}
