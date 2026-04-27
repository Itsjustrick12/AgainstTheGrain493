using UnityEngine;

public class UnpauseGame : MonoBehaviour
{
    public void Unpause()
    {
        GameManager manager = FindFirstObjectByType<GameManager>();
        if (manager != null)
        {
            manager.UnPauseGame();
        }
    }
}
