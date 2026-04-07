using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/EndTurn")]

public class EndTurnAction : EntityAction
{
    public override string GetName()
    {
        return "End Turn";
    }

    public override List<Vector3Int> GetValidTargets(Entity entity)
    {
        List<Vector3Int> valid = new List<Vector3Int>();
        valid.Add(new Vector3Int(0, 0, -1));
        return valid;
    }

    public override bool IsAOE()
    {
        return false;
    }

    public override bool IsPossible(Entity entity)
    {
        return true;
    }

    public override void PerformAt(Entity entity, List<Vector3Int> positions)
    {
        return;
    }

    public override void PerformAt(Entity entity, Vector3Int pos)
    {
        return;
    }
}
