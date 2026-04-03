using JetBrains.Annotations;
using UnityEngine;

public class StrengthBuff : Buff
{
    public int baseIncrease = 0;
    public float multiplier = 1;

    public StrengthBuff(int duration, int baseAmount, float mult = 1) : base(duration)
    {
        baseIncrease = baseAmount;
        multiplier = mult;
    }
}
