using PixelCrushers.DialogueSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
using static UnityEngine.RuleTile.TilingRuleOutput;
using PixelCrushers.DialogueSystem;
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

    [SerializeField] private Color normalColor;
    [SerializeField] private Color buffColor;
    [SerializeField] private Color debuffColor;

    [SerializeField] private Vector3 offset = new Vector3(2.5f, 0, 0);

    //used for swapping image to different background for each unit type
    [SerializeField] private Sprite farmerUI;
    [SerializeField] private Sprite animalUI;
    [SerializeField] private Sprite robotUI;
    [SerializeField] private Image image;

    private void OnEnable()
    {
        GameManager.StartEnemyTurn += HidePanel;
    }

    private void OnDisable()
    {
        GameManager.StartEnemyTurn -= HidePanel;
    }

    public void ShowPanel(Unit currUnit)
    {
        if (!GameManager.Instance.isPlayerTurn) return;
        if (DialogueManager.IsConversationActive) return;
        if (currUnit == null)
        {
            //Debug.Log("Null Check");
            return;
        }

        currentCanvas.alpha = 1;
        currentCanvas.interactable = true;
        currentCanvas.blocksRaycasts = true; //Prevents things from behind it being clicked

        switch (currUnit.GetEntityType())
        {
            case EntityType.Farmer:
                image.sprite = farmerUI;
                break;
            case EntityType.Enemy:
                image.sprite = robotUI;
                break;
            case EntityType.Animal:
                image.sprite = animalUI;
                break;
            default:
                break;
        }

        MovePanel(currUnit); 

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
            healthText.color = buffColor;
        }
        else
        {
            healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
            healthText.color = normalColor;
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
                strengthText.color = buffColor;
            }
            else
            {
                strengthText.text = strength.ToString() + " ( " + valDif + ")";
                strengthText.color = debuffColor;
            }
            
        }
        else
        {
            strengthText.text = strength.ToString();
            strengthText.color = normalColor;
        }

        baseStat = info.moveRange;
        if (moveRange != baseStat)
        {
            int valDif = moveRange - baseStat;
            if (valDif > 0)
            {
                movementText.text = moveRange.ToString() + " ( +" + valDif + ")";
                movementText.color = buffColor;
            }
            else
            {
                movementText.text = moveRange.ToString() + " ( " + valDif + ")";
                movementText.color = debuffColor;
            }
            
        }
        else
        {
            movementText.text = moveRange.ToString();
            movementText.color = normalColor;
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


    public void MovePanel(Unit currUnit)
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 size = rect.sizeDelta;

        Vector3 worldPos = currUnit.GetGridPos();

        // First try with normal offset
        Vector3 desiredWorldPos = worldPos + offset;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(desiredWorldPos);

        // Check horizontal bounds
        bool offRight = screenPos.x + size.x > Screen.width;
        // Flip offset if going off screen
        Vector3 finalOffset = offset;

        if (offRight)
        {
            finalOffset = new Vector3(offset.x*-1+1, offset.y, 0);
            screenPos = Camera.main.WorldToScreenPoint(worldPos + finalOffset);
        }

        rect.position = screenPos;
    }
}