using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text mainObjectiveText;
    [SerializeField] private TMP_Text stepsText;

    private void Awake()
    {
        Instance = this;
    }

    // Main objective (big goal)
    public void SetMainObjective(string text)
    {
        mainObjectiveText.text = text;
    }

    // Smaller instruction text
    public void SetSteps(string text)
    {
        stepsText.text = text;
    }

    // Helper to clear UI
    public void Clear()
    {
        mainObjectiveText.text = "";
        stepsText.text = "";
    }
}