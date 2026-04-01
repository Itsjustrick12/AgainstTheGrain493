using TMPro;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class IntEvent : UnityEvent<int> { }

//Right now only interface with the wheat crop
public class BarnUIMenu : MonoBehaviour
{
    public AnimalButton[] buttons;
    public EconomyManager econManager;
    //public TextMeshProUGUI grainText;
    public TextMeshProUGUI coinText;

    public GameObject uiPanel;

    public static IntEvent OnUnitPurchased = new IntEvent();
    public static UnityEvent OnPurchaseComplete = new UnityEvent();
    public static UnityEvent CancelAction = new UnityEvent();

    private void OnDisable()
    {
        Barn.OnBarnInteraction -= ShowMenu;
    }

    public void Awake()
    {
        econManager = EconomyManager.Instance;
        Barn.OnBarnInteraction += ShowMenu;
    }

    public void Start()
    {
        foreach (AnimalButton button in buttons)
        {
            button.UpdateButton();
        }
        
    }

    public void ShowMenu()
    {
        Barn.OnBarnInteraction += ShowMenu;
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
        UpdateCropText(econManager.GetHarvestedCrops(1));
        UpdateCoinText(econManager.GetCoins());
        
    }

    public void HideMenu()
    {
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
}
