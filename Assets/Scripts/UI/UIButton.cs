using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Need to call awake to find parent Naviagatable UI
public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    protected int index = 0;
    protected NaviagatableUI parentUI;
    public bool acceptingInput = true;

    public Sprite normalSprite;
    public Sprite highlightSprite;
    public Sprite unavailableSprite;

    public bool isSelected = false;
    public bool available = true;

    protected Image image;

    public virtual void Awake()
    {
        parentUI = GetComponentInParent<NaviagatableUI>();
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    public virtual void Initialize(int buttonIndex)
    {
        index = buttonIndex;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (DialogueManager.IsConversationActive)
        {
            return;
        }
        if (acceptingInput)
        {
            parentUI.SetSelectedIndex(index);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (DialogueManager.IsConversationActive)
        {
            return;
        }
        if (acceptingInput)
        {
            parentUI.ReportAction();
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (DialogueManager.IsConversationActive)
        {
            return;
        }
    }

    public void SetAcceptingInput(bool value)
    {
        acceptingInput = value;
    }

    public void TurnOffInput()
    {
        acceptingInput = false;
    }

    public void TurnOnInput()
    {
        acceptingInput = true;
    }

    public virtual void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisual();
    }

    public virtual void SetAvailable(bool value)
    {
        available = value;
        UpdateVisual(); // default to not selected
    }

    public virtual void UpdateVisual()
    {
        if (isSelected)
        {
            image.sprite = highlightSprite;
        }
        else if (!available)
        {
            image.sprite = unavailableSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
    }

}
