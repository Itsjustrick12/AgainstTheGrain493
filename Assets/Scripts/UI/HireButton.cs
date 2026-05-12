using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HireButton : UIButton
{
    JobBoardUI jobBoardUI;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI wageText;
    public Image workerImage;

    public int entityID;

    public override void Awake()
    {
        base.Awake();
        jobBoardUI = FindFirstObjectByType<JobBoardUI>();
    }

    private void OnEnable()
    {
        isSelected = false;
        image.sprite = normalSprite;
    }

    public void UpdateButton(int id)
    {
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(id);
        if (info == null)
            return;

        entityID = id;
        workerImage.sprite = info.sprite;
        nameText.text = info.entityName;
        wageText.text = info.purchasePrice.ToString();

        UpdateVisual(info.purchasePrice);
    }

    public void UpdateVisual(int price)
    {
        bool canAfford = EconomyManager.Instance.GetCoins() >= price;
        acceptingInput = canAfford;
        image.sprite = canAfford ? normalSprite : unavailableSprite;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (acceptingInput)
        {
            isSelected = true;
            image.sprite = highlightSprite;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (acceptingInput)
        {
            isSelected = false;
            image.sprite = normalSprite;
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (acceptingInput && isSelected)
        {
            jobBoardUI.HireFarmer(entityID);
        }
    }
}