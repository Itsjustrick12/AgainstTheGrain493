using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Handles the farmer hiring job board UI
public class JobBoardUI : NaviagatableUI
{
    public EconomyManager econManager;

    [Header("Currency")]
    public TextMeshProUGUI coinText;

    [Header("Farmer Listings")]
    public GameObject farmerButtonObj;
    public Transform buttonLayout;

    // List of hireable farmer unit IDs
    public int[] farmerList;

    [Header("Preview")]
    public Image unitPreview;
    public HireButton hireButton;

    [Header("Stats")]
    [SerializeField] private StatDisplay[] stats;

    public static IntEvent OnFarmerHired = new IntEvent();
    public static UnityEvent CancelAction = new UnityEvent();

    private void Awake()
    {
        econManager = EconomyManager.Instance;

        hireButton.SetAcceptingInput(false);

        // Generate farmer listing buttons
        foreach (int id in farmerList)
        {
            GameObject obj = Instantiate(farmerButtonObj, buttonLayout);

            AnimalButton button = obj.GetComponent<AnimalButton>();

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

        SetSelectedIndex(0);

        UpdateCoinText(econManager.GetCoins());

        TurnOnInput();

        UpdateButtons();
    }

    public void HideMenu()
    {
        TurnOffInput();

        hireButton.SetAcceptingInput(false);

        gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        CancelAction?.Invoke();

        HideMenu();
    }

    public override void SelectButton(int index)
    {
        base.SelectButton(index);

        AnimalButton btn = buttons[index].GetComponent<AnimalButton>();

        btn.SetSelected(true);

        unitPreview.sprite =
            UnitDatabase.Instance.GetSprite(btn.entityID);

        UnitInfo info =
            UnitDatabase.Instance.GetUnitInfo(btn.entityID);

        if (info != null)
        {
            foreach (StatDisplay display in stats)
            {
                display.gameObject.SetActive(true);
            }

            // Example farmer stats
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

        UpdateButtons();
    }

    public override void DeselectButton(int index)
    {
        base.DeselectButton(index);

        AnimalButton btn =
            buttons[index].GetComponent<AnimalButton>();

        btn.SetSelected(false);
    }

    public override void ReportAction()
    {
        base.ReportAction();

        HireFarmer(
            buttons[selectedChoice]
            .GetComponent<AnimalButton>()
            .entityID
        );
    }

    public void HireFarmer(int id)
    {
        UnitInfo info =
            UnitDatabase.Instance.GetUnitInfo(id);

        if (info != null &&
            econManager.AttemptToBuy(info.purchasePrice))
        {
            UpdateCoinText(econManager.GetCoins());

            HideMenu();

            OnFarmerHired.Invoke(id);

            Debug.Log("Hired farmer: " + info.entityName);
        }
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

    public void UpdateButtons()
    {
        if (buttons.Count == 0)
            return;

        HireButton btn = buttons[selectedChoice].GetComponent<HireButton>();

        btn.UpdateButton(1);
    }
}