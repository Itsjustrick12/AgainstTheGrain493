using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Actions/Cancel")]
public class Cancel : UnitAction
{
    public override string GetName()
    {
        return "Cancel";
    }

    public override List<Vector3Int> GetValidTargets(Unit unit)
    {
        return null;
    }

    public override bool IsAOE()
    {
        return false;
    }

    public override bool IsPossible(Unit unit)
    {
        return true;
    }

    public override void PerformAt(Unit unit, List<Vector3Int> positions)
    {
        //Literally just dont do anything
    }

    public override void PerformAt(Unit unit, Vector3Int pos)
    {
        //Literally just dont do anything
    }
}
