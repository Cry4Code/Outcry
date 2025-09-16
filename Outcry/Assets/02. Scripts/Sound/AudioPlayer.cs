using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float volume, float pitch)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        // 매번 다른 소리처럼 들리게 피치를 살짝 변형
        audioSource.pitch = pitch * Random.Range(0.95f, 1.05f);

        audioSource.Play();
        
        StartCoroutine(ReturnToPoolWhenFinished());
    }

    private IEnumerator ReturnToPoolWhenFinished()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        AudioManager.Instance.ReturnAudioPlayerToPool(this);
    }
}
