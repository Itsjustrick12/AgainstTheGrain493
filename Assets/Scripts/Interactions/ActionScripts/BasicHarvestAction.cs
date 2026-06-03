using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/Harvest")]
public class BasicHarvestAction : EntityAction
{
    public static Action onHarvest;

    //sees if the action is preformable
    public override bool Action(TileData tileData)
    {
        Crop crop = tileData.GetOccupyingEntity() as Crop;
        if(crop != null && crop.CanBeHarvested())
        {
            return true;
        }
        return false;
    }

    //actually preforms the Action on the tile
    public override void PerformAt(TileData tileData)
    {
        TileManager manager = FindFirstObjectByType<TileManager>();
        Vector3Int pos = tileData.GetGridPos();
        Crop targetCrop = manager.GetCropOnTile(pos);
        if(targetCrop != null)targetCrop.Harvest();
    }
}
