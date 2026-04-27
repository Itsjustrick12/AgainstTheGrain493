using UnityEngine;

public class PlayMusicOnStart : MonoBehaviour
{

    public MusicTrack track = MusicTrack.MAIN_MENU;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.Instance.PlayMusic(track);
    }
}
