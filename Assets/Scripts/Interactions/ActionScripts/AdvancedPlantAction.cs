using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/AdvancedPlant")]
public class AdvancedPlantAction : PlantAction
{

    //actually checks to see if the action can be done at position tilePos
    public override bool Action(TileData tileData)
    {
        if(data.IsPlantable())
        {
            return true;
        }
        return false;
    }

    //actually preforms the Action on the tile
    public override void PerformAt(TileData tileData)
    {
        //attempt to plant crop
        GM.SpawnCropOnTile(CropDatabase.Instance.GetCropInfo(cropID), pos);
        onPlant?.Invoke();
    }
}
