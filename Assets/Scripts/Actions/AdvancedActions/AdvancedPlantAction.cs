using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/AdvancedPlant")]
public class AdvancedPlantAction : PlantAction
{

    //actually checks to see if the action can be done at position tilePos
    public override bool Action(TileData tileData)
    {
        if(tileData.IsPlantable())
        {
            return true;
        }
        return false;
    }

    //actually preforms the Action on the tile
    public override void PerformAt(TileData tileData)
    {
        //attempt to plant crop
        Vector3Int pos = tileData.GetGridPos();
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        gameManager.SpawnCropOnTile(CropDatabase.Instance.GetCropInfo(cropID), pos);
        onPlant?.Invoke();
    }
}
