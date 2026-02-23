using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/Attack")]
public class BasicAttackAction : UnitAction
{
    public override string GetName()
    {
        return "Attack";
    }
    //Need to validate size when returned
    public override List<Vector3Int> GetValidTargets(Unit unit)
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
                Unit unitCheck = data.occupyingEntity as Unit;
                if (unitCheck == null)
                {
                    continue;
                }
                else if (!unit.IsSameTeamAs(unitCheck))
                {

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

    public override bool IsPossible(Unit unit)
    {
        //Attack isn't possible if there are no nearby enemy units or the unit already moved
        if (GetValidTargets(unit).Count <= 0 && !unit.isActive)
        {
            return false;
        }
        return true;
    }

    public override void PerformAt(Unit unit, List<Vector3Int> positions)
    {
        //Just attack the unit from the selected position, for this basic attack there shouldn't be more than one target
        PerformAt(unit, positions[0]);

    }

    public override void PerformAt(Unit unit, Vector3Int pos)
    {
        //Execute a simple attack on the unit at the location specified
        Unit targetUnit = FindFirstObjectByType<TileManager>().GetUnitOnTile(pos);

        if (targetUnit == null)
        {
            return;
        }

        //do a simple attack
        targetUnit.TakeDamage(unit.GetStrength());
    }
}
