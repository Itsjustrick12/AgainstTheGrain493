using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewCrop", menuName = "AgainstTheGrain/Entities/Crop")]
public class CropInfo : EntityInfo
{
    [Header("Crop Specific")]
    //Seed stage counts as a stage, if you want a simple "grow for one turn to harvest" this number would be two
    public int numStages;
    //Used to progress to full harvest, these are the sprites rendered on the tilemap
    //There should be a sprite for each sprite
    public Sprite[] growthStageSprites;


    [Header("MultiHarvesting")]
    public bool isMultiHarvest = false;
    public int onHarvestStage = -1;
    public Sprite barrenSprite = null;
}