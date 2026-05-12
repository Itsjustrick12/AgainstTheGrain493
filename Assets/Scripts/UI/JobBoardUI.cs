using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JobBoardUI : MonoBehaviour
{
    public EconomyManager econManager;

    [Header("Currency")]
    public TextMeshProUGUI coinText;

    [Header("Farmer Listings")]
    public GameObject hireCardObj;
    public Transform buttonLayout;
    [SerializeField] protected List<GameObject> buttons;

    public int[] farmerList;

    public List<SellCropButton> sellCropButtons;

    public static IntEvent OnFarmerHired = new IntEvent();
    public static UnityEvent CancelAction = new UnityEvent();

    private void Awake()
    {
        econManager = EconomyManager.Instance;

        foreach (int id in farmerList)
        {
            GameObject obj = Instantiate(hireCardObj, buttonLayout);
            HireButton button = obj.GetComponentInChildren<HireButton>();
            buttons.Add(obj);
            button.UpdateButton(id);
            button.Initialize(buttons.Count - 1);
        }
    }

    private void OnEnable()
    {
        EconomyManager.OnCoinsChanged += RefreshVisuals;
    }

    private void OnDisable()
    {
        EconomyManager.OnCoinsChanged -= RefreshVisuals;
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
        RefreshVisuals(econManager.GetCoins());
        StartCoroutine(EnableInputNextFrame());
        SetButtonsAcceptingInput(false);
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        CancelAction?.Invoke();
        HideMenu();
    }

    public void HireFarmer(int id)
    {
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(id);

        if (info != null && econManager.AttemptToBuy(info.purchasePrice))
        {
            UpdateCoinText(econManager.GetCoins());
            HideMenu();
            OnFarmerHired.Invoke(id);
            Debug.Log("Hired farmer: " + info.entityName);
        }
    }

    public void UpdateCoinText(int coinAmount)
    {
        coinText.text = coinAmount.ToString();
    }

    public void UpdateSellButtons()
    {
        foreach (SellCropButton button in sellCropButtons)
        {
            button.UpdateVisual();
        }
    }

    public void RefreshVisuals(int coinAmount)
    {
        UpdateCoinText(coinAmount);

        foreach (GameObject obj in buttons)
        {
            HireButton btn = obj.GetComponentInChildren<HireButton>();
            UnitInfo info = UnitDatabase.Instance.GetUnitInfo(btn.entityID);
            if (info != null)
                btn.UpdateVisual(info.purchasePrice);
        }
        UpdateSellButtons();
    }

    private IEnumerator EnableInputNextFrame()
    {
        yield return null; // wait one frame for the opening click to clear
        SetButtonsAcceptingInput(true);
    }

    private void SetButtonsAcceptingInput(bool accepting)
    {
        foreach (GameObject obj in buttons)
        {
            HireButton btn = obj.GetComponent<HireButton>();
            if (btn != null)
                btn.acceptingInput = accepting;
        }
    }
}