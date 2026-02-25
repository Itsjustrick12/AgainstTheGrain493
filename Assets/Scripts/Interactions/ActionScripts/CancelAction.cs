using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Cancel")]
public class Cancel : EntityAction
{
    public override string GetName()
    {
        return "Cancel";
    }

    public override List<Vector3Int> GetValidTargets(Entity entity)
    {
        return null;
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
        //Literally just dont do anything
    }

    public override void PerformAt(Entity entity, Vector3Int pos)
    {
        //Literally just dont do anything
    }
}
