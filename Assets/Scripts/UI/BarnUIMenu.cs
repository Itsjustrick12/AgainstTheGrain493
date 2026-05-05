using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows;
[System.Serializable]
public class IntEvent : UnityEvent<int> { }

//Right now only interface with the wheat crop
public class BarnUIMenu : NaviagatableUI
{
    public EconomyManager econManager;
    //public TextMeshProUGUI grainText;
    public TextMeshProUGUI coinText;

    public GameObject animalButtonObj;

    public static IntEvent OnUnitPurchased = new IntEvent();
    public static UnityEvent CancelAction = new UnityEvent();

    public Transform buttonLayout;
    public int[] animalList;

    public Transform sellButtonLayout;
    public List<SellCropButton> sellCropButtons;

    public BuyButton buyButton;
    public Image unitPreview;

    [SerializeField] private StatDisplay[] stats;

    public void Awake()
    {
        econManager = EconomyManager.Instance;
        buyButton.SetAcceptingInput(false);
        //Create enough buttons for all the units specified in the list for the shop
        foreach (int id in animalList)
        {
            //Create a button and place it under the button layout group
            GameObject obj = Instantiate(animalButtonObj, buttonLayout);
            AnimalButton button = obj.GetComponent<AnimalButton>();
            buttons.Add(obj);
            button.UpdateButton(id);
            button.Initialize(buttons.Count - 1);
        }
        sellCropButtons = new List<SellCropButton>();
        List<int> crops = GameManager.Instance.GetCropIDs();
        foreach (Transform child in sellButtonLayout)
        {
            SellCropButton button = child.gameObject.GetComponent<SellCropButton>();
            if ( button != null)
            {
                if (crops.Contains(button.cropID))
                {
                    button.gameObject.SetActive(true);
                    sellCropButtons.Add(button);
                }
                //Hide the buttons if the crop isn't in the level
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnEnable()
    {
        EconomyManager.OnCoinsChanged += RefreshVisuals;
    }

    public void OnDisable()
    {
        EconomyManager.OnCoinsChanged -= RefreshVisuals;
    }

    public override void DeselectButton(int index)
    {
        base.DeselectButton(index);
        AnimalButton btn = buttons[index].GetComponent<AnimalButton>();
        btn.SetSelected(false);
    }

    public override void SelectButton(int index)
    {
        base.SelectButton(index);
        AnimalButton btn = buttons[index].GetComponent<AnimalButton>();
        unitPreview.sprite = UnitDatabase.Instance.GetSprite(btn.entityID);

        //Set the UI info based on the unit's stats

        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(btn.entityID);

        if (info != null)
        {
            foreach (StatDisplay display in stats)
            {
                display.gameObject.SetActive(true);
            }
            //fill in stat info
            stats[0].SetText(info.strength);
            stats[1].SetText(info.moveRange);
            stats[2].SetText(info.baseHealth);
        }
        else
        {
            foreach (StatDisplay display in stats)
            {
                display.gameObject.SetActive(false);
            }
        }

        btn.SetSelected(true);

        //Update the buy button based on if the new selected index can be purchased
        UpdateButtons();
    }

    public void ShowMenu()
    {
        this.gameObject.SetActive(true);
        SetSelectedIndex(0);

        UpdateCropText(econManager.GetHarvestedCrops(1));
        UpdateCoinText(econManager.GetCoins());
        TurnOnInput();
        //Update Buttons
        UpdateButtons();

    }

    public void HideMenu()
    {
        TurnOffInput();
        buyButton.SetAcceptingInput(false);
        this.gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        CancelAction?.Invoke();
        HideMenu();
    }

    public void UpdateCropText(int grainAmount)
    {
        //grainText.text = grainAmount.ToString();
    }

    public void RefreshVisuals(int coinAmount)
    {
        UpdateCoinText(coinAmount);
        UpdateButtons();
    }

    public void UpdateCoinText(int coinAmount)
    {
        coinText.text = coinAmount.ToString();
    }

    public void SellCrop(int id)
    {
        if (econManager.SellHarvestedCrops(id)){
            UpdateCropText(econManager.GetHarvestedCrops(id));
            UpdateCoinText(econManager.GetCoins());
        }
        else
        {
            Debug.Log("You have nothing to sell");
        }
    }

    public void BuyUnit(int id)
    {
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(id);
        if (info != null && econManager.AttemptToBuy(info.purchasePrice))
        {
            //Set the unit to spawn, close the menu, and await spawn
            UpdateCoinText(econManager.GetCoins());
            HideMenu();
            OnUnitPurchased.Invoke(id);
            Debug.Log("Purchased unit: " + info.entityName);
        }
        
    }

    public override void ReportAction()
    {
        base.ReportAction();
        //Do something here in the derived class
        BuyUnit(buttons[selectedChoice].GetComponent<AnimalButton>().entityID);
    }

    public void UpdateButtons()
    {
        foreach (SellCropButton button in sellCropButtons)
        {
            button.UpdateVisual();
        }

        //get reference to the current unit and see if we can buy it
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(buttons[selectedChoice].GetComponent<AnimalButton>().entityID);

        buyButton.UpdateVisual(info != null ? info.purchasePrice : int.MaxValue);
    }
}
