using JetBrains.Annotations;
using UnityEngine;
//The crop object that is spawned in the game world and treated like an entity
public class Crop : Entity
{
    //What crop does this entity represent?
    public int id;
    private CropInfo refCrop;

    private int currentStage = 0;

    [Header("State Variables")]

    private bool isWatered = false;
    private bool isHarvestable = false;

    //Used for multiharvesting
    private bool isMultiHarvest = false;
    private int onHarvestStage = 0;
    private bool isBarren = false;
    private Sprite barrenSprite;

    public void Initialize(CropInfo info)
    {
        refCrop = info;
        id = info.id;
        isWatered = false;
        currentStage = 0;
        isHarvestable = false;
        sprite.sprite = info.growthStageSprites[0];
        isMultiHarvest = info.isMultiHarvest;
        onHarvestStage = info.onHarvestStage;
        barrenSprite = info.barrenSprite;
    }

    public override void Initialize()
    {
        CropInfo info = CropDatabase.Instance.GetCropInfo(id);
        if (info == null)
        {
            Debug.LogError("Tried to initialize crop without a valid info in the database scriptable object. Check the resources folder!");
        }
        Initialize(info);
    }

    public void WaterCrop()
    {
        shadeSprite.sprite = sprite.sprite;
        shadeSprite.enabled = true;
        isWatered = true;
    }

    public void ResetWater()
    {
        shadeSprite.sprite = sprite.sprite;
        shadeSprite.enabled = false;
        isWatered = false;
    }

    public void ProcessGrowth()
    {
        if (CanGrow())
        {
            //Process the crop's stage
            currentStage = Mathf.Min(currentStage + 1, refCrop.numStages-1);

            //If processed through all stages, its now harvestable
            if (refCrop != null && refCrop.numStages-1 == currentStage)
            {
                //Get the last sprite index
                sprite.sprite = refCrop.growthStageSprites[refCrop.numStages - 1];
                isHarvestable = true;
                isBarren = false;
            }
            else
            {
                if (refCrop.growthStageSprites.Length <= currentStage)
                {
                    Debug.Log("There isn't a sprite for this stage");
                    return;
                }
                // Only swap away from barren sprite once it actually starts regrowing
                if (isBarren && currentStage > onHarvestStage)
                {
                    isBarren = false;
                }
                if (!isBarren)
                {
                    sprite.sprite = refCrop.growthStageSprites[currentStage];
                }

            }

        }
        ResetWater();
    }

    //May get more complicated later
    public bool CanGrow()
    {
        if (isWatered)
        {
            return true;
        }
        return false;
    }

    public bool CanBeHarvested()
    {
        return isHarvestable;
    }

    public void Harvest()
    {
        if (CanBeHarvested())
        {

            EconomyManager.Instance.AddHarvestedCrops(id);
            //If multiharvest, jump to the stage defined by the CropInfo, then proceed like normal
            if (isMultiHarvest)
            {
                currentStage = onHarvestStage;
                sprite.sprite = barrenSprite;
                isHarvestable = false;
                isBarren = true;
            }
            else
            {
                DestroyEntity();
            }
            //TODO Add logic for increasing the player's crop count
            //Remove the entity from it's current TileData and destroy the GameObject
        }

    }

    public bool IsHarvestable()
    {
        return isHarvestable;
    }

    public bool IsWatered()
    {
        return isWatered;
    }

    //Use the events system to get updates about state when the turn advanced
    private void OnEnable()
    {
        GameManager.StartPlayerTurn += ProcessGrowth;
    }

    private void OnDisable()
    {
        GameManager.StartPlayerTurn -= ProcessGrowth;
    }
}
