using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantAction", menuName = "Actions/Plant")]
public class PlantAction : EntityAction
{
    public int cropID = 1;
    public static Action onPlant;

    public void SetCropID(int id)
    {
        cropID = id;
    }

    //actually checks to see if the action can be done at position tilePos
    public override bool Action(TileData tileData)
    {
        if (tileData != null && tileData.IsPlantable())
        {
            return true;
        }

        return false;
    }
    //actually preforms the Action on the tile
    public override void PerformAt(TileData tileData)
    {
        Vector3Int pos = tileData.GetGridPos();
        GameManager manager = FindFirstObjectByType<GameManager>();
        manager.SpawnCropOnTile(CropDatabase.Instance.GetCropInfo(cropID), pos);
        onPlant?.Invoke();
    }
}
