using UnityEngine;
using UnityEngine.UI;

//Turns the crop counter into an interactive button for selecting a crop
public class CropButton : UIButton
{
    //NEEDS TO BE SET EXPLICITILY OR THINGS BREAK
    public int cropID = 1;
    private Image image;
    public Sprite normalSprite;
    public Sprite highlightSprite;
    public Sprite unavailableSprite;

    public bool isSelected = false;
    public bool available = true;

    //For shrinking counter
    private CropCounterUI counter;

    public override void Awake()
    {
        base.Awake();
        if (parentUI == null)
        {
            parentUI = FindFirstObjectByType<PickCropUI>();
        }
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        if (counter == null)
        {
            counter = GetComponent<CropCounterUI>();
        }
        acceptingInput = false;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisual();
    }

    public void SetAvailable(bool value)
    {
        available = value;
        UpdateVisual(); // default to not selected
    }

    public void UpdateVisual()
    {   
        if (isSelected)
        {
            image.sprite = highlightSprite;
        }
        else if (!available)
        {
            image.sprite = unavailableSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
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
