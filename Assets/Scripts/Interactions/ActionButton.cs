using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ActionButton : UIButton
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private EntityAction storedAction;

    public override void Awake()
    {
        base.Awake();
        if (parentUI == null)
        {
            parentUI = FindFirstObjectByType<ActionMenu>();
        }
    }

    public void InitializeAction(EntityAction action)
    {
        if (action == null)
        {
            Debug.LogError("This action doesn't exist!");
        }
        storedAction = action;
        text.text = action.GetName();
    }

    public void Initialize(int buttonIndex, EntityAction action)
    {
        InitializeAction(action);
        Initialize(buttonIndex);
    }

    public EntityAction GetAction()
    {
        return storedAction;
    }

}