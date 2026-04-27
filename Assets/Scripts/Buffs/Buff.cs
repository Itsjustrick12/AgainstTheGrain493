using UnityEngine;

//Should be slapped onto anything can can be buffed
public interface IBuffable
{
    void AddBuff(Buff buff);
    void RemoveBuff(Buff buff);
    void ClearBuffs();
}

public abstract class Buff
{
    protected int remainingTurns;
    Entity target;

    public Buff(int duration)
    {
        remainingTurns = duration;
    }

    public void Apply(Entity target)
    {
        this.target = target;
        GameManager.StartPlayerTurn += Tick;
        OnApply();
    }

    public void Tick()
    {
        OnTurnTick();

        remainingTurns--;

        if (remainingTurns <= 0)
        {
            Expire();
        }
    }

    public virtual void Expire()
    {
        target.RemoveBuff(this);
        GameManager.StartPlayerTurn -= Tick;
        OnExpire();
    }

    protected virtual void OnApply() { }
    protected virtual void OnTurnTick() { }
    public virtual void OnExpire() {
        if (target is Unit unit)
        {
            unit.SetIsFed(false);
        }
    }

}
