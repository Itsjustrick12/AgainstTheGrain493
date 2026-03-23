using UnityEngine;
//Used for facilitating feeding entities
public class FeedManager : MonoBehaviour
{
    public PickCropUI pickCropUI;
    private Unit selectedUnit;
    public void OpenFeedUI(Unit unit)
    {
        if (!EconomyManager.Instance.HasACrop())
        {
            //play a sound here, no crops to feed
            return;
        }
        selectedUnit = unit;

        pickCropUI.OnCropSelected.RemoveListener(OnCropChosen);

        pickCropUI.StartPicking(true);
        //Subscribe the event below for whenever the pickCropUI selects a crop
        pickCropUI.OnCropSelected.AddListener(OnCropChosen);
    }

    private void OnCropChosen(int cropID)
    {
        pickCropUI.OnCropSelected.RemoveListener(OnCropChosen);
        pickCropUI.StopPicking();

        //Apply effect to unit here
        Debug.Log("Feed Crop with ID " + cropID + " to the " + selectedUnit.name);
        //Subtract Crop count by 1
        EconomyManager.Instance.FeedHarvestedCrops(cropID);
    }
}