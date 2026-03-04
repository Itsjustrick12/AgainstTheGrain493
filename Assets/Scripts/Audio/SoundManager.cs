using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    MOVE,
    ATTACK,
    HURT,
    SELECT,
    PLACE,
    DEATH
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    //[SerializeField] private AudioClip[] soundList;
    public static SoundManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    /*
    public static void PlaySound(SoundType sound , float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
    */
}
