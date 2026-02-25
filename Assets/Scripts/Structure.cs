using UnityEngine;
using System;
public class Structure : Entity
{
    public static Action OnBarnInteraction;
    public void Interact()
    {
        if (isActive)
        {
            Debug.Log("INTERACTING WITH BARN");
            OnBarnInteraction?.Invoke();
        }
    }
}
