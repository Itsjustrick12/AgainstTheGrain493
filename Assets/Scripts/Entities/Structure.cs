using UnityEngine;
using System;
using System.Collections.Generic;
public class Structure : Entity
{
    public int ID;
    public virtual void Interact()
    {
        if (isActive)
        {
            Debug.Log("Tried to Intreact with simple structure");
        }
    }

    public override void Awake()
    {
        base.Awake();
        InitializeActions();
    }

    public void InitializeActions()
    {
        //This references the action set defined in the unit database
        actions = StructureDatabase.Instance.GetActions(ID);
    }
}
