using UnityEngine;
using System.Collections;
using TMPro;

public class UnitInfoPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup currentCanvas;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI movementText;

    //[SerializeField] private Color normalColor;
    //[SerializeField] private Color buffColor;
    //[SerializeField] private Color debuffColor;

    [SerializeField] private Vector3 offset = new Vector3(2.5f, 0, 0);

    public void ShowPanel(Unit currUnit)
    {
        if (currUnit == null)
        {
            //Debug.Log("Null Check");
            return;
        }

        currentCanvas.alpha = 1;
        currentCanvas.interactable = true;
        currentCanvas.blocksRaycasts = true; //Prevents things from behind it being clicked
        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(currUnit.ID);
        string unitName = info.entityName;
        int currentHealth = currUnit.GetHealth();
        int maxHealth = info.baseHealth;
        int strength = currUnit.GetStrength();
        int moveRange = currUnit.GetMoveRange();

        Vector2 canvasPosition = Camera.main.WorldToScreenPoint(currUnit.GetGridPos()+offset);


        this.transform.position = canvasPosition;

        nameText.text = unitName;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        strengthText.text = strength.ToString();
        movementText.text = moveRange.ToString();



        //Debug.Log(unitName);

    }

    public void HidePanel()
    {
        currentCanvas.alpha = 0;
        currentCanvas.interactable = false;
        currentCanvas.blocksRaycasts = false; //Prevents things from behind it being clicked
    }

}