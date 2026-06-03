using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/Water")]
public class BasicWaterAction : EntityAction
{
    public static Action onWater;

    //actually checks to see if the action can be done at position tilePos
    public virtual bool Action(TileData tileData)
    {
        if (tileData != null && tileData.HasOccupant())
        {
            Crop cropCheck = tileData.occupyingEntity as Crop;
            //You only need to water crops if they aren't fully grown and they haven't been watered already
            if (cropCheck != null && (!cropCheck.IsWatered() && !cropCheck.IsHarvestable())){
                
                return true;
            }

        }
        return false;
    }

    //actually preforms the Action on the tile
    public virtual void PerformAt(TileData tileData)
    {
        Crop targetCrop = tileData.occupyingEntity as Crop;
        TileManager manager = FindFirstObjectByType<TileManager>();

        //make sure a crop exists
        if (targetCrop == null)
        {
            Debug.Log("No Crop");
            return;
        }

        manager.SetTile(tileData.GetGridPos(), TileType.WateredDirt);
        targetCrop.WaterCrop();
        onWater?.Invoke();
    }
}
