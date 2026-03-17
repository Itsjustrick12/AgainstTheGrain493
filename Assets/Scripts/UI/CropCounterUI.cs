using UnityEngine;

public class CropCounterUI : ExpandingCounterUI
{
    public int cropID = 1;

    protected override int GetCounterValue()
    {
        return EconomyManager.Instance.GetHarvestedCrops(cropID);
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
}
