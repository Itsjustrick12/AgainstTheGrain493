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
