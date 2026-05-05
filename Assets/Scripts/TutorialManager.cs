using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    private bool robotMoved = false;
    private bool cropPlanted = false;
    private bool cropWatered = false;
    private bool cropGrown = false;
    private bool cropHarvested = false;
    private bool cropSold = false;
    private bool chickenPurchased = false;
    private bool enemyHit = false;

    [SerializeField] private Tilemap hoverMap;
    [SerializeField] private TileBase hoverTile;

    private void Awake()
    {
        Instance = this;
        if (!tutorialEnabled)
        {
            gameObject.SetActive(false);
            return;
        }
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
        Lua.RegisterFunction("HighlightTile", this,
        SymbolExtensions.GetMethodInfo(() => HighlightTileLua(0.0, 0.0)));
        Lua.RegisterFunction("ClearHoverTiles", this,
            SymbolExtensions.GetMethodInfo(() => ClearHoverTiles()));
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
        Lua.UnregisterFunction("HighlightTile");
        Lua.UnregisterFunction("ClearHoverTiles");
    }

    private void TryStartConversation(string key)
    {
        if (!string.IsNullOrEmpty(key))
            DialogueManager.StartConversation(key);
    }

    public void OnUnitClicked()
    {
        TryStartConversation(onUnitClickedConversation);
        UnitInteractionSystem.OnUnitSelected -= OnUnitClicked;
    }

    public void OnRobotMove()
    {
        if (requireCropGrownBeforeRobotMove && !cropGrown) return;
        robotMoved = true;
        TryStartConversation(onRobotMoveConversation);
        GameManager.StartEnemyTurn -= OnRobotMove;
    }

    public void OnCropPlanted()
    {
        cropPlanted = true;
        TryStartConversation(onCropPlantedConversation);
        PlantAction.onPlant -= OnCropPlanted;
    }

    public void OnCropGrow()
    {
        if (requireCropWateredBeforeGrow && !cropWatered) return;
        cropGrown = true;
        TryStartConversation(onCropGrowConversation);
        GameManager.StartPlayerTurn -= OnCropGrow;
    }

    public void OnCropWatered()
    {
        cropWatered = true;
        TryStartConversation(onCropWateredConversation);
        BasicWaterAction.onWater -= OnCropWatered;
    }

    public void OnCropHarvested()
    {
        cropHarvested = true;
        TryStartConversation(onCropHarvestedConversation);
        BasicHarvestAction.onHarvest -= OnCropHarvested;
    }

    public void OnChickenPurchased()
    {
        chickenPurchased = true;
        TryStartConversation(onChickenPurchasedConversation);
        SpawnUnitAction.OnSpawn -= OnChickenPurchased;
    }

    public void OnEnemyHit()
    {
        enemyHit = true;
        TryStartConversation(onEnemyHitConversation);
        Unit.OnEnemyHit -= OnEnemyHit;
    }

    public void OnFriendlyDie()
    {
        TryStartConversation(onFriendlyDieConversation);
        Unit.OnFriendlyDie -= OnFriendlyDie;
    }

    public void OnAnimalDie()
    {
        TryStartConversation(onAnimalDieConversation);
        Unit.OnAnimalDie -= OnAnimalDie;
    }

    public void HighlightTileLua(double x, double y)
    {
        HighlightTile(new Vector3Int((int)x, (int)y, 0));
    }

    public void HighlightTile(Vector3Int tile)
    {
        hoverMap.SetTile(tile, hoverTile);
    }

    public void ClearHoverTiles()
    {
        hoverMap.ClearAllTiles();
    }
}