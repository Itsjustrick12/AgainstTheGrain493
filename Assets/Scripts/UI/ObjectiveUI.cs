using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text mainObjectiveText;

    private void Awake()
    {
        Instance = this;
    }

    // Main objective (big goal)
    public void SetObjective(string text)
    {
        mainObjectiveText.text = text;
    }

    // Helper to clear UI
    public void Clear()
    {
        mainObjectiveText.text = "";
    }
}