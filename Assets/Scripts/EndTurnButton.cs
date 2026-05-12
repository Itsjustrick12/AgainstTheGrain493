using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndTurnButton : UIButton
{
    private UnitInteractionSystem interactionSystem;
    private GameManager gameManager;

    public override void Awake()
    {
        image = GetComponent<Image>();
        gameManager = FindFirstObjectByType<GameManager>();
        interactionSystem = FindFirstObjectByType<UnitInteractionSystem>();
    }

    private void OnEnable()
    {
        isSelected = false;
        UnitInteractionSystem.OnStateChanged += CanInteract;
        GameManager.StartEnemyTurn += CanInteract;
        GameManager.StartPlayerTurn += CanInteract;
        CanInteract();
    }

    private void OnDisable()
    {
        UnitInteractionSystem.OnStateChanged -= CanInteract;
        GameManager.StartEnemyTurn -= CanInteract;
        GameManager.StartPlayerTurn -= CanInteract;
    }

    private void CanInteract()
    {
        acceptingInput = gameManager.isPlayerTurn
                      && interactionSystem.state == InteractionState.Selection;
        UpdateVisual();
    }

    private void CanInteract(InteractionState newState)
    {
        acceptingInput = gameManager.isPlayerTurn
                      && newState == InteractionState.Selection;
        UpdateVisual();
    }

    public override void UpdateVisual()
    {
        image.sprite = acceptingInput ? normalSprite : unavailableSprite;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (acceptingInput)
            image.sprite = highlightSprite;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (acceptingInput)
            interactionSystem.AskEndTurn();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        image.sprite = acceptingInput ? normalSprite : unavailableSprite;
    }
}