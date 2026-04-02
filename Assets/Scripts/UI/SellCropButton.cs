using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellCropButton : UIButton
{
    public int cropID;
    private EconomyManager economyManager;
    public AudioClip sellSound;
    public AudioClip uiFailure;

    public override void Awake()
    {
        economyManager = FindAnyObjectByType<EconomyManager>();
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        isSelected = false;
        image.sprite = normalSprite;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlightSprite;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (acceptingInput)
        {
            if (economyManager.SellHarvestedCrops(cropID))
            {
                SoundManager.Instance.PlaySound(sellSound);
            }
            else
            {
                SoundManager.Instance.PlaySound(uiFailure);
            }
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = normalSprite;
    }
}
