using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/AdvancedWater")]
public class AdvancedWaterAction : BasicWaterAction
{

    //actually preforms the Action on the tile
    public virtual void PerformAt(TileData tileData)
    {
        Crop targetCrop = tileData.occupyingEntity as Crop;

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

    public void AddDebuff(Unit targetUnit)
    {
        if(targetUnit != null)
        {
            MovementBuff movementBuff = new MovementBuff(1, -1 * (targetUnit.GetBaseMoveRange() - 1));
            if(movementBuff.baseIncrease > 0)
            {
                movementBuff.baseIncrease = 0;
            }
            targetUnit.AddBuff(movementBuff);
        }
    }
}
