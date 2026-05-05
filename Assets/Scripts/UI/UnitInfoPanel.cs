using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class UnitInfoPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup currentCanvas;

    [SerializeField] private List<TextMeshProUGUI> infoText;

    [SerializeField] RectTransform healthBarMask;
    [SerializeField] RectTransform healthBar;
    [SerializeField] float healthBarWidth;

    [SerializeField] Image unitImage;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color buffColor;
    [SerializeField] private Color debuffColor;

    [SerializeField] private Vector3 offset = new Vector3(2.5f, 0, 0);

    //used for swapping image to different background for each unit type
    [SerializeField] private List<Sprite> UIpanels;
    [SerializeField] private Image image;

    private void OnEnable()
    {
        GameManager.StartEnemyTurn += HidePanel;
    }

    private void OnDisable()
    {
        GameManager.StartEnemyTurn -= HidePanel;
    }

    public void ShowPanel(Entity entity)
    {
        if (!GameManager.Instance.isPlayerTurn) return;
        if (entity == null)
        {
            //Debug.Log("Null Check");
            return;
        }

        currentCanvas.alpha = 1;
        currentCanvas.interactable = true;
        currentCanvas.blocksRaycasts = true; //Prevents things from behind it being clicked

        //sets the background sprite depending on the entity type
        switch (entity.GetEntityType())
        {
            case EntityType.Farmer:
                image.sprite = UIpanels[0];
                break;
            case EntityType.Crop:
                image.sprite = UIpanels[1];
                break;
            case EntityType.Enemy:
                image.sprite = UIpanels[2];
                break;
            case EntityType.Animal:
                image.sprite = UIpanels[3];
                break;
            case EntityType.Structure:
                image.sprite = UIpanels[4];
                break;
            default:
                break;
        }

        MovePanel(entity); 

        PopulatePanel(entity);

        //Debug.Log(unitName);

    }

    public void HidePanel()
    {
        currentCanvas.alpha = 0;
        currentCanvas.interactable = false;
        currentCanvas.blocksRaycasts = false; //Prevents things from behind it being clicked
    }

    public void PopulatePanel(Entity entity)
    {
        /*
            infoText[] guide
            in general:
                0 is the name
                1 is the health info
        */

        //first hide everything
        foreach TextMeshProUGUI text in infoText)
        {
            text.gameObject.SetActive(false);
            text.text = "";
        }

        //grab the general info first
        EntityInfo info;
        if(entity as Unit != null) info = UnitDatabase.Instance.GetUnitInfo((entity as Unit).GetEntityID());
        else if(entity as Crop != null) info = CropDatabase.Instance.GetCropInfo((entity as Crop).GetEntityID());
        else info = StructureDatabase.Instance.GetStructureInfo((entity as Structure).GetEntityID());
        //name
        string name = info.entityName;
        infoText[0].text = name;

        //hp
        int maxHealth = info.baseHealth;
        int currentHealth = entity.GetCurrentHealth();

        //set the health text
        int baseStat = info.baseHealth;
        if (currentHealth > baseStat)
        {
            int valDif = currentHealth - baseStat;
            infoText[1].text = currentHealth.ToString() + " (" + valDif + ")" + "/" + maxHealth.ToString();
            infoText[1].color = buffColor;
        }
        else
        {
            infoText[1].text = currentHealth.ToString() + "/" + maxHealth.ToString();
            infoText[1].color = normalColor;
        }

        //grabbing unit specific info
        if(entity as Unit != null)
        {
            /*
                for Units:
                    2 is strength
                    3 is movement range
            */
            Unit currUnit = entity as Unit;

            //strength
            baseStat = (info as UnitInfo).strength;
            int strength = currUnit.GetStrength();
            if (strength != baseStat)
            {
                Debug.Log("BaseStat: " + baseStat);
                Debug.Log("Strength: " + strength);
                int valDif = strength - baseStat;
                //Debug.Log("Diff: " + valDif);
                if(valDif > 0)
                {
                    infoText[2].text = strength.ToString() + " ( +" + valDif + ")";
                    infoText[2].color = buffColor;
                }
                else
                {
                    infoText[2].text = strength.ToString() + " ( -" + valDif + ")";
                    infoText[2].color = debuffColor;
                }
                
            }
            else
            {
                infoText[2].text = strength.ToString();
                infoText[2].color = normalColor;
            }

            //movement range
            int moveRange = currUnit.GetMoveRange();
            baseStat = (info as UnitInfo).moveRange;
            if (moveRange != baseStat)
            {
                int valDif = moveRange - baseStat;
                if (valDif > 0)
                {
                    infoText[3].text = moveRange.ToString() + " ( +" + valDif + ")";
                    infoText[3].color = buffColor;
                }
                else
                {
                    infoText[3].text = moveRange.ToString() + " ( -" + valDif + ")";
                    infoText[3].color = debuffColor;
                }
                
            }
            else
            {
                infoText[3].text = moveRange.ToString();
                infoText[3].color = normalColor;
            }


        }
        else if(entity as Crop != null)
        {
            /*
                for Crops:
                    2 is turns till harvest
            */
            Crop currCrop = entity as Crop;
            //baseStat is being used for the # of stages total
            baseStat = (info as CropInfo).numStages;
            int currStage = currCrop.GetCurrentStage();
            baseStat -= currStage;
            
            infoText[2].text = baseStat.ToString();
            infoText[2].color = normalColor;

        }

        //show what's updated
        foreach(TextMeshProUGUI text in infoText)
        {
            if(text.text != "")
            {
                text.gameObject.SetActive(true);
            }
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


    public void MovePanel(Entity entity)
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 size = rect.sizeDelta;

        Vector3 worldPos = entity.GetGridPos();

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