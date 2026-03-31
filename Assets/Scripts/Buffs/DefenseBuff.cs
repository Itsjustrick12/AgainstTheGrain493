using JetBrains.Annotations;
using UnityEngine;

public class DefenseBuff : Buff
{
    public int baseIncrease = 0;
    public float multiplier = 1;

    public DefenseBuff(int duration, int baseAmount, float mult = 1) : base(duration)
    {
        baseIncrease = baseAmount;
        multiplier = mult;
    }
}
