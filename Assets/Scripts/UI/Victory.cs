using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreenScript : MonoBehaviour
{
    public void Setup()
    {
        //gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("Tilemap");
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
