using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Actions/TestAction")]
public class TestAction : UnitAction
{

    //Returns the name of the action for scripts to identify actions with
    public override string GetName() => "Test";

    public override List<Vector3Int> GetValidTargets(Unit unit)
    {
        //do a loop to select all the valid targets
        return new List<Vector3Int>();
    }

    public override bool IsAOE()
    {
        return false;
    }

    //Determines whether or not a unit can perform a given action
    public override bool IsPossible(Unit unit)
    {
        return unit.IsActive();
    }

    public override void PerformAt(Unit unit, List<Vector3Int> positions)
    {
        PerformAt(unit, positions[0]);
    }

    public override void PerformAt(Unit unit, Vector3Int pos)
    {
        Debug.Log("Unit with ID " + unit.ID + " is performing the TestAction at position " + pos);
    }
}