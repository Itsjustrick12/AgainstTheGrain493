using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private bool tutorialEnabled = true;

    [Header("Conversation Keys")]
    [SerializeField] private string onUnitClickedConversation = "Tutorial/FirstUnitClick";
    [SerializeField] private string onRobotMoveConversation = "Tutorial/OnRobotMove";
    [SerializeField] private string onCropPlantedConversation = "Tutorial/OnPlant";
    [SerializeField] private string onCropGrowConversation = "Tutorial/OnCropGrow";
    [SerializeField] private string onCropWateredConversation = "Tutorial/OnWater";
    [SerializeField] private string onCropHarvestedConversation = "Tutorial/OnHarvest";
    [SerializeField] private string onChickenPurchasedConversation = "Tutorial/OnChickenPurchased";
    [SerializeField] private string onEnemyHitConversation = "Tutorial/EnemyHit";
    [SerializeField] private string onFriendlyDieConversation = "Tutorial/OnFriendlyDie";
    [SerializeField] private string onAnimalDieConversation = "Tutorial/OnAnimalDie";

    [Header("Trigger Conditions")]
    [SerializeField] private bool requireCropGrownBeforeRobotMove = true;
    [SerializeField] private bool requireCropWateredBeforeGrow = true;

    [SerializeField] private ObjectiveUI mainObjectiveBox;
    [SerializeField] private ObjectiveUI stepObjectiveBox;

    private bool robotMoved = false;
    private bool cropPlanted = false;
    private bool cropWatered = false;
    private bool cropGrown = false;
    private bool cropHarvested = false;
    private bool cropSold = false;
    private bool chickenPurchased = false;
    private bool enemyHit = false;

    private void Awake()
    {
        Instance = this;
        if (!tutorialEnabled)
        {
            gameObject.SetActive(false);
            return;
        }

        mainObjectiveBox.SetObjective("Kill that rustbolt");
        stepObjectiveBox.SetObjective("Click on a farmer");
    }

    private void OnEnable()
    {
        UnitInteractionSystem.OnUnitSelected += OnUnitClicked;
        BasicHarvestAction.onHarvest += OnCropHarvested;
        BasicWaterAction.onWater += OnCropWatered;
        PlantAction.onPlant += OnCropPlanted;
        GameManager.StartPlayerTurn += OnCropGrow;
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
        SpawnUnitAction.OnSpawn -= OnChickenPurchased;
        GameManager.StartEnemyTurn -= OnRobotMove;
        Unit.OnFriendlyDie -= OnFriendlyDie;
        Unit.OnAnimalDie -= OnAnimalDie;
        Unit.OnEnemyHit -= OnEnemyHit;
    }

    private void TryStartConversation(string key)
    {
        if (!string.IsNullOrEmpty(key))
            DialogueManager.StartConversation(key);
    }

    public void OnUnitClicked()
    {
        TryStartConversation(onUnitClickedConversation);

        stepObjectiveBox.SetObjective("Move farmer onto dirt and plant wheat");

        UnitInteractionSystem.OnUnitSelected -= OnUnitClicked;
    }

    public void OnRobotMove()
    {
        if (requireCropGrownBeforeRobotMove && !cropGrown) return;
        robotMoved = true;
        TryStartConversation(onRobotMoveConversation);
        stepObjectiveBox.SetObjective("Harvest the Wheat");
        GameManager.StartEnemyTurn -= OnRobotMove;
    }

    public void OnCropPlanted()
    {
        cropPlanted = true;
        TryStartConversation(onCropPlantedConversation);
        stepObjectiveBox.SetObjective("Move farmer near seeds and water them");
        PlantAction.onPlant -= OnCropPlanted;
    }

    public void OnCropGrow()
    {
        if (requireCropWateredBeforeGrow && !cropWatered) return;
        cropGrown = true;
        TryStartConversation(onCropGrowConversation);
        stepObjectiveBox.SetObjective("Water the crops again");
        GameManager.StartPlayerTurn -= OnCropGrow;
    }

    public void OnCropWatered()
    {
        cropWatered = true;
        TryStartConversation(onCropWateredConversation);
        stepObjectiveBox.SetObjective("End the Turn");
        BasicWaterAction.onWater -= OnCropWatered;
    }

    public void OnCropHarvested()
    {
        cropHarvested = true;
        TryStartConversation(onCropHarvestedConversation);
        stepObjectiveBox.SetObjective("Sell Wheat at barn & buy chicken");
        BasicHarvestAction.onHarvest -= OnCropHarvested;
    }

    public void OnChickenPurchased()
    {
        chickenPurchased = true;
        TryStartConversation(onChickenPurchasedConversation);
        stepObjectiveBox.SetObjective("Purchase another chicken and attack the tinhead");
        SpawnUnitAction.OnSpawn -= OnChickenPurchased;
    }

    public void OnEnemyHit()
    {
        enemyHit = true;
        TryStartConversation(onEnemyHitConversation);
        stepObjectiveBox.SetObjective("Kill that bot");
        Unit.OnEnemyHit -= OnEnemyHit;
    }

    public void OnFriendlyDie()
    {
        TryStartConversation(onFriendlyDieConversation);
        stepObjectiveBox.SetObjective("Bruh");
        Unit.OnFriendlyDie -= OnFriendlyDie;
    }

    public void OnAnimalDie()
    {
        TryStartConversation(onAnimalDieConversation);
        stepObjectiveBox.SetObjective("Bruh");
        Unit.OnAnimalDie -= OnAnimalDie;
    }
}