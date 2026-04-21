using PixelCrushers.DialogueSystem;
using static UnityEngine.CullingGroup;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    //[SerializeField] private TutorialDialogueTrigger dialogueTrigger;
    [SerializeField] private bool tutorialEnabled = true;

    public static TutorialManager Instance;

    private bool robotMoved = false;

    private bool cropPlanted = false;
    private bool cropWatered = false;
    private bool cropGrown = false;
    private bool cropHarvested = false;

    private bool cropSold = false;
    private bool chickenPurchased = false;
    private bool enemyHit = false;

    private void OnEnable()
    {
        UnitInteractionSystem.OnUnitSelected += OnUnitClicked;
        BasicHarvestAction.onHarvest += OnCropHarvested;
        BasicWaterAction.onWater += OnCropWatered;
        PlantAction.onPlant += OnCropPlanted;
        GameManager.StartPlayerTurn += OnCropGrow;
        //UnitInteractionSystem.OnUnitMoved += OnUnitClicked;
        SpawnUnitAction.OnSpawn += OnChickenPurchased;
        GameManager.StartEnemyTurn += OnRobotMove;

        Unit.OnFriendlyDie += OnFriendlyDie;
        Unit.OnAnimalDie += OnAnimalDie;
        Unit.OnEnemyHit += OnEnemyHit;
    }

    private void OnDisable()
    {
        UnitInteractionSystem.OnUnitSelected -= OnUnitClicked;
        BasicHarvestAction.onHarvest -= OnCropHarvested;
        BasicWaterAction.onWater -= OnCropWatered;
        PlantAction.onPlant -= OnCropPlanted;
        GameManager.StartPlayerTurn -= OnCropGrow;
        //UnitInteractionSystem.OnUnitMoved += OnUnitClicked;
        SpawnUnitAction.OnSpawn -= OnChickenPurchased;
        GameManager.StartEnemyTurn -= OnRobotMove;

        Unit.OnFriendlyDie -= OnFriendlyDie;
        Unit.OnAnimalDie -= OnAnimalDie;
        Unit.OnEnemyHit -= OnEnemyHit;
    }

    private void Awake()
    {
        Instance = this;
        if (!tutorialEnabled)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void OnUnitClicked()
    {
        DialogueManager.StartConversation("Tutorial/FirstUnitClick");
        UnitInteractionSystem.OnUnitSelected -= OnUnitClicked;
    }

    public void OnRobotMove()
    {
        if (cropGrown)
        {
            robotMoved = true;
            DialogueManager.StartConversation("Tutorial/OnRobotMove");
            GameManager.StartEnemyTurn -= OnRobotMove;
        }
    }

    public void OnCropPlanted()
    {
        cropPlanted = true;
        DialogueManager.StartConversation("Tutorial/OnPlant");
        PlantAction.onPlant -= OnCropPlanted;
    }

    public void OnCropGrow()
    {
        if (cropWatered)
        {
            cropGrown = true;
            DialogueManager.StartConversation("Tutorial/OnCropGrow");
            GameManager.StartPlayerTurn -= OnCropGrow;
        }
    }

    public void OnCropWatered()
    {
        cropWatered = true;
        DialogueManager.StartConversation("Tutorial/OnWater");
        BasicWaterAction.onWater -= OnCropWatered;
    }

    public void OnCropHarvested()
    {
        cropHarvested = true;
        DialogueManager.StartConversation("Tutorial/OnHarvest");
        BasicHarvestAction.onHarvest -= OnCropHarvested;
    }

    public void OnChickenPurchased()
    {
        chickenPurchased = true;
        DialogueManager.StartConversation("Tutorial/OnChickenPurchased");
        SpawnUnitAction.OnSpawn -= OnChickenPurchased;
    }

    public void OnEnemyHit()
    {
        enemyHit = true;
        DialogueManager.StartConversation("Tutorial/EnemyHit");
        Unit.OnEnemyHit -= OnEnemyHit;
    }

    public void OnFriendlyDie()
    {
        DialogueManager.StartConversation("Tutorial/OnFriendlyDie");
        Unit.OnFriendlyDie -= OnFriendlyDie;
    }

    public void OnAnimalDie()
    {
        DialogueManager.StartConversation("Tutorial/OnAnimalDie");
        Unit.OnAnimalDie -= OnAnimalDie;
    }

}