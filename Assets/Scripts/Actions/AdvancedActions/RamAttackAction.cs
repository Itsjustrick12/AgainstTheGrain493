using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
[CreateAssetMenu(menuName = "Actions/RamAttack")]
public class RamAttackAction : BasicAttackAction
{
    //Need to validate size when returned
    public override List<Vector3Int> GetValidTargets(Entity entity)
    {
        List<Vector3Int> targets = new List<Vector3Int>();
        //get references 
        if (entity == null)
        {
            Debug.LogError("Trying to get valid targets based on an invalid Unit in attack action");
            return targets;
        }

        TileManager TM = FindFirstObjectByType<TileManager>();

        Unit unit = entity as Unit;
        if (unit == null)
        {
            Debug.LogError("No Unit, just an entity");
        }

        Vector3Int startPos = unit.GetGridPos();
        int atkRange = unit.GetAttackRange();

        for (int i = -atkRange; i <= atkRange; i++)
        {
            for (int j = -atkRange; j <= atkRange; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) > atkRange) continue;

                //Don't include self as target
                if (i == 0 && j == 0) continue;

                Vector3Int currentTile = startPos + new Vector3Int(i, j, 0);
                TileData data = TM.GetTileDataAt(currentTile);
                if (data != null && data.HasOccupant())
                {
                    Unit unitCheck = data.occupyingEntity as Unit;
                    if (unitCheck != null && !unit.IsSameTeamAs(unitCheck))
                    {
                        targets.Add(currentTile);
                    }
                }
            }
        }

        return targets;

    }
    
    public override void PerformAt(Entity entity, Vector3Int pos)
    {
        //Execute a simple attack on the unit at the location specified
        Unit targetUnit = FindFirstObjectByType<TileManager>().GetUnitOnTile(pos);

        //distance the targetunit gets knocked back
        int distance = 2;

        if (targetUnit == null)
        {
            return;
        }

        Unit unit = entity as Unit;
        if (unit == null)
        {
            Debug.LogError("No Unit, just an entity");
        }

        if (unit.HasAnimator())
        {
            if(targetUnit.GetGridPos().x - unit.GetGridPos().x != 0)
            {
                unit.animator.SetFloat("facing", targetUnit.GetGridPos().x - unit.GetGridPos().x);
            }
            unit.SetAnimationTrigger("attack");
        }

        //do a simple attack
        SoundManager.Instance.PlayEntitySound(entity, SoundType.ATTACK);
        unit.ShowNumber(unit.GetStrength(), targetUnit.GetGridPos(), unit.GetGridPos().x - targetUnit.GetGridPos().x);
        targetUnit.TakeDamage(unit.GetStrength(), unit.GetGridPos());
        targetUnit.KnockbackHelper(unit, distance);
    }
}
