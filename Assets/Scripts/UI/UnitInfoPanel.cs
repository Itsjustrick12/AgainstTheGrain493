using UnityEngine;
using System.Collections;
using TMPro;

public class UnitInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI movementText;
    [SerializeField] private CanvasGroup currentCanvas;


    public void ShowPanel(Unit currUnit)
    {
        currentCanvas.alpha = 1;
        currentCanvas.interactable = true;
        currentCanvas.blocksRaycasts = true; //Prevents things from behind it being clicked
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(currUnit.ID);
        string unitName = info.name;
        int currentHealth = currUnit.GetHealth();
        int maxHealth = info.baseHealth;
        int strength = info.strength;
        int moveRange = info.moveRange;

        nameText.text = unitName;
        healthText.text = "Health: " + currentHealth + " / " + maxHealth;
        strengthText.text = "Strength: " + strength;
        movementText.text = "Movement Range: " + moveRange;

    }

    public void HidePanel()
    {
        currentCanvas.alpha = 0;
        currentCanvas.interactable = false;
        currentCanvas.blocksRaycasts = false; //Prevents things from behind it being clicked
    }

}
