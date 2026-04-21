using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
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
    public TextMeshProUGUI topText;

    [Header("Default Actions")]
    public EntityAction wait;
    public EntityAction cancel;
    public EntityAction endTurn;

    [Header("Layout Sizing")]
    public RectTransform menuPanel;
    public float baseHeight = 16f;
    public float buttonHeight = 48f;


    [SerializeField] private Vector3 offset = new Vector3(2.5f, 0, 0);
    private Vector3 anchoredWorldPos;

    bool isDefaultMenu = false;

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        cursor = FindFirstObjectByType<TileCursor>();
        buttons = new List<GameObject>();
    }

    public override void DeselectButton(int index)
    {
        if (index < 0 || index > buttons.Count - 1) return;
        base.DeselectButton(index);
        buttons[index].GetComponent<Image>().color = Color.white;
    }

    public override void SelectButton(int index)
    {
        if (index < 0 || index > buttons.Count - 1) return;
        base.SelectButton(index);
        buttons[index].GetComponent<Image>().color = shadeColor;
    }

    //Called from the UnitInteractionSystem for getting the action the user wants
    public void ShowMenu(Unit unit)
    {
        if (unit == null)
        {
            Debug.LogError("You passed a null unit to ShowMenu");
        }
        isDefaultMenu = false;
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
        //Resize to fit all the buttons
        ResizeMenu();
        //move to be near unit
        MovePanel(unit);
        topText.text = UnitDatabase.Instance.GetUnitInfo(unit.ID).entityName;
        //Select the first button
        SetSelectedIndex(0);
        TurnOnInput();
    }

    void LateUpdate()
    {
        if (!gameObject.activeSelf) return;

        if (isDefaultMenu)
        {
            transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
        }
        else
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(anchoredWorldPos);
            transform.position = screenPos;
        }
    }

    public void ShowDefaultMenu()
    {
        isDefaultMenu = true;
        gameObject.SetActive(true);
        //Delete previous buttons
        ClearButtons();
        //Add Wait and Cancel
        AddEmptyDefaults();
        ResizeMenu();
        topText.text = "Options";
        //Select the first button
        SetSelectedIndex(0);
        TurnOnInput();
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

    public void AddEmptyDefaults()
    {
        CreateButton(endTurn);
        CreateButton(cancel);
    }

    public void HideMenu()
    {
        TurnOffInput();
        ClearButtons();
        gameObject.SetActive(false);
    }

    //shrink or grow the sprite of the action menu based on the number of buttons
    private void ResizeMenu()
    {
        if (menuPanel == null) return;

        Vector2 size = menuPanel.sizeDelta;
        size.y = baseHeight + buttons.Count * buttonHeight;
        menuPanel.sizeDelta = size;
    }

    public void MovePanel(Unit currUnit)
    {
        //check if on the left side
        bool rightSide = Camera.main.WorldToScreenPoint(currUnit.GetGridPos()).x > (Screen.width / 2f);
        anchoredWorldPos = currUnit.GetGridPos()+(rightSide ? new Vector3(-offset.x+1, offset.y) : offset);
    }

}