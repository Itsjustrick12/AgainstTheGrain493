using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/AdvancedHarvest")]
public class AdvancedHarvestAction : BasicHarvestAction
{

    public override void PerformAt(Entity unit, Vector3Int pos)
    {
        //get the crop that will be harvested
        Crop targetCrop = FindFirstObjectByType<TileManager>().GetCropOnTile(pos);

        if (targetCrop == null)
        {
            return;
        }
        Unit unitCheck = unit as Unit;
        if (unitCheck != null && unitCheck.HasAnimator())
        {
            if(pos.x - unitCheck.GetGridPos().x != 0)
            {
                unitCheck.animator.SetFloat("facing", pos.x - unitCheck.GetGridPos().x);
            }
            unitCheck.SetAnimationTrigger("harvest");
        }

        //Harvest the crop at the position
        targetCrop.Harvest();
        targetCrop = FindFirstObjectByType<TileManager>().GetCropOnTile(new Vector3Int(pos.x + 1, pos.y, pos.z));
        if(targetCrop != null) targetCrop.Harvest();
        targetCrop = FindFirstObjectByType<TileManager>().GetCropOnTile(new Vector3Int(pos.x - 1, pos.y, pos.z));
        if(targetCrop != null) targetCrop.Harvest();
        targetCrop = FindFirstObjectByType<TileManager>().GetCropOnTile(new Vector3Int(pos.x, pos.y + 1, pos.z));
        if(targetCrop != null) targetCrop.Harvest();
        targetCrop = FindFirstObjectByType<TileManager>().GetCropOnTile(new Vector3Int(pos.x, pos.y - 1, pos.z));
        if(targetCrop != null) targetCrop.Harvest();
        onHarvest?.Invoke();
    }
}
