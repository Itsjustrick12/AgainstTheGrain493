using UnityEngine;
using System;
using System.Collections.Generic;
public class Structure : Entity
{
    public int ID;
    public static Action OnBarnInteraction;
    public void Interact()
    {
        if (isActive)
        {
            Debug.Log("INTERACTING WITH BARN");
            OnBarnInteraction?.Invoke();
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
