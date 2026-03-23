using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractableWithCrop
{
    // Called when a crop needs to be picked
    void OnCropPicked(int cropID);
}

//Event needed for both picking feed and planting crop
[System.Serializable]
public class CropSelectedEvent : UnityEngine.Events.UnityEvent<int> { }

public class PickCropUI : NaviagatableUI
{
    public CropSelectedEvent OnCropSelected;
    private bool picking = false;
    private bool feeding = false;
    public override void Navigate(InputAction.CallbackContext context)
    {
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
            } while (!buttons[selectedChoice].GetComponent<CropCounterUI>().availible && attempts <= buttons.Count);
        }
        else if (inputVector.y < 0)
        {
            //Down until next crop or self
            do
            {
                selectedChoice++;
                if (selectedChoice >= buttons.Count) selectedChoice = 0;
                attempts++;
            } while (!buttons[selectedChoice].GetComponent<CropCounterUI>().availible && attempts <= buttons.Count);
        }

        // If no available button found, reset to original
        if (!buttons[selectedChoice].GetComponent<CropCounterUI>().availible)
        {
            selectedChoice = originalChoice;
        }

        SelectButton();
    }

    public override void ReportAction(InputAction.CallbackContext context)
    {
        if (picking)
        {
            //Report the crop id
            // Get the currently selected button
            if (selectedChoice >= 0 && selectedChoice < buttons.Count)
            {
                CropCounterUI counter = buttons[selectedChoice].GetComponent<CropCounterUI>();
                if (counter != null && counter.availible)
                {
                    // Fire your event with the crop ID
                    OnCropSelected?.Invoke(counter.cropID);
                }
            }
            //Do event here
            StopPicking();
        }
    }

    public override void DeselectButton(int index)
    {
        base.DeselectButton(index);
        CropCounterUI counter = buttons[index].GetComponent<CropCounterUI>();
        if (counter != null)
        {

            counter.Deselect();
        }
    }

    public override void SelectButton()
    {
        base.SelectButton();
        CropCounterUI counter = buttons[selectedChoice].GetComponent<CropCounterUI>();
        if (counter != null)
        {

            counter.Select();
        }
    }

    public void StartPicking(bool isFeeding = false)
    {
        if (picking)
            return;
        picking = true;
        feeding = isFeeding;

        selectedChoice = -1;
        //Loop over icons and make them small and skip unavailible ones for starting index
        for (int i = 0; i < buttons.Count; i++)
        {
            CropCounterUI counter = buttons[i].GetComponent<CropCounterUI>();
            if (counter != null)
            {
                if (feeding)
                {
                    counter.UpdateAvailability();
                }
                else
                {
                    counter.SetIconOnly(true);
                }

                //Set the first index if availible hasn't been found yet
                if ((counter.availible || !feeding) && selectedChoice == -1)
                {
                    selectedChoice = i;
                }
            }
        }
        SelectButton();
    }

    public void StopPicking()
    {
        if (!picking)
            return;
        picking = false;
        foreach (GameObject obj in buttons)
        {
            CropCounterUI counter = obj.GetComponent<CropCounterUI>();
            if (counter != null)
            {
                counter.SetIconOnly(false);
                counter.SetAvailable();
            }

        }
    }

}
