using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/Wait")]
public class WaitAction : EntityAction
{
    public override string GetName()
    {
        return "Wait";
    }

    public override List<Vector3Int> GetValidTargets(Entity unit)
    {
        return null;
    }

    public override bool IsAOE()
    {
        return false;
    }

    public override bool IsPossible(Entity unit)
    {
        return true;
    }

    public override void PerformAt(Entity unit, List<Vector3Int> positions)
    {
        //Literally just dont do anything
    }

    public override void PerformAt(Entity unit, Vector3Int pos)
    {
        //Literally just dont do anything
    }
}
