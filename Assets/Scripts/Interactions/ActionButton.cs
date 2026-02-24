using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField]private UnitAction storedAction;

    public void Initialize(UnitAction action)
    {
        if (action == null)
        {
            Debug.LogError("This action doesn't exist!");
        }
        storedAction = action;
        text.text = action.GetName();
    }

    public UnitAction GetAction()
    {
        return storedAction;
    }

}