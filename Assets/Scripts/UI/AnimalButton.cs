using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image unitImage;
    public TextMeshProUGUI priceText;

    public int entityID;

    public void UpdateButton(int id)
    {
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(id);

        unitImage.sprite = info.sprite;
        priceText.text = info.purchasePrice.ToString();
        nameText.text = info.entityName;
    }

    public void UpdateButton()
    {
        UpdateButton(entityID);
    }
}
