using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{

}

public class ActionButton : UIButton
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField]private EntityAction storedAction;

    public void Initialize(EntityAction action)
    {
        if (action == null)
        {
            Debug.LogError("This action doesn't exist!");
        }
        storedAction = action;
        text.text = action.GetName();
    }

    public EntityAction GetAction()
    {
        return storedAction;
    }

}