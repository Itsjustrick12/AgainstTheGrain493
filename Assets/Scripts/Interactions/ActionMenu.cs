using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;
using static TMPro.Examples.ObjectSpin;


public class ActionMenu : MonoBehaviour
{

    public Canvas uiCanvas;

    private TileCursor cursor;
    private TileManager tileManager;
    private GameManager gameManager;

    [SerializeField]private int selectedChoice = 0;
    private int numChoices;

    //for input modularity
    DefaultInputActions input;

    public EntityActionEvent OnActionSelected;

    [Header("UI References")]
    public Transform buttonContainer;
    public GameObject actionButtonPrefab;
    public GameObject selectionArrow;

    [Header("Default Actions")]
    public EntityAction wait;
    public EntityAction cancel;

    //Used to manage what actions are shown to the player
    private List<GameObject> spawnedButtons = new();

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        cursor = FindFirstObjectByType<TileCursor>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    //Used ChatGPT here for understanding Vector2 based inputs
    public void Navigate(InputAction.CallbackContext context)
    {
        if (!gameManager.isPlayerTurn)
            return;

        Vector2 inputVector = context.ReadValue<Vector2>();

        // Prevent tiny analog drift from triggering movement
        if (Mathf.Abs(inputVector.y) < 0.5f)
            return;

        DeselectButton();

        if (inputVector.y > 0)
        {
            // Up
            selectedChoice--;
            if (selectedChoice < 0)
                selectedChoice = numChoices - 1;
        }
        else if (inputVector.y < 0)
        {
            // Down
            selectedChoice++;
            if (selectedChoice >= numChoices)
                selectedChoice = 0;
        }
    }

    //Called from the UnitInteractionSystem for getting the action the user wants
    public void ShowMenu(Unit unit)
    {
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

        selectedChoice = 0;
        numChoices = spawnedButtons.Count();
    }

    public void DeselectButton()
    {
        if (numChoices == 0)
            return;

        EventSystem.current.SetSelectedGameObject(null);
    }

    //Spawns the buttonPrefab for each action availble to the Unit
    private void CreateButton(EntityAction action)
    {
        // Spawn button
        GameObject buttonObj = Instantiate(actionButtonPrefab, buttonContainer);
        spawnedButtons.Add(buttonObj);

        // Get script from prefab
        ActionButton actionButton = buttonObj.GetComponent<ActionButton>();

        // Initialize with action + event callback
        actionButton.Initialize(action);
    }

    //Resets the options upon showing a new unit
    private void ClearButtons()
    {
        foreach (GameObject btn in spawnedButtons)
        {
            Destroy(btn);
        }

        spawnedButtons.Clear();
    }

    //Is used to tell the interaction system what action to do
    private void ReportAction(InputAction.CallbackContext context)
    {
        if (spawnedButtons.Count == 0)
            return;

        EntityAction action = spawnedButtons[selectedChoice]
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
        ClearButtons();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        input = new DefaultInputActions();
        input.Enable();
        input.UI.Navigate.performed += Navigate;
        input.UI.Submit.performed += ReportAction;

    }

    private void OnDisable()
    {
        input.UI.Navigate.performed -= Navigate;
        input.UI.Submit.performed -= ReportAction;
        input.Disable();
    }

    public void TurnOffInput()
    {
        if (input == null)
            return;

        input.Disable();
        //input.Dispose();

    }

    public void TurnOnInput()
    {
        if (input == null)
            return;

        input.Enable();

    }
}