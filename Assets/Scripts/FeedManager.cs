using UnityEngine;
//Used for facilitating feeding entities
public class FeedManager : MonoBehaviour
{
    public PickCropUI pickCropUI;
    private Unit selectedUnit;

    public AudioClip feedSound;
    public AudioClip thumpSound;
    public bool temp = true;

    public static event System.Action OnFeedingComplete;

    public void OpenFeedUI(Unit unit)
    {
        if (!EconomyManager.Instance.HasACrop() || unit.GetIsFed())
        {
            //play a sound here, no crops to feed or unit is full already
            SoundManager.Instance.PlaySound(thumpSound);
            OnFeedingComplete?.Invoke();
            return;
        }
        selectedUnit = unit;

        pickCropUI.OnCropSelected.RemoveListener(OnCropChosen);

        pickCropUI.StartPicking(temp);
        //Subscribe the event below for whenever the pickCropUI selects a crop
        pickCropUI.OnCropSelected.AddListener(OnCropChosen);
    }

    public void CancelFeed()
    {
        if (selectedUnit == null) return;
        pickCropUI.OnCropSelected.RemoveListener(OnCropChosen);
        pickCropUI.CancelPicking();
        selectedUnit = null;
        OnFeedingComplete?.Invoke();
    }

    private void OnCropChosen(int cropID)
    {
        pickCropUI.OnCropSelected.RemoveListener(OnCropChosen);
        pickCropUI.StopPicking();

        //Apply effect to unit here
        //Debug.Log("Feed Crop with ID " + cropID + " to the " + selectedUnit.name);
        //Subtract Crop count by 1
        EconomyManager.Instance.FeedHarvestedCrops(cropID);
        SoundManager.Instance.PlaySound(feedSound);
        
        //For basic wheat, just heal the unit slightly
        if (cropID == 1)
        {
            selectedUnit.Heal(10);
        }
        //if pepper, do strength
        else if (cropID == 2)
        {
            selectedUnit.AddBuff(new StrengthBuff(3,5, 1));
        }
        //if carrot increase movement
        else if (cropID == 3)
        {
            selectedUnit.AddBuff(new MovementBuff(3, 2, 1));
        }
        //if potato increase defense
        else if (cropID == 4)
        {
            selectedUnit.AddBuff(new DefenseBuff(3, 2, 1));
        }

        //Set the unit as fed
        selectedUnit.SetIsFed(true);
        OnFeedingComplete?.Invoke();
    }
}