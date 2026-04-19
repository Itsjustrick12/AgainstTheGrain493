using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
[CreateAssetMenu(menuName = "Actions/Attack")]
public class BasicAttackAction : EntityAction
{
    public override string GetName()
    {
        return "Attack";
    }
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

        ////get a reference to all tiles nearby and check if there are opposing units there
        //foreach (Vector3Int offset in TileManager.DIRECTIONS)
        //{
        //    Vector3Int currentTile = startPos + offset;
        //    TileData data = TM.GetTileDataAt(currentTile);

        //    if (data != null && data.HasOccupant())
        //    {
        //        Unit unitCheck = data.occupyingEntity as Unit;
        //        if (unitCheck == null)
        //        {
        //            continue;
        //        }
        //        else if (!unit.IsSameTeamAs(unitCheck))
        //        {

        //            targets.Add(currentTile);
        //        }

        //    }
        //}

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

    public override void PerformAt(Entity entity, Vector3Int pos)
    {
        //Execute a simple attack on the unit at the location specified
        TileManager tileManager = FindFirstObjectByType<TileManager>();
        Unit targetUnit = tileManager.GetUnitOnTile(pos);

        if (targetUnit == null)
        {
            return;
        }

        Unit unit = entity as Unit;
        if (unit == null)
        {
            Debug.LogError("No Unit, just an entity");
        }

        unit.SetAnimationTrigger("attack");

        //do a simple attack
        SoundManager.Instance.PlayEntitySound(entity, SoundType.ATTACK);
        unit.ShowNumber(unit.GetStrength(), tileManager.entitiesMap.CellToWorld(pos), unit.GetGridPos().x - targetUnit.GetGridPos().x);
        targetUnit.TakeDamage(unit.GetStrength(), unit.GetGridPos());
    }
}
