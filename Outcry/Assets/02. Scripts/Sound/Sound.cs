using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public string address;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;
}
