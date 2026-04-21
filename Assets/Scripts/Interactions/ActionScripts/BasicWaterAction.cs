using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/Water")]
public class BasicWaterAction : EntityAction
{
    public AudioClip[] waterSounds;
    public static Action onWater;
    public override string GetName()
    {
        return "Water";
    }
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
                if (cropCheck != null && (!cropCheck.IsWatered() && !cropCheck.IsHarvestable())){
                    
                    targets.Add(currentTile);
                }

            }
        }

        return targets;

    }

    public override bool IsAOE()
    {
        return false;
    }

    public override bool IsPossible(Entity unit)
    {
        //Attack isn't possible if there are no nearby enemy units or the unit already moved
        if (GetValidTargets(unit).Count <= 0 || !unit.IsActive())
        {
            return false;
        }
        return true;
    }

    public override void PerformAt(Entity unit, List<Vector3Int> positions)
    {
        //Just attack the unit from the selected position, for this basic attack there shouldn't be more than one target
        PerformAt(unit, positions[0]);

    }

    public override void PerformAt(Entity unit, Vector3Int pos)
    {
        TileManager manager = FindFirstObjectByType<TileManager>();
        //Execute a simple attack on the unit at the location specified
        Crop targetCrop = manager.GetCropOnTile(pos);

        if (targetCrop == null)
        {
            return;
        }

        Unit unituse = unit as Unit;
        if (unituse != null && unituse.HasAnimator())
        {
            if(pos.x - unituse.GetGridPos().x != 0)
            {
                unituse.animator.SetFloat("facing", pos.x - unituse.GetGridPos().x);
            }
            unituse.SetAnimationTrigger("harvest");
        }

        manager.SetTile(pos, TileType.WateredDirt);

        //Water the crop at the position
        targetCrop.WaterCrop();
        SoundManager.Instance.PlaySound(waterSounds[UnityEngine.Random.Range(0, waterSounds.Length)]);
        onWater?.Invoke();
    }
}
