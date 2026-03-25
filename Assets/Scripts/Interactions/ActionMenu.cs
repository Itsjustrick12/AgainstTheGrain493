using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class NaviagatableUI : MonoBehaviour
{
    protected GameManager gameManager;

    [SerializeField] protected List<GameObject> buttons;
    [SerializeField] protected int selectedChoice = 0;
    protected int numChoices => buttons.Count;
    public GameObject selectionArrow;

    //for input modularity
    DefaultInputActions input;

    [SerializeField] protected AudioClip navigateNoise;
    [SerializeField] protected AudioClip reportNoise;

    public virtual void Navigate(InputAction.CallbackContext context)
    {
        if (gameManager != null && !gameManager.isPlayerTurn)
            return;

        if (buttons == null || buttons.Count == 0)
            return;

        Vector2 inputVector = context.ReadValue<Vector2>();

        // Prevent tiny analog drift from triggering movement
        if (Mathf.Abs(inputVector.y) < 0.5f)
            return;

        int prevIndex = selectedChoice;

        if (inputVector.y > 0)
        {
            // Up
            selectedChoice--;
            if (selectedChoice < 0)
            {
                selectedChoice = numChoices - 1;
            }
            
        }
        else if (inputVector.y < 0)
        {
            // Down
            selectedChoice++;
            if (selectedChoice >= numChoices)
            {
                selectedChoice = 0;
            }
        }

        DeselectButton(prevIndex);
        SelectButton();
        SoundManager.Instance.PlaySound(navigateNoise);
    }

    public virtual void SelectButton()
    {

    }

    public virtual void ReportAction(InputAction.CallbackContext context)
    {
        SoundManager.Instance.PlaySound(reportNoise);
    }
    private void OnEnable()
    {
        input = new DefaultInputActions();
        gameManager = GameManager.Instance;
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

    public virtual void DeselectButton(int index)
    {
        if (numChoices == 0)
            return;

        EventSystem.current.SetSelectedGameObject(null);
    }
    public void TurnOnInput()
    {
        if (input == null)
            return;

        input.Enable();

    }

    //Resets the options upon showing a new unit
    protected void ClearButtons()
    {
        foreach (GameObject btn in buttons)
        {
            Destroy(btn);
        }

        buttons.Clear();
    }
}

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

    ////Used ChatGPT here for understanding Vector2 based inputs
    //public override void Navigate(InputAction.CallbackContext context)
    //{
    //    base.Navigate(context);
    //}

    public override void DeselectButton(int index)
    {
        buttons[index].GetComponent<Image>().color = Color.white;
    }

    public override void SelectButton()
    {
        buttons[selectedChoice].GetComponent<Image>().color = shadeColor;
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

        selectedChoice = 0;
        buttons[selectedChoice].GetComponent<Image>().color = shadeColor;
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
        actionButton.Initialize(action);
    }



    //Is used to tell the interaction system what action to do
    public override void ReportAction(InputAction.CallbackContext context)
    {
        base.ReportAction(context);
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