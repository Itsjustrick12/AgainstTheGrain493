using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/AdvancedWater")]
public class AdvancedWaterAction : BasicWaterAction
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

        //get a reference to all tiles
        foreach (Vector3Int offset in TileManager.DIRECTIONS)
        {
            for(int i = 0; i < 3; i++)
            {
                Vector3Int currentTile = startPos + (offset * i);
                TileData data = TM.GetTileDataAt(currentTile);
                //check if there is a dirt tile //or an enemy 
                if(data.GetType() == TileType.Dirt)// || (data.GetOccupyingEntity() as Unit && (data.GetOccupyingEntity() as Unit))
                {
                    targets.Add(startPos + offset);
                    i = 3;
                }
            }
        }
        //Debug.Log("Found " + targets.Count + " different crops that can be harvested");
        return targets;

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

    public override bool IsAOE()
    {
        return true;
    }

    public override void PerformAt(Entity unit, Vector3Int pos)
    {
        TileManager manager = FindFirstObjectByType<TileManager>();

        //attempt to trigger the animation
        Unit unitCheck = unit as Unit;
        if(unitCheck != null && unitCheck.HasAnimator())
        {
            if(pos.x - unitCheck.GetGridPos().x != 0)
            {
                unitCheck.animator.SetFloat("facing", pos.x - unitCheck.GetGridPos().x);
            }
            unitCheck.SetAnimationTrigger("water");
        }

        //attempt to water closest
        Crop targetCrop = manager.GetCropOnTile(pos);
        Unit targetUnit = manager.GetUnitOnTile(pos);
        if(targetCrop != null) targetCrop.WaterCrop();
        if(targetUnit != null) AddDebuff(targetUnit);
        if(manager.GetTileTypeAt(pos) == TileType.Dirt) manager.SetTile(pos, TileType.WateredDirt);

        //water the dirt away from the unit, if there's an enemy on the tile debuff it, else if there's a crop water it
        if(pos.x != unit.GetGridPos().x)
        {   
            int direction = (pos.x - unit.GetGridPos().x) / Mathf.Abs(pos.x - unit.GetGridPos().x);
            pos = new Vector3Int(pos.x + direction, pos.y, pos.z);
            if(manager.GetTileTypeAt(pos) == TileType.Dirt) manager.SetTile(pos, TileType.WateredDirt);
            targetCrop = manager.GetCropOnTile(pos);
            targetUnit = manager.GetUnitOnTile(pos);
            if(targetCrop != null) targetCrop.WaterCrop();
            if(targetUnit != null) AddDebuff(targetUnit);
            pos = new Vector3Int(pos.x + direction, pos.y, pos.z);
            if(manager.GetTileTypeAt(pos) == TileType.Dirt) manager.SetTile(pos, TileType.WateredDirt);
            targetCrop = manager.GetCropOnTile(pos);
            targetUnit = manager.GetUnitOnTile(pos);
            if(targetCrop != null) targetCrop.WaterCrop();
            if(targetUnit != null) AddDebuff(targetUnit);
        }
        else if(pos.y != unit.GetGridPos().y)
        {
            int direction = (pos.y - unit.GetGridPos().y) / Mathf.Abs(pos.y - unit.GetGridPos().y);
            pos = new Vector3Int(pos.x, pos.y + direction, pos.z);
            if(manager.GetTileTypeAt(pos) == TileType.Dirt) manager.SetTile(pos, TileType.WateredDirt);
            targetCrop = manager.GetCropOnTile(pos);
            targetUnit = manager.GetUnitOnTile(pos);
            if(targetCrop != null) targetCrop.WaterCrop();
            if(targetUnit != null) AddDebuff(targetUnit);
            pos = new Vector3Int(pos.x, pos.y + direction, pos.z);
            if(manager.GetTileTypeAt(pos) == TileType.Dirt) manager.SetTile(pos, TileType.WateredDirt);
            targetCrop = manager.GetCropOnTile(pos);
            targetUnit = manager.GetUnitOnTile(pos);
            if(targetCrop != null) targetCrop.WaterCrop();
            if(targetUnit != null) AddDebuff(targetUnit);
        }

        //SoundManager.Instance.PlaySound(waterSounds[UnityEngine.Random.Range(0, waterSounds.Length)]);
        onWater?.Invoke();
    }

    public override List<Vector3Int> GetExtensionTiles(Vector3Int target, Vector3Int casterPos)
    {
        Vector3Int dir = new Vector3Int(
            Math.Sign(target.x - casterPos.x),
            Math.Sign(target.y - casterPos.y), 0);

        return new List<Vector3Int> { target + dir, target + (dir * 2)};
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
