using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    ATTACK,
    HURT,
    SELECT,
    PLACE,
    WALK,
    DEATH
}

public enum MusicTrack
{
    NONE,
    MAIN_MENU, //1
    BATTLE, //2
    VICTORY, //3
    GAME_OVER //4
}

public class SoundManager : MonoBehaviour
{
    //[SerializeField] private AudioClip[] soundList;
    public static SoundManager Instance;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    private bool sfxMuted = false;
    private bool musicMuted = false;

    [SerializeField] private AudioClip[] musicTracks;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSFXMuted(bool muted)
    {
        sfxMuted = muted;
    }

    public bool IsSFXMuted() => sfxMuted;

    public void SetMusicMuted(bool muted)
    {
        musicMuted = muted;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

    public bool IsMusicMuted() => musicMuted;

    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.Log("No Sound for that type!");
            return;
        }
        if (sfxMuted)
        {
            return;
        }
        sfxSource.PlayOneShot(clip);
    }

    public void PlayEntitySound(Entity entity, SoundType type)
    {
        Unit tempUnit = entity as Unit;
        if(tempUnit != null)
        {
            int unitID = tempUnit.ID;
            UnitInfo info = UnitDatabase.Instance.GetUnitInfo(unitID);
            switch (type)
            {
                case SoundType.HURT:
                    PlaySound(info.hurtSound);
                    break;
                case SoundType.DEATH:
                    PlaySound(info.deathSound);
                    break;
                case SoundType.ATTACK:
                    PlaySound(info.attackSound);
                    break;
                case SoundType.WALK:
                    PlaySound(info.walkSound);
                    break;
                default:
                    break;
            }
        }
    }

    public void PlayMusic(MusicTrack track)
    {
        if (track == MusicTrack.NONE)
        {
            StopMusic();
            return;
        }

        if (musicMuted)
        {
            return;
        }

        AudioClip clip = musicTracks[(int)track - 1]; // -1 to skip NONE
        if (clip == null) return;

        // Don't restart if already playing the same track
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }
}
