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
        EconomyManager.OnCropChanged += UpdateVisual;
    }

    private void OnDisable()
    {
        EconomyManager.OnCropChanged -= UpdateVisual;
    }

    //Wrapper function
    public void UpdateVisual(int id)
    {
        if (id == cropID)
        {
            UpdateVisual();
        }
    }

    public override void UpdateVisual()
    {
        //Check if we have a crop to sell
        bool hasCrop = economyManager.GetHarvestedCrops(cropID) > 0;
        
        acceptingInput = hasCrop;

        image.sprite = hasCrop ? normalSprite : unavailableSprite;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (acceptingInput)
        {
            image.sprite = highlightSprite;
        }
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
        if (acceptingInput)
        {
            image.sprite = normalSprite;
        }
    }
}
