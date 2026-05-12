using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimalButton : UIButton
{
    BarnUIMenu barnUI;
    public TextMeshProUGUI nameText;
    public Image unitImage;
    public TextMeshProUGUI priceText;
    public AnimalUISpriteDatabase database;

    public int entityID;

    public override void Awake()
    {
        base.Awake();
        barnUI = parentUI as BarnUIMenu;
    }

    public void UpdateButton(int id)
    {
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(id);

        unitImage.sprite = info.sprite;
        priceText.text = info.purchasePrice.ToString();
        nameText.text = info.entityName;
        entityID = id;

        if (database == null) return;

        AnimalUISpriteDatabase.AnimalSpriteSet spriteSet = database.GetSpriteSet(id);

        if (spriteSet != null)
        {
            normalSprite = spriteSet.normalSprite;
            highlightSprite = spriteSet.highlightSprite;
            unavailableSprite = spriteSet.unavailableSprite;

            UpdateVisual();
        }
    }

    //Remove the click functionality of the base class
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (acceptingInput)
        {
            parentUI.SetSelectedIndex(index);
        }
    }

    //Remove the hover functionality of the base class
    public override void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void UpdateSpriteSet(Sprite[] sprites, int offset)
    {
        normalSprite = sprites[offset];
        highlightSprite = sprites[offset+1];
        unavailableSprite = sprites[offset+2];
        UpdateVisual();
    }
}
