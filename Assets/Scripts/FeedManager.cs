using UnityEngine;
//Used for facilitating feeding entities
public class FeedManager : MonoBehaviour
{
    public PickCropUI pickCropUI;
    private Unit selectedUnit;

    public AudioClip feedSound;
    public AudioClip thumpSound;
    public bool temp = true;
    public void OpenFeedUI(Unit unit)
    {
        if (!EconomyManager.Instance.HasACrop() || unit.GetIsFed())
        {
            //play a sound here, no crops to feed or unit is full already
            SoundManager.Instance.PlaySound(thumpSound);
            return;
        }
        selectedUnit = unit;

        pickCropUI.OnCropSelected.RemoveListener(OnCropChosen);

        pickCropUI.StartPicking(temp);
        //Subscribe the event below for whenever the pickCropUI selects a crop
        pickCropUI.OnCropSelected.AddListener(OnCropChosen);
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
        //For now, just heal the unit slightly
        selectedUnit.Heal(5);

        //if pepper, do strength
        if (cropID == 2)
        {
            selectedUnit.AddBuff(new StrengthBuff(3,5, 1));
        }

        //Set the unit as fed
        selectedUnit.SetIsFed(true);
    }
}