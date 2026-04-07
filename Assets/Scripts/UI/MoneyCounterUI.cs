using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

//Abstract class used by all scaling UI boxes for updating the counters via script
public abstract class ExpandingCounterUI : MonoBehaviour
{
    public RectTransform counterBar;
    public TextMeshProUGUI numberText;

    public UINumber[] digits;

    public float baseWidth = 32f;
    //For every digit, add this amount to the width to scale it up
    public float digitWidth = 32f;

    protected int currentAmount = 0;
    [SerializeField] protected bool iconOnlyMode = false;

    protected abstract int GetCounterValue();

    public virtual void UpdateCounter()
    {
        UpdateCounter(GetCounterValue());
    }

    public virtual void UpdateCounter(int newValue)
    {
        currentAmount = Mathf.Max(0, newValue);
        //Basically, don't show the number and shrink to just the icon
        if (iconOnlyMode)
        {
            counterBar.sizeDelta = new Vector2(baseWidth, counterBar.sizeDelta.y);
            foreach (var digit in digits) digit.gameObject.SetActive(false);
            if (numberText != null) numberText.gameObject.SetActive(false);
            return;
        }

        string valueStr = currentAmount.ToString();
        int digitCount = valueStr.Length;

        // Resize the bar
        Vector2 size = counterBar.sizeDelta;
        size.x = baseWidth + digitCount * digitWidth;
        counterBar.sizeDelta = size;

        // Update digit sprites
        for (int i = 0; i < digits.Length; i++)
        {
            if (i < digitCount)
            {
                digits[i].gameObject.SetActive(true);

                int digit = valueStr[i] - '0';
                digits[i].UpdateDigit(digit);
            }
            else
            {
                digits[i].gameObject.SetActive(false);
            }
        }

        // Disable TMP text if you're using sprite digits instead
        if (numberText != null)
        {
            numberText.gameObject.SetActive(false);
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
    public void SetIconOnly(bool value)
    {
        iconOnlyMode = value;
        //Hide or show the buttons
        UpdateCounter();
    }
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
