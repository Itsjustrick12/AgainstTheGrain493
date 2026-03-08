using TMPro;
using UnityEngine;
using UnityEngine.Windows;

//Abstract class used by all scaling UI boxes for updating the counters via script
public abstract class ExpandingCounterUI : MonoBehaviour
{
    public RectTransform counterBar;
    public TextMeshProUGUI numberText;

    public float baseWidth = 32f;
    //For every digit, add this amount to the width to scale it up
    public float digitWidth = 32f;

    protected int currentAmount = 0;

    protected abstract int GetCounterValue();

    public virtual void UpdateCounter()
    {
        UpdateCounter(GetCounterValue());
    }

    public virtual void UpdateCounter(int newValue)
    {
        currentAmount = Mathf.Max(0, newValue);

        int digits = currentAmount.ToString().Length;

        //Resize the bar
        Vector2 size = counterBar.sizeDelta;
        //Assume base width includes a single digit
        size.x = baseWidth + (digits - 1) * digitWidth;
        counterBar.sizeDelta = size;
        //Update text
        if (numberText != null)
        {
            numberText.text = currentAmount.ToString();
        }
    }

    protected virtual void OnEnable()
    {
        SubscribeEvents();
    }

    public void Start()
    {
        UpdateCounter();
    }

    protected virtual void OnDisable()
    {
        UnsubscribeEvents();
    }

    protected abstract void SubscribeEvents();
    protected abstract void UnsubscribeEvents();
}
//Used for tracking the coins in the economy manager
public class MoneyCounterUI : ExpandingCounterUI
{
    protected override int GetCounterValue()
    {
        return EconomyManager.Instance.GetCoins();
    }

    protected override void SubscribeEvents()
    {
        EconomyManager.OnCoinsChanged += UpdateCounter;
    }

    protected override void UnsubscribeEvents()
    {
        EconomyManager.OnCoinsChanged -= UpdateCounter;
    }
}
