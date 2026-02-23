using JetBrains.Annotations;
using System.Collections.Generic;
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

    [SerializeField]private int selectedChoice = -1;
    private int numChoices;

    //for input modularity
    DefaultInputActions input;

    public UnitActionEvent OnActionSelected;

    [Header("UI References")]
    public Transform buttonContainer;
    public GameObject actionButtonPrefab;
    public GameObject selectionArrow;

    //Used to manage what actions are shown to the player
    private List<GameObject> spawnedButtons = new();

    public Vector3Int actionLocation;

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        cursor = FindFirstObjectByType<TileCursor>();
        gameManager = FindFirstObjectByType<GameManager>();
        PopulateOptions();
    }
    public void ExecuteAction(InputAction.CallbackContext a)
    {
        //if (!GameManager.instance.isPlayerTurn)
        //{
        //    return;
        //}
        Debug.Log("Current Choice: " + selectedChoice);
        ReportAction();
    }

    //Used ChatGPT here for understanding Vector2 based inputs
    public void Navigate(InputAction.CallbackContext context)
    {
        if (!gameManager.isPlayerTurn)
            return;

        if (!context.performed)
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

        SelectButton();
    }

    public void DeselectButton()
    {
        

    }

    public void SelectButton()
    {
        
    }

    //Called from the UnitInteractionSystem for getting the action the user wants
    public void ShowMenu(Unit unit)
    {
        if (unit == null)
        {
            Debug.LogError("You passed a null unit to ShowMenu");
        }

        gameObject.SetActive(true);
        TurnOffInput();
        //Delete previous buttons
        ClearButtons();

        List<UnitAction> actions = unit.GetAvailableActions();
        
        //Only create buttons for the possible actions from the given position
        foreach (UnitAction action in actions)
        {

            //Each action has it's own "can i do this?" logic, check to see if the action is availible
            if (!action.IsPossible(unit))
            {
                continue;
            }

            CreateButton(action);
        }
        TurnOnInput();
    }
    //Spawns the buttonPrefab for each action availble to the Unit
    private void CreateButton(UnitAction action)
    {
        // Spawn button
        GameObject buttonObj = Instantiate(actionButtonPrefab, buttonContainer);
        spawnedButtons.Add(buttonObj);

        // Get script from prefab
        ActionButton actionButton = buttonObj.GetComponent<ActionButton>();

        // Initialize with action + event callback
        actionButton.Initialize(action, OnActionSelected);
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

    public void ReportAction()
    {

    }

    public void PopulateOptions()
    {

    }

    //Only show details based options and the end turn button when no unit is selected
    public void DetermineDefaults()
    {

    }

    //hide any buttons that have actions that cant be taken
    public void DetermineValidOptions(Unit unit)
    {

        
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
        //input.UI.Submit.performed += ExecuteAction;

    }

    private void OnDisable()
    {
        input.UI.Navigate.performed -= Navigate;
        //input.UI.Submit.performed -= ExecuteAction;
        input.Disable();
    }

    public void TurnOffInput()
    {
        if (input == null)
            return;

        input.Disable();
        input.Dispose();

    }

    public void TurnOnInput()
    {
        if (input == null)
            return;

        input.Enable();

    }
}