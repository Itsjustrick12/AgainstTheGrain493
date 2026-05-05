using UnityEngine;

public class UnlockTest : MonoBehaviour
{
    void Start()
    {
        PlayerPrefs.SetInt("Level_1_Unlocked", 1);
        PlayerPrefs.SetInt("Level_2_Unlocked", 0);
        PlayerPrefs.Save();
    }
}