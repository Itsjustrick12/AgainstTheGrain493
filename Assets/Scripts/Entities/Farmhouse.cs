using UnityEngine;
using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
public class Farmhouse : Structure
{
    public static Action OnFarmhouseInteraction;
    [SerializeField] private ParticleSystem sparkle;
    public override void Interact()
    {
        if (isActive)
        {
            Debug.Log("INTERACTING WITH FARMHOUSE");
            OnFarmhouseInteraction?.Invoke();
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        HideParticles();
    }

    private void OnEnable()
    {
        Lua.RegisterFunction("ShowFarmhouseParticles", this,
            SymbolExtensions.GetMethodInfo(() => ShowParticles()));
        Lua.RegisterFunction("HideFarmhouseParticles", this,
            SymbolExtensions.GetMethodInfo(() => HideParticles()));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("ShowFarmhouseParticles");
        Lua.UnregisterFunction("HideFarmhouseParticles");
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
