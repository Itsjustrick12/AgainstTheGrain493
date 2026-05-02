using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
public class Structure : Entity
{
    public int ID;
    public bool isEnemy = false;
    public virtual void Interact()
    {
        if (isActive && IsInteractable())
        {
            Debug.Log("Tried to Intreact with simple structure");
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        StructureInfo info = StructureDatabase.Instance.GetStructureInfo(ID);
        actions = info.actions;
        maxHealth = info.baseHealth;
        currentHealth = info.baseHealth;
    }

    public override void Awake()
    {
        base.Awake();
    }

    public bool IsSameTeamAs(Entity entity)
    {
        //if unit get team, if structure get team

        Unit unitCheck = entity as Unit;
        Structure structureCheck = entity as Structure;

        if (unitCheck != null) {
            if ((unitCheck.isEnemy && isEnemy) || ((!unitCheck.isEnemy && !isEnemy))){
                return true;
            }
        }
        else if (structureCheck != null) {
            
            if ((structureCheck.isEnemy && isEnemy) || ((!structureCheck.isEnemy && !isEnemy))){
                return true;
            }
        }
        return false;

    }
}
