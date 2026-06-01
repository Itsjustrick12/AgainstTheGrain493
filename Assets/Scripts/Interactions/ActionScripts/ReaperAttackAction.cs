using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/ReaperAttack")]
public class ReaperAttackAction : BasicAttackAction
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
                //makes sure data exists, has an occupying unit, and is an enemy
                if(data != null && data.HasOccupant() && data.occupyingEntity as Unit != null && (data.occupyingEntity as Unit).GetIsEnemy())
                {
                    targets.Add(currentTile);
                }
                else 
                {
                    data = TM.GetTileDataAt(new Vector3Int(currentTile.x, currentTile.y + 1, currentTile.z));

                    if(data != null && data.HasOccupant() && data.occupyingEntity as Unit != null && (data.occupyingEntity as Unit).GetIsEnemy())
                    {
                        targets.Add(currentTile);
                    }
                    else
                    {
                        data = TM.GetTileDataAt(new Vector3Int(currentTile.x, currentTile.y - 1, currentTile.z));

                        if(data != null && data.HasOccupant() && data.occupyingEntity as Unit != null && (data.occupyingEntity as Unit).GetIsEnemy())
                        {
                            targets.Add(currentTile);
                        }
                    }
                }
            }
            else if(currentTile.y != unit.GetGridPos().y)
            {
                if(data != null && data.HasOccupant() && data.occupyingEntity as Unit != null && (data.occupyingEntity as Unit).GetIsEnemy())
                {
                    targets.Add(currentTile);
                }
                else 
                {
                    data = TM.GetTileDataAt(new Vector3Int(currentTile.x + 1, currentTile.y, currentTile.z));

                    if(data != null && data.HasOccupant() && data.occupyingEntity as Unit != null && (data.occupyingEntity as Unit).GetIsEnemy())
                    {
                        targets.Add(currentTile);
                    }
                    else
                    {
                        data = TM.GetTileDataAt(new Vector3Int(currentTile.x - 1, currentTile.y, currentTile.z));

                        if(data != null && data.HasOccupant() && data.occupyingEntity as Unit != null && (data.occupyingEntity as Unit).GetIsEnemy())
                        {
                            targets.Add(currentTile);
                        }
                    }
                }
            }
            if (data != null && data.HasOccupant())
            {
                
                Unit unitCheck = data.occupyingEntity as Unit;
                if (unitCheck != null && unitCheck.GetIsEnemy())
                {
                    targets.Add(currentTile);
                }

            }
        }
        //Debug.Log("Found " + targets.Count + " different crops that can be harvested");
        return targets;

    }

    public override void PerformAt(Entity entity, Vector3Int pos)
    {
        TileManager manager = FindFirstObjectByType<TileManager>();
        Unit unit = entity as Unit;
        if(unit != null && unit.HasAnimator())
        {
            if(pos.x - unit.GetGridPos().x != 0)
            {
                unit.animator.SetFloat("facing", pos.x - unit.GetGridPos().x);
            }
            unit.SetAnimationTrigger("attack");
        }

        //if an enemy exists, atack them
        Unit targetUnit = manager.GetUnitOnTile(pos);
        if(targetUnit != null && targetUnit.GetIsEnemy())
        {
            unit.ShowNumber(unit.GetStrength(), targetUnit.GetGridPos(), unit.GetGridPos().x - targetUnit.GetGridPos().x);
            targetUnit.TakeDamage(unit.GetStrength(), unit.GetGridPos());
        }

        SoundManager.Instance.PlayEntitySound(entity, SoundType.ATTACK);

        //also attack the neighboring enemies as well
        if(pos.x != unit.GetGridPos().x)
        {
            targetUnit = manager.GetUnitOnTile(new Vector3Int(pos.x, pos.y + 1, pos.z));
            if(targetUnit != null && targetUnit.GetIsEnemy())
            {
                unit.ShowNumber(unit.GetStrength(), targetUnit.GetGridPos(), unit.GetGridPos().x - targetUnit.GetGridPos().x);
                targetUnit.TakeDamage(unit.GetStrength(), unit.GetGridPos());
            }
            targetUnit = manager.GetUnitOnTile(new Vector3Int(pos.x, pos.y - 1, pos.z));
            if(targetUnit != null && targetUnit.GetIsEnemy())
            {
                unit.ShowNumber(unit.GetStrength(), targetUnit.GetGridPos(), unit.GetGridPos().x - targetUnit.GetGridPos().x);
                targetUnit.TakeDamage(unit.GetStrength(), unit.GetGridPos());
            }
        }
        else if(pos.y != unit.GetGridPos().y)
        {
            targetUnit = manager.GetUnitOnTile(new Vector3Int(pos.x + 1, pos.y, pos.z));
            if(targetUnit != null && targetUnit.GetIsEnemy())
            {
                unit.ShowNumber(unit.GetStrength(), targetUnit.GetGridPos(), unit.GetGridPos().x - targetUnit.GetGridPos().x);
                targetUnit.TakeDamage(unit.GetStrength(), unit.GetGridPos());
            }
            targetUnit = manager.GetUnitOnTile(new Vector3Int(pos.x - 1, pos.y, pos.z));
            if(targetUnit != null && targetUnit.GetIsEnemy())
            {
                unit.ShowNumber(unit.GetStrength(), targetUnit.GetGridPos(), unit.GetGridPos().x - targetUnit.GetGridPos().x);
                targetUnit.TakeDamage(unit.GetStrength(), unit.GetGridPos());
            }
        }
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
