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

    [Header("Sprites")]

    public int entityID;

    public override void Awake()
    {
        base.Awake();

        jobBoardUI = parentUI as JobBoardUI;
    }

    public void UpdateButton(int id)
    {
        UnitInfo info =
            UnitDatabase.Instance.GetUnitInfo(id);

        if (info == null)
            return;

        entityID = id;
        workerImage.sprite = info.sprite;
        nameText.text = info.entityName;
        wageText.text = info.purchasePrice.ToString();

    }

    // Select worker listing
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!acceptingInput)
            return;

        parentUI.SetSelectedIndex(index);
    }

    // Disable hover auto-selection
    public override void OnPointerEnter(PointerEventData eventData)
    {

    }


    public void SetUnavailable(bool unavailable)
    {
        acceptingInput = !unavailable;
    }
}