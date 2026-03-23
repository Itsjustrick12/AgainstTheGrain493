using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//This is the base class for making actions in the game that are availble after moving a unit
public abstract class EntityAction : ScriptableObject
{

    //Returns the name of the action for scripts to identify actions with
    public abstract string GetName();

    //Determines whether or not a unit can perform a given action
    public abstract bool IsPossible(Entity entity);
    //Holds the logic that is excuted when the action is chosen

    public abstract List<Vector3Int> GetValidTargets(Entity entity);

    public abstract bool IsAOE();

    //The unit here is the unit performing the action
    public abstract void PerformAt(Entity entity, List<Vector3Int> positions);
    //The unit here is the unit performing the action
    public abstract void PerformAt(Entity entity, Vector3Int pos);
}