using UnityEngine;
using TMPro;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private UnitAction storedAction;
    private UnitActionEvent onPressed;

    public void Initialize(UnitAction action, UnitActionEvent callback)
    {
        if (action == null)
        {
            Debug.LogError("This action doesn't exist!");
        }
        storedAction = action;
        onPressed = callback;

        text.text = action.GetName();
    }

    public void Press()
    {
        onPressed?.Invoke(storedAction);
    }
}