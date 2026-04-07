using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FeedButton : UIButton
{
    private UnitInteractionSystem interactionSystem;

    public override void Awake()
    {
        image = GetComponent<Image>();
        interactionSystem = FindFirstObjectByType<UnitInteractionSystem>();
    }

    private void OnEnable()
    {
        isSelected = false;
        UnitInteractionSystem.OnStateChanged += CanInteract;
        EconomyManager.OnCropChanged += DetermineAvailability;
        UpdateVisual(); // sync on enable in case state already changed
    }

    private void OnDisable()
    {
        UnitInteractionSystem.OnStateChanged -= CanInteract;
        EconomyManager.OnCropChanged -= DetermineAvailability;
    }

    private void CanInteract(InteractionState newState)
    {
        acceptingInput = (newState == InteractionState.Selection) && EconomyManager.Instance.HasACrop();
        UpdateVisual();
    }

    private void DetermineAvailability(int cropID)
    {
        acceptingInput = EconomyManager.Instance.HasACrop();
        UpdateVisual();
    }

    public override void UpdateVisual()
    {
        image.sprite = acceptingInput ? normalSprite : unavailableSprite;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (acceptingInput)
            image.sprite = highlightSprite;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (acceptingInput)
            interactionSystem.StartFeedTargeting();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (acceptingInput)
            image.sprite = normalSprite;
        else
            image.sprite = unavailableSprite;
    }
}

