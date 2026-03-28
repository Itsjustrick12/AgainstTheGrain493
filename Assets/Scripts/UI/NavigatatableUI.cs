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

        int newIndex = selectedChoice;
        if (inputVector.y < 0)
        {
            // Up
            newIndex++;
            if (newIndex >= numChoices)
            {
                newIndex = 0;
            }

        }
        else if (inputVector.y > 0)
        {
            // Down
            newIndex--;
            if (newIndex < 0)
            {
                newIndex = numChoices-1;
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
            DeselectButton(prev);

        SelectButton(selectedChoice);
    }

    public virtual void SelectButton(int index)
    {
        SoundManager.Instance.PlaySound(navigateNoise);
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

    public virtual void TurnOffInput()
    {
        if (input == null)
            return;

        input.Disable();
        //input.Dispose();

    }

    public virtual void TurnOnInput()
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
