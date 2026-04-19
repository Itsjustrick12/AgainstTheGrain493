using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NaviagatableUI : MonoBehaviour
{
    protected GameManager gameManager;

    [SerializeField] protected List<GameObject> buttons;
    [SerializeField] protected int selectedChoice = 0;
    protected int numChoices => buttons.Count;
    public GameObject selectionArrow;


    //for input modularity
    DefaultInputActions input;

    [SerializeField] protected AudioClip navigateUp;
    [SerializeField] protected AudioClip navigateDown;
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

        int newIndex = selectedChoice;
        if (inputVector.y > 0)
        {
            newIndex--;
            if (newIndex < 0)
            {
                newIndex = numChoices - 1;
            }
        }
        else if (inputVector.y < 0)
        {
            newIndex++;
            if (newIndex >= numChoices)
            {
                newIndex = 0;
            }
        }

        SetSelectedIndex(newIndex);
    }

    public void SetSelectedIndex(int newIndex)
    {
        if (newIndex < 0 || newIndex >= numChoices)
            return;
        


            int prev = selectedChoice;
        selectedChoice = newIndex;

        if (prev >= 0)
        {
            DeselectButton(prev);
            if (prev != newIndex)
            {
                SoundManager.Instance.PlaySound(newIndex > prev ? navigateDown : navigateUp);
            }
        }

        SelectButton(selectedChoice);
    }

    public virtual void SelectButton(int index)
    {
    }
    public virtual void DeselectButton(int index)
    {
    }


    public virtual void MoveSelection(int index)
    {
        //Deselect current before moving selection to new index
        SetSelectedIndex(index);
    }

    public virtual void ReportAction(InputAction.CallbackContext context)
    {
        ReportAction();
    }

    public virtual void ReportAction()
    {
        SoundManager.Instance.PlaySound(reportNoise);
        //Do something here in the derived class
    }

    private void OnEnable()
    {
        input = new DefaultInputActions();
        gameManager = GameManager.Instance;
        //TurnOnInput();

    }

    private void OnDisable()
    {
        TurnOffInput();
    }

    public virtual void TurnOffInput()
    {
        if (input == null) return;
        input.UI.Navigate.performed -= Navigate;
        input.UI.Submit.performed -= ReportAction;
        input.Disable();
    }

    public virtual void TurnOnInput()
    {
        if (input == null) return;
        // Unsubscribe first to prevent duplicate subscriptions
        input.UI.Navigate.performed -= Navigate;
        input.UI.Submit.performed -= ReportAction;
        input.UI.Navigate.performed += Navigate;
        input.UI.Submit.performed += ReportAction;
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
        selectedChoice = 0;
    }
}
