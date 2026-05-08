using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    public GameObject controlsPanel;

    public void ShowControls()
    {
        controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
    }
}
