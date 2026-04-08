using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class UnitInfoPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup currentCanvas;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI movementText;

    [SerializeField] RectTransform healthBarMask;
    [SerializeField] RectTransform healthBar;
    [SerializeField] float healthBarWidth;

    [SerializeField] Image unitImage;

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

        Vector2 canvasPosition = Camera.main.WorldToScreenPoint(currUnit.GetGridPos() + offset);
        this.transform.position = canvasPosition;

        PopulatePanel(currUnit);

        //Debug.Log(unitName);

    }

    public void HidePanel()
    {
        currentCanvas.alpha = 0;
        currentCanvas.interactable = false;
        currentCanvas.blocksRaycasts = false; //Prevents things from behind it being clicked
    }

    public void PopulatePanel(Unit currUnit)
    {

        UnitInfo info = UnitDatabase.Instance.GetUnitInfo(currUnit.ID);
        string unitName = info.entityName;
        int currentHealth = currUnit.GetHealth();
        int maxHealth = info.baseHealth;
        int strength = currUnit.GetStrength();
        int moveRange = currUnit.GetMoveRange();

        nameText.text = unitName;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        strengthText.text = strength.ToString();
        movementText.text = moveRange.ToString();

        int baseStat = 0;

        baseStat = info.baseHealth;
        if (currentHealth > baseStat)
        {
            int valDif = currentHealth - baseStat;
            healthText.text = currentHealth.ToString() + " (" + valDif + ")" + "/" + maxHealth.ToString();
            healthText.color = Color.green;
        }
        else
        {
            healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
            healthText.color = Color.black;
        }

        baseStat = info.strength;
        if (strength != baseStat)
        {
            Debug.Log("BaseStat: " + baseStat);
            Debug.Log("Strength: " + strength);
            int valDif = strength - baseStat;
            //Debug.Log("Diff: " + valDif);
            if(valDif > 0)
            {
                strengthText.text = strength.ToString() + " ( +" + valDif + ")";
                strengthText.color = Color.green;
            }
            else
            {
                strengthText.text = strength.ToString() + " ( -" + valDif + ")";
                strengthText.color = Color.red;
            }
            
        }
        else
        {
            strengthText.text = strength.ToString();
            strengthText.color = Color.black;
        }

        baseStat = info.moveRange;
        if (moveRange != baseStat)
        {
            int valDif = moveRange - baseStat;
            if (valDif > 0)
            {
                movementText.text = moveRange.ToString() + " ( +" + valDif + ")";
                movementText.color = Color.green;
            }
            else
            {
                movementText.text = moveRange.ToString() + " ( -" + valDif + ")";
                movementText.color = Color.red;
            }
            
        }
        else
        {
            movementText.text = moveRange.ToString();
            movementText.color = Color.black;
        }

        unitImage.sprite = info.sprite;
        UpdateHealthBar(currentHealth, maxHealth);
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarMask == null) return;

        healthBarWidth = healthBar.rect.width;

        float ratio = (maxHealth > 0) ? (float)currentHealth / maxHealth : 0f;
        ratio = Mathf.Clamp01(ratio); //Clamp so buffs don't make it stretch

        // Scale the fill by adjusting its width while keeping the left edge anchored
        healthBarMask.sizeDelta = new Vector2(healthBarWidth * ratio, healthBarMask.sizeDelta.y);
    }
}