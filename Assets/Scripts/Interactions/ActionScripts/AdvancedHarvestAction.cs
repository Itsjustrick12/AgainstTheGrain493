using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/AdvancedHarvest")]
public class AdvancedHarvestAction : BasicHarvestAction
{
    //
    public override List<Vector3Int> GetValidTargets(Entity unit)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        //get references 
        if (unit == null)
        {
            Debug.LogError("Trying to get valid targets based on an invalid Unit in attack action");
            return targets;
        }

        TileManager TM = FindFirstObjectByType<TileManager>();

        Vector3Int startPos = unit.GetGridPos();

        //get a reference to all tiles nearby and check if there are opposing units there
        foreach (Vector3Int offset in TileManager.DIRECTIONS)
        {
            Vector3Int currentTile = startPos + offset;
            TileData data = TM.GetTileDataAt(currentTile);

            if(currentTile.x != unit.GetGridPos().x)
            {
                //makes sure data exists, has an occupying entity, is a crop, and can be harvested
                if(data != null && data.HasOccupant() && data.occupyingEntity as Crop != null && (data.occupyingEntity as Crop).CanBeHarvested())
                {
                    targets.Add(currentTile);
                }
                else 
                {
                    data = TM.GetTileDataAt(new Vector3Int(currentTile.x, currentTile.y + 1, currentTile.z));

                    if(data != null && data.HasOccupant() && data.occupyingEntity as Crop != null && (data.occupyingEntity as Crop).CanBeHarvested())
                    {
                        targets.Add(currentTile);
                    }
                    else
                    {
                        data = TM.GetTileDataAt(new Vector3Int(currentTile.x, currentTile.y - 1, currentTile.z));

                        if(data != null && data.HasOccupant() && data.occupyingEntity as Crop != null && (data.occupyingEntity as Crop).CanBeHarvested())
                        {
                            targets.Add(currentTile);
                        }
                    }
                }
            }
            else if(currentTile.y != unit.GetGridPos().y)
            {
                if(data != null && data.HasOccupant() && data.occupyingEntity as Crop != null && (data.occupyingEntity as Crop).CanBeHarvested())
                {
                    targets.Add(currentTile);
                }
                else 
                {
                    data = TM.GetTileDataAt(new Vector3Int(currentTile.x + 1, currentTile.y, currentTile.z));

                    if(data != null && data.HasOccupant() && data.occupyingEntity as Crop != null && (data.occupyingEntity as Crop).CanBeHarvested())
                    {
                        targets.Add(currentTile);
                    }
                    else
                    {
                        data = TM.GetTileDataAt(new Vector3Int(currentTile.x - 1, currentTile.y, currentTile.z));

                        if(data != null && data.HasOccupant() && data.occupyingEntity as Crop != null && (data.occupyingEntity as Crop).CanBeHarvested())
                        {
                            targets.Add(currentTile);
                        }
                    }
                }
            }
            if (data != null && data.HasOccupant())
            {
                Crop cropCheck = data.occupyingEntity as Crop;
                //You only need to water crops if they aren't fully grown and they haven't been watered already
                if (cropCheck != null && cropCheck.CanBeHarvested())
                {

                    targets.Add(currentTile);
                }

            }
        }
        //Debug.Log("Found " + targets.Count + " different crops that can be harvested");
        return targets;

    }

    public override void PerformAt(Entity unit, Vector3Int pos)
    {
        TileManager manager = FindFirstObjectByType<TileManager>();
        Unit unitCheck = unit as Unit;
        if(unitCheck != null && unitCheck.HasAnimator())
        {
            if(pos.x - unitCheck.GetGridPos().x != 0)
            {
                unitCheck.animator.SetFloat("facing", pos.x - unitCheck.GetGridPos().x);
            }
            unitCheck.SetAnimationTrigger("harvest");
        }

        //if a crop exists, harvest the crop
        Crop targetCrop = manager.GetCropOnTile(pos);
        if(targetCrop != null)targetCrop.Harvest();

        //harvest the crops at the positions next to the crop
        if(pos.x != unit.GetGridPos().x)
        {
            targetCrop = manager.GetCropOnTile(new Vector3Int(pos.x, pos.y + 1, pos.z));
            if(targetCrop != null)targetCrop.Harvest();
            targetCrop = manager.GetCropOnTile(new Vector3Int(pos.x, pos.y - 1, pos.z));
            if(targetCrop != null)targetCrop.Harvest();
        }
        else if(pos.y != unit.GetGridPos().y)
        {
            targetCrop = manager.GetCropOnTile(new Vector3Int(pos.x + 1, pos.y, pos.z));
            if(targetCrop != null)targetCrop.Harvest();
            targetCrop = manager.GetCropOnTile(new Vector3Int(pos.x - 1, pos.y, pos.z));
            if(targetCrop != null)targetCrop.Harvest();
        }

        onHarvest?.Invoke();
    }

    public override List<Vector3Int> GetExtensionTiles(Vector3Int target, Vector3Int casterPos)
    {
        Vector3Int dir = new Vector3Int(
            Math.Sign(target.x - casterPos.x),
            Math.Sign(target.y - casterPos.y), 0);
        Vector3Int perp = new Vector3Int(dir.y, dir.x, 0);

        return new List<Vector3Int> { target - perp, target + perp };
    }
}
