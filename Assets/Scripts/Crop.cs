using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

//The crop object that is spawned in the game world and treated like an entity
public class Crop : Entity
{
    //What crop does this entity represent?
    public int id;
    private CropInfo refCrop;

    public int currentStage = 0;

    public bool isWatered = false;
    public bool isHarvestable = false;

    public void Intialize(CropInfo info)
    {
        refCrop = info;
        id = info.id;
        isWatered = false;
        currentStage = 0;
        isHarvestable = false;
        sprite.sprite = info.growthStageSprites[0];
    }

    public void Intialize(int id)
    {
        CropInfo info = CropDatabase.Instance.GetCropInfo(id);
        if (info == null)
        {
            Debug.LogError("Tried to initialize crop without a valid info in the database scriptable object. Check the resources folder!");
        }
        Intialize(info);
    }

    public void Intialize()
    {
        if (refCrop == null)
        {
            Debug.LogError("Tried to initialize crop without a valid info in the database scriptable object. Check the resources folder!");
        }
        Intialize(refCrop);
    }

    public void WaterCrop()
    {
        isWatered = true;
    }

    public void ProcessGrowth()
    {
        if (CanGrow())
        {
            //Process the crop's stage
            currentStage = Mathf.Min(currentStage + 1, refCrop.numStages);

            //If processed through all stages, its now harvestable
            if (refCrop != null && refCrop.numStages-1 == currentStage)
            {
                //Get the last sprite index
                sprite.sprite = refCrop.growthStageSprites[refCrop.numStages - 1];
                isHarvestable = true;
            }
            else
            {
                if (refCrop.growthStageSprites.Length < currentStage)
                {
                    Debug.Log("There isn't a sprite for this stage");
                    return;
                }
                
                sprite.sprite = refCrop.growthStageSprites[currentStage];
                
            }

        }
        isWatered = false;
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
            //TODO Add logic for increasing the player's crop count
            //Remove the entity from it's current TileData and destroy the GameObject
            gameManager.AddHarvestedCrops(id);
            DestroyEntity();
        }

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
