using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Actions/TestAction")]
public class TestAction : EntityAction
{

    //Returns the name of the action for scripts to identify actions with
    public override string GetName() => "Test";

    public override List<Vector3Int> GetValidTargets(Entity entity)
    {
        //do a loop to select all the valid targets
        return new List<Vector3Int>();
    }

    public override bool IsAOE()
    {
        return false;
    }

    //Determines whether or not a entity can perform a given action
    public override bool IsPossible(Entity entity)
    {
        return entity.IsActive();
    }

    public override void PerformAt(Entity entity, List<Vector3Int> positions)
    {
        PerformAt(entity, positions[0]);
    }

    public override void PerformAt(Entity entity, Vector3Int pos)
    {
        Unit unit = entity as Unit;
        if (unit != null)
        {

            Debug.Log("Unit with ID " + unit.ID + " is performing the TestAction at position " + pos);
        }
    }
}