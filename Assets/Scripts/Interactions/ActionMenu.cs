using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ActionMenu : NaviagatableUI
{

    public Canvas uiCanvas;

    private TileCursor cursor;
    private TileManager tileManager;

    public Color shadeColor;

    public EntityActionEvent OnActionSelected;

    [Header("UI References")]
    public Transform buttonContainer;
    public GameObject actionButtonPrefab;

    [Header("Default Actions")]
    public EntityAction wait;
    public EntityAction cancel;

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        cursor = FindFirstObjectByType<TileCursor>();
        buttons = new List<GameObject>();
    }

    public override void DeselectButton(int index)
    {
        base.DeselectButton(index);
        buttons[index].GetComponent<Image>().color = Color.white;
    }

    public override void SelectButton(int index)
    {
        base.SelectButton(index);
        buttons[index].GetComponent<Image>().color = shadeColor;
    }

    //Called from the UnitInteractionSystem for getting the action the user wants
    public void ShowMenu(Unit unit)
    {
        TurnOnInput();
        if (unit == null)
        {
            Debug.LogError("You passed a null unit to ShowMenu");
        }

        gameObject.SetActive(true);
        //Delete previous buttons
        ClearButtons();

        List<EntityAction> actions = unit.GetAvailableActions();

        //Debug.Log("Actions is of size " + actions.Count);
        
        //Only create buttons for the possible actions from the given position
        foreach (EntityAction action in actions)
        {
            CreateButton(action);
        }
        //Add Wait and Cancel
        AddDefaults();

        //Select the first button
        SetSelectedIndex(0);
    }



    //Spawns the buttonPrefab for each action availble to the Unit
    private void CreateButton(EntityAction action)
    {
        // Spawn button
        GameObject buttonObj = Instantiate(actionButtonPrefab, buttonContainer);
        buttons.Add(buttonObj);

        // Get script from prefab
        ActionButton actionButton = buttonObj.GetComponent<ActionButton>();

        // Initialize with action + event callback
        actionButton.Initialize(buttons.Count-1, action);
    }

    public override void ReportAction()
    {
        base.ReportAction();
        if (buttons.Count == 0)
            return;

        EntityAction action = buttons[selectedChoice]
            .GetComponent<ActionButton>()
            .GetAction();

        OnActionSelected.Invoke(action);
        HideMenu();
    }


    //Only show details based options and the end turn button when no unit is selected
    public void AddDefaults()
    {
        CreateButton(wait);
        CreateButton(cancel);
    }

    public void HideMenu()
    {
        TurnOffInput();
        ClearButtons();
        gameObject.SetActive(false);
    }

}