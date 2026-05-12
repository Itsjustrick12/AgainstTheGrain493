using TMPro;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text mainObjectiveText;
    CanvasGroup alphaGroup;

    public AudioClip PopUpSound;

    private void Awake()
    {
        Instance = this;
        alphaGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        GameManager.StartEnemyTurn += HideBox;
        GameManager.StartPlayerTurn += ShowBox;
        RegisterLuaFunctions();
    }

    private void OnDisable()
    {
        GameManager.StartEnemyTurn -= HideBox;
        GameManager.StartPlayerTurn -= ShowBox;
        UnregisterLuaFunctions();
    }

    private void RegisterLuaFunctions()
    {
        Lua.RegisterFunction("ShowObjectiveBox", this, SymbolExtensions.GetMethodInfo(() => ShowBox()));
        Lua.RegisterFunction("HideObjectiveBox", this, SymbolExtensions.GetMethodInfo(() => HideBox()));
        Lua.RegisterFunction("SetObjective", this, SymbolExtensions.GetMethodInfo(() => SetObjective(string.Empty)));
        Lua.RegisterFunction("ClearObjective", this, SymbolExtensions.GetMethodInfo(() => Clear()));
    }

    private void UnregisterLuaFunctions()
    {
        Lua.UnregisterFunction("ShowObjectiveBox");
        Lua.UnregisterFunction("HideObjectiveBox");
        Lua.UnregisterFunction("SetObjective");
        Lua.UnregisterFunction("ClearObjective");
    }

    public void ShowBox()
    {
        if (mainObjectiveText.text == "") return;
        alphaGroup.alpha = 255f;
        SoundManager.Instance.PlaySound(PopUpSound);
    }

    public void HideBox()
    {
        alphaGroup.alpha = 0f;
    }

    public void SetObjective(string text)
    {
        mainObjectiveText.text = text;
        ShowBox();
    }

    public void Clear()
    {
        mainObjectiveText.text = "";
        HideBox();
    }
}