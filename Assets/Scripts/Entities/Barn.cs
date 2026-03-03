using UnityEngine;
using System;
using System.Collections.Generic;
public class Barn : Structure
{
    public static Action OnBarnInteraction;
    public override void Interact()
    {
        if (isActive)
        {
            Debug.Log("INTERACTING WITH BARN");
            OnBarnInteraction?.Invoke();
        }
    }
}
