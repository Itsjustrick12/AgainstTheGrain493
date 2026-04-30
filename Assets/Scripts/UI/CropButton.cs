using UnityEngine;
using UnityEngine.UI;

//Turns the crop counter into an interactive button for selecting a crop
public class CropButton : UIButton
{
    //NEEDS TO BE SET EXPLICITILY OR THINGS BREAK
    public int cropID = 1;

    //For shrinking counter
    [SerializeField]private CropCounterUI counter;
    [SerializeField] private Image cropImage;
    [SerializeField] private CropButtonArrow arrow;

    public override void Awake()
    {
        base.Awake();
        if (parentUI == null)
        {
            parentUI = FindFirstObjectByType<PickCropUI>();
        }
        if (cropImage == null)
        {
            cropImage = GetComponentInChildren<Image>();
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
        arrow?.SetIconOnly(value);
    }

    public void SetCropID(int id)
    {
        counter.cropID = id;
        cropID = id;
        cropImage.sprite = CropDatabase.Instance.GetIcon(id);
    }

    public override void SetSelected(bool selected)
    {
        base.SetSelected(selected);
        if (selected)
            arrow?.Show();
        else
            arrow?.Hide();
    }
}
