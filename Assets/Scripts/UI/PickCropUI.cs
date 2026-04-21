using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//Event needed for both picking feed and planting crop
[System.Serializable]
public class CropSelectedEvent : UnityEngine.Events.UnityEvent<int> { }

public class PickCropUI : NaviagatableUI
{
    public CropSelectedEvent OnCropSelected;
    public UnityEvent OnCropCancelled;
    private bool picking = false;
    private bool feeding = false;

    public GameObject cropButtonPrefab;
    public Transform buttonContainer;
    private List<int> cropIDs = new List<int>();

    [Header("Other Buttons")]
    public RectTransform feedButton;
    public RectTransform endTurnButton;

    public void Start()
    {
        //dont enable input until told to do so
        TurnOffInput();
        GetCropIDs();
        DestroyChildren();
        SpawnCropButtons();
    }

    public void GetCropIDs()
    {
        cropIDs = GameManager.Instance.GetCropIDs();
    }

    // Call this before Start if you want to set crop IDs programmatically
    public void SetCropIDs(List<int> ids)
    {
        cropIDs = ids;
        DestroyChildren();
        SpawnCropButtons();
    }

    public void DestroyChildren()
    {
        foreach (Transform pos in buttonContainer)
        {
            if (pos == feedButton || pos == endTurnButton) continue;
            Destroy(pos.gameObject);
        }
    }

    private void SpawnCropButtons()
    {
        foreach (GameObject obj in buttons)
            Destroy(obj);
        buttons.Clear();

        foreach (int id in cropIDs)
        {
            // Instantiate inactive so OnEnable doesn't fire yet
            GameObject spawned = Instantiate(cropButtonPrefab, buttonContainer);
            CropButton cropButton = spawned.GetComponentInChildren<CropButton>();
            if (cropButton != null)
                cropButton.SetCropID(id);
                buttons.Add(cropButton.gameObject);


        }

        // All buttons configured, now safe to enable
        foreach (GameObject obj in buttons)
            obj.SetActive(true);
        if (feedButton != null)
            feedButton.SetAsLastSibling();
        if (endTurnButton != null)
            endTurnButton.SetAsLastSibling();
    }

    public override void Navigate(InputAction.CallbackContext context)
    {
        if (DialogueManager.IsConversationActive)
        {
            return;
        }
        //if not feeding, dont do availibility skipping
        if (!feeding)
        {
            base.Navigate(context);
            return;
        }

        if (!picking || buttons.Count == 0)
            return;

        Vector2 inputVector = context.ReadValue<Vector2>();

        // Prevent tiny analog drift from triggering movement
        if (Mathf.Abs(inputVector.y) < 0.5f)
            return;

        DeselectButton(selectedChoice);

        int originalChoice = selectedChoice;

        //Do attempt loop to see if theres another index to go to but if not fall out
        int attempts = 0;
        if (inputVector.y > 0)
        {
            //Up until next crop or self
            do
            {
                selectedChoice--;
                if (selectedChoice < 0) selectedChoice = buttons.Count - 1;
                attempts++;
            } while (!buttons[selectedChoice].GetComponent<CropButton>().available && attempts <= buttons.Count);
        }
        else if (inputVector.y < 0)
        {
            //Down until next crop or self
            do
            {
                selectedChoice++;
                if (selectedChoice >= buttons.Count) selectedChoice = 0;
                attempts++;
            } while (!buttons[selectedChoice].GetComponent<CropButton>().available && attempts <= buttons.Count);
        }

        // If no available button found, reset to original
        if (!buttons[selectedChoice].GetComponent<CropButton>().available)
        {
            selectedChoice = originalChoice;
        }

        SelectButton(selectedChoice);
    }

    public override void ReportAction()
    {
        base.ReportAction();
        if (picking)
        {
            //Report the crop id
            // Get the currently selected button
            if (selectedChoice >= 0 && selectedChoice < buttons.Count)
            {
                CropButton button = buttons[selectedChoice].GetComponent<CropButton>();
                if (button != null && button.available)
                {
                    // Fire your event with the crop ID
                    OnCropSelected?.Invoke(button.cropID);
                    if (feeding)
                    {
                        OnCropCancelled?.Invoke();
                    }
                }
            }
            //Do event here
            StopPicking();
        }
    }

    public override void DeselectButton(int index)
    {
        base.DeselectButton(index);
        CropButton btn = buttons[index].GetComponent<CropButton>();
        btn.SetSelected(false);
    }

    public override void SelectButton(int index)
    {
        base.SelectButton(index);
        CropButton btn = buttons[index].GetComponent<CropButton>();
        btn.SetSelected(true);
    }

    public void StartPicking(bool isFeeding = false)
    {
        if (picking)
            return;
        TurnOnInput();
        picking = true;
        feeding = isFeeding;

        selectedChoice = -1;
        //Loop over icons and make them small and skip unavailable ones for starting index
        for (int i = 0; i < buttons.Count; i++)
        {
            CropButton button = buttons[i].GetComponent<CropButton>();
            if (button != null)
            {
                button.Initialize(i);
                if (feeding)
                {
                    button.UpdateAvailability();
                }
                else
                {
                    button.SetIconOnly(true);
                }

                //Set the first index if available hasn't been found yet
                if ((button.available || !feeding) && selectedChoice == -1)
                {
                    selectedChoice = i;
                }
            }
        }
        SetSelectedIndex(selectedChoice);
    }

    public void StopPicking()
    {
        if (!picking)
            return;
        picking = false;
        foreach (GameObject obj in buttons)
        {
            CropButton button = obj.GetComponent<CropButton>();
            if (button != null)
            {
                button.SetIconOnly(false);
                button.SetAvailable(true);
            }

        }

        TurnOffInput();
    }

    public override void TurnOnInput()
    {
        base.TurnOnInput();
        //loop and turn on crop buttons to enable hover
        foreach (GameObject obj in buttons)
        {
            CropButton button = obj.GetComponent<CropButton>();
            if (button != null)
            {
                button.TurnOnInput();
            }

        }
    }

    public override void TurnOffInput()
    {
        base.TurnOffInput();
        //loop and turn off crop buttons to prevent hovers during game
        foreach (GameObject obj in buttons)
        {
            CropButton button = obj.GetComponent<CropButton>();
            if (button != null)
            {
                button.TurnOffInput();
            }

        }

        //hide the highlight on the current button
        if (selectedChoice >= 0 && selectedChoice < buttons.Count)
        {
            DeselectButton(selectedChoice);
        }
    }
    //Used by undo functionality to go back to action selection without breaking anything
    public void CancelPicking()
    {
        if (!picking) return;
        if (selectedChoice >= 0 && selectedChoice < buttons.Count)
        {
            DeselectButton(selectedChoice);
        }
        selectedChoice = -1;
        picking = false;
        feeding = false;
        foreach (GameObject obj in buttons)
        {
            CropButton button = obj.GetComponent<CropButton>();
            if (button != null)
            {
                button.SetIconOnly(false);
                button.SetAvailable(true);
            }
        }
        TurnOffInput();
        OnCropCancelled?.Invoke();
    }
}
