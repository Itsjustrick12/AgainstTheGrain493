using UnityEngine;
using UnityEngine.EventSystems;

//Need to call awake to find parent Naviagatable UI
public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private int index = 0;
    protected NaviagatableUI parentUI;
    public bool acceptingInput = true;

    public virtual void Awake()
    {
        parentUI = GetComponentInParent<NaviagatableUI>();
    }

    public virtual void Initialize(int buttonIndex)
    {
        index = buttonIndex;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (acceptingInput)
        {
            parentUI.SetSelectedIndex(index);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (acceptingInput)
        {
            parentUI.ReportAction();
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
}
