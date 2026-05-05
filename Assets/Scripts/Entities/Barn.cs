using UnityEngine;
using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
public class Barn : Structure
{
    public static Action OnBarnInteraction;
    [SerializeField] private ParticleSystem sparkle;
    public override void Interact()
    {
        if (isActive)
        {
            Debug.Log("INTERACTING WITH BARN");
            OnBarnInteraction?.Invoke();
        }
        Debug.Log("Barn is at position" + GetGridPos());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        HideParticles();
    }

    private void OnEnable()
    {
        Lua.RegisterFunction("ShowBarnParticles", this,
            SymbolExtensions.GetMethodInfo(() => ShowParticles()));
        Lua.RegisterFunction("HideBarnParticles", this,
            SymbolExtensions.GetMethodInfo(() => HideParticles()));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("ShowBarnParticles");
        Lua.UnregisterFunction("HideBarnParticles");
    }

    public void ShowParticles()
    {
        if (sparkle == null) return;
        sparkle.Play();
    }

    public void HideParticles()
    {
        if (sparkle == null) return;

        sparkle.Stop();
    }

}
