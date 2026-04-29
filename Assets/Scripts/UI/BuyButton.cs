using UnityEngine;
using UnityEngine.EventSystems;

public class BuyButton : UIButton
{

    private void OnEnable()
    {
        isSelected = false;
        image.sprite = normalSprite;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (acceptingInput)
        {
            isSelected = true;
            image.sprite = highlightSprite;
        }
    }

    public void UpdateVisual(int price)
    {
        bool canAfford = EconomyManager.Instance.GetCoins() >= price;
        acceptingInput = canAfford;
        image.sprite = canAfford ? normalSprite : unavailableSprite;
        //UpdateVisual();
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
            parentUI.ReportAction();
        }
    }

}
