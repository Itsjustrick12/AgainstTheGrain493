using UnityEngine;
using UnityEngine.UI;

//Turns the crop counter into an interactive button for selecting a crop
public class CropButton : UIButton
{
    //NEEDS TO BE SET EXPLICITILY OR THINGS BREAK
    public int cropID = 1;

    //For shrinking counter
    private CropCounterUI counter;

    public override void Awake()
    {
        base.Awake();
        if (parentUI == null)
        {
            parentUI = FindFirstObjectByType<PickCropUI>();
        }
        if (counter == null)
        {
            counter = GetComponent<CropCounterUI>();
        }
        acceptingInput = false;
    }

    public void UpdateAvailability()
    {
        int count = EconomyManager.Instance.GetHarvestedCrops(cropID);
        SetAvailable(count > 0);
    }

    public void SetIconOnly(bool value)
    {
        if (counter != null)
        {
            counter.SetIconOnly(value);
        }
    }
}
