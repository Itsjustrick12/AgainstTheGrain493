using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/Harvest")]
public class BasicHarvestAction : EntityAction
{
    public static Action onHarvest;
    
    //Need to validate size when returned
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
        //Execute a simple attack on the unit at the location specified
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

        //Water the crop at the position

        targetCrop.Harvest();
        onHarvest?.Invoke();
    }
}
