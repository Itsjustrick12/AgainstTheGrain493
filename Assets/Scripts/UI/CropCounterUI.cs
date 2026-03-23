using UnityEngine;
using UnityEngine.UI;
public class CropCounterUI : ExpandingCounterUI
{
    public int cropID = 1;
    public Sprite normalSprite;
    public Sprite highlightSprite;
    public Sprite unavailibleSprite;
    public Image image;

    public bool availible = true;

    protected override int GetCounterValue()
    {
        return EconomyManager.Instance.GetHarvestedCrops(cropID);
    }

    public void Select()
    {
        image.sprite = highlightSprite;
    }

    public void Deselect()
    {
        image.sprite = normalSprite;
    }

    public void SetUnavailible()
    {
        image.sprite = unavailibleSprite;
    }

    public override void UpdateCounter()
    {
        UpdateCounter(cropID);
    }

    public override void UpdateCounter(int id)
    {
        if (id != cropID) return;
        int amt = GetCounterValue();
        base.UpdateCounter(amt);
    }

    protected override void SubscribeEvents()
    {
        EconomyManager.OnCropChanged += UpdateCounter;
    }

    protected override void UnsubscribeEvents()
    {
        EconomyManager.OnCropChanged -= UpdateCounter;
    }

    public void UpdateAvailability()
    {
        // Check how many crops the player has
        int count = EconomyManager.Instance.GetHarvestedCrops(cropID);

        if (count > 0)
        {
            SetAvailable();
        }
        else
        {
            SetUnavailable();
        }
    }

    public void SetAvailable()
    {
        image.sprite = normalSprite;
        // Ensure button can be interacted with
        availible = true;
    }

    public void SetUnavailable()
    {
        image.sprite = unavailibleSprite;
        availible = false;
    }

}
