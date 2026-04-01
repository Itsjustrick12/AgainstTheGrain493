using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
[System.Serializable]
public class IntEvent : UnityEvent<int> { }

//Right now only interface with the wheat crop
public class BarnUIMenu : NaviagatableUI
{
    public EconomyManager econManager;
    //public TextMeshProUGUI grainText;
    public TextMeshProUGUI coinText;

    public GameObject uiPanel;

    public GameObject animalButtonObj;

    public static IntEvent OnUnitPurchased = new IntEvent();
    public static UnityEvent OnPurchaseComplete = new UnityEvent();
    public static UnityEvent CancelAction = new UnityEvent();

    public Transform buttonLayout;
    public int[] animalList;

    public BuyButton buyButton;

    public Image unitPreview;

    private void OnDisable()
    {
        Barn.OnBarnInteraction -= ShowMenu;
    }

    public void Awake()
    {
        Barn.OnBarnInteraction += ShowMenu;
    }

    public void Start()
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
            button.Initialize(buttons.Count-1);
        }
        
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
        btn.SetSelected(true);
    }

    public void ShowMenu()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
        SetSelectedIndex(0);
        UpdateCropText(econManager.GetHarvestedCrops(1));
        UpdateCoinText(econManager.GetCoins());
        TurnOnInput();
        buyButton.SetAcceptingInput(true);
    }

    public void HideMenu()
    {
        TurnOffInput();
        buyButton.SetAcceptingInput(false);
        uiPanel.SetActive(false);
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
            OnUnitPurchased.Invoke(id);
            Debug.Log("Purchased unit: " + info.entityName);

            OnPurchaseComplete.Invoke();
            HideMenu();
        }
        
    }

    public override void ReportAction()
    {
        base.ReportAction();
        //Do something here in the derived class
        BuyUnit(buttons[selectedChoice].GetComponent<AnimalButton>().entityID);
    }
}
