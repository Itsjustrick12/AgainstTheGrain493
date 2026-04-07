using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public enum InteractionState
{
    Selection,
    Movement,
    ActionSelection,
    DecisionSelection,
    FeedTargeting,
    TargetSelection,
    Execution
}

//This is needed to call events and recieve callbacks from UI buttons
[System.Serializable]
public class EntityActionEvent : UnityEvent<EntityAction>
{
}

//This script is responsible for handling the interactions between units
//This includes movement, crop interactions, attacking, and more
public class UnitInteractionSystem : TileCursor
{
    public TileManager tileManager;
    [SerializeField] FeedManager feedManager;
    private PickCropUI cropPicker; 
    public Tilemap optionsMap;
    public TileBase optionTile;
    //Stores the location of the current tile selected
    [SerializeField]private ActionMenu actionMenu;

    public InteractionState state;

    //Get reference to the unit/location we are currently interacting with
    public Vector3Int selectedPosition;

    //Needed for reseting movement with cancel action
    public Vector3Int prevLocation;
    public Vector3Int afterLocation;

    public Entity selectedEntity;

    public Transform hoverTransform;
    public SpriteRenderer hoverSprite;

    private Vector3 offset = new Vector3(0.5f, 0.75f, 0);

    //Change this later but for now default Unity UI interactions is good
    private AgainstTheGrainInput input;

    [SerializeField] List<Vector3Int> validLocations;
    private EntityAction currAction;

    public bool isInputOn = true;

    public bool isFeeding = false;

    private Stack<InteractionState> stateHistory = new Stack<InteractionState>();

    public static event System.Action<InteractionState> OnStateChanged;


    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        cropPicker = FindFirstObjectByType<PickCropUI>();
        //actionMenu = FindFirstObjectByType<ActionMenu>();
        actionMenu.OnActionSelected.AddListener(SelectAction);
        PushState(InteractionState.Selection);
        BarnUIMenu.OnUnitPurchased.AddListener(OnUnitSelected);
        BarnUIMenu.CancelAction.AddListener(StopAction);
        FeedManager.OnFeedingComplete += StopFeeding;
        feedManager = FindFirstObjectByType<FeedManager>();
        validLocations = new List<Vector3Int>();
    }
    //Restrict to only display updated tiles
    protected override void HandleCursor()
    {
        if (!isInputOn) return;

        Vector3Int tile = GetMouseTile();

        if (showHighlight && tile != currentTile && targetMap.HasTile(tile))
        {
            if ((validLocations.Count==0) || (validLocations.Count > 0 && IsInRange(tile)))
            {
                DeselectLast();
                currentTile = tile;
                SelectCurrent(tile);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        //Move the hoverSprite to the currently selected location
        hoverTransform.position = GetCurrentTile() + offset;
    }

    //Modify state with this to keep track of history for undo functionality
    private void PushState(InteractionState newState)
    {
        stateHistory.Push(state);
        state = newState;
        OnStateChanged?.Invoke(state);
    }

    public bool AttemptSelection(Vector3Int pos)
    {
         
        TileData data = tileManager.GetTileDataAt(pos);

        if (data == null || !data.HasOccupant())
        {
            return false;
        }

        selectedPosition = pos;
        selectedEntity = data.GetOccupyingEntity();
        if (selectedEntity != null && selectedEntity.IsActive())
        {

            return true;
        }
        return false;
    }

    //Transitions through the state machine to determine what action to take
    public void OnSelect(InputAction.CallbackContext context)
    {
        Vector3Int pos = GetCurrentTile();
        switch (state)
        {
            case InteractionState.Selection:
                if (isFeeding) return;
                //Attempt to set the selected Unit on the tile
                if (AttemptSelection(pos))
                {
                    //If above is true, get reference to entity and switch to movement phase
                    selectedPosition = pos;
                    selectedEntity = tileManager.GetTileDataAt(pos).GetOccupyingEntity();
                    if (selectedEntity != null && selectedEntity is Unit)
                    {
                        Unit unit = selectedEntity as Unit;
                        if (unit.isEnemy) return;
                        validLocations = unit.GetMovementRange();
                        
                        LiftHoverSprite();
                        foreach(Vector3Int tile in validLocations)
                        {
                            optionsMap.SetTile(tile, optionTile);
                        }
                        SoundManager.Instance.PlayEntitySound(unit, SoundType.SELECT);
                        PushState(InteractionState.Movement);
                        return;
                    }
                    else if (selectedEntity != null && selectedEntity is Structure)
                    {
                        //do structure interaction, skip movement interaction
                        Structure structureCheck = selectedEntity as Structure;
                        List<EntityAction> actions = structureCheck.GetAvailableActions();

                        // Store the SpawnUnitAction as currAction so SelectAction can use it later
                        // when OnPurchaseComplete fires after the player picks a unit in the UI
                        if (actions != null && actions.Count > 0)
                        {
                            currAction = actions[0]; // assumes first action is SpawnUnitAction
                            PushState(InteractionState.ActionSelection);
                            structureCheck.Interact();
                        }
                    }
                }
                else
                {
                    selectedEntity = null;
                }
                break;
            case InteractionState.Movement:
                TileData fromData = tileManager.GetTileDataAt(selectedPosition);
                TileData toData = tileManager.GetTileDataAt(pos);
                //if selected tile, try to place the unit at the current location to see if it works
                if (selectedEntity != null && toData != null)
                {
                    optionsMap.ClearAllTiles();
                    //check they if they're the same
                    if (fromData == toData)
                    {
                        //Just place at same place and move on
                        ClearHoverSprite();
                        //switch to interaction selection phase
                        ShowUnitOptions(toData.GetOccupyingEntity());

                        //Needed for the cancel action to work
                        prevLocation = selectedPosition;
                        afterLocation = pos;
                        SoundManager.Instance.PlayEntitySound(selectedEntity, SoundType.PLACE);
                        return;
                    }
                    //Only place if entity can go to new tile
                    else if (toData.CanPlaceEntity())
                    {
                        ClearHoverSprite();
                        tileManager.MoveEntity(selectedPosition, pos);

                        //For now just switch to selection
                        ShowUnitOptions(toData.GetOccupyingEntity());
                        //Needed for the cancel action to work
                        prevLocation = selectedPosition;
                        afterLocation = pos;
                        SoundManager.Instance.PlayEntitySound(selectedEntity, SoundType.PLACE);
                        return;
                    }
                }
                else
                {
                    //Place the unit back or just dont do anything
                    ClearHoverSprite();
                    state = InteractionState.Selection;
                    OnStateChanged?.Invoke(state);

                }
                break;
            case InteractionState.TargetSelection:
                if (!currAction.IsAOE() && AttemptTarget(pos))
                {
                    state = InteractionState.Selection;
                    OnStateChanged?.Invoke(state);
                }
                break;
            case InteractionState.FeedTargeting:
                // Try to feed unit
                TryFeedAtPosition(pos);
                break;
            default:
                break;
        }
    }

    //Call upon selected a unit in movement
    public void LiftHoverSprite()
    {
        if (selectedEntity!= null)
        {
            selectedEntity.HideSprite();
            hoverSprite.sprite = selectedEntity.GetSprite();
        }

    }

    public bool IsInRange(Vector3Int pos)
    {
        return validLocations.Contains(pos);
    }

    public bool AttemptTarget(Vector3Int pos)
    {
        if (currAction == null) return false;
        //Check that the position you clicked is a valid target
        if (validLocations.Count > 0 && validLocations.Contains(pos))
        {
            //Execute the action
            //Need to remain general
            currAction.PerformAt(selectedEntity as Entity, pos);
            selectedEntity.Deactivate();
            ResetData();
            return true;
        }
        return false; 
    }

    //Clear before placement
    public void ClearHoverSprite()
    {
        if (selectedEntity != null)
        {
            selectedEntity.ShowSprite();
        }
        hoverSprite.sprite = null;
    }

    private void ShowUnitOptions(Unit unit)
    {
        if (unit == null)
        {
            Debug.LogError("THERES NO UNIT TO SHOW OPTIONS FOR");
            return;
        }
        PushState(InteractionState.ActionSelection);
        actionMenu.ShowMenu(unit);
    }
    //used to allow player to undo the steps in the action state machine
    public void UndoAction(InputAction.CallbackContext context)
    {
        UndoAction();
    }
    public void UndoAction()
    {
        if (stateHistory.Count == 0)
        {
            //can't undo if there's no history
            return;
        }
        //get the previous state type then do the appropriate undo action to get to that state
        InteractionState previous = stateHistory.Pop();
        Unit unit = selectedEntity as Unit;
        switch (state)
        {

            case InteractionState.Movement:
                //clear movement tiles and deselect the unit
                optionsMap.ClearAllTiles();
                validLocations.Clear();
                ClearHoverSprite();
                selectedEntity = null;
                selectedPosition = new Vector3Int(0, 0, 1);
                break;
            case InteractionState.ActionSelection:
                actionMenu.HideMenu();
                if (afterLocation != prevLocation)
                {
                    tileManager.MoveEntity(afterLocation, prevLocation);
                    afterLocation = prevLocation;
                }
                //show movement highlights
                if (unit != null)
                {
                    //show movement range
                    validLocations = unit.GetMovementRange();
                    foreach (Vector3Int pos in validLocations)
                    {
                        optionsMap.SetTile(pos, optionTile);
                    }
                }
                LiftHoverSprite();
                break;
            case InteractionState.DecisionSelection:
                // Close whichever decision UI is open
                if (currAction is PlantAction)
                {
                    cropPicker.OnCropSelected.RemoveListener(OnPlantSelected);
                    cropPicker.CancelPicking(); // whatever your hide method is
                }
                else if (currAction is SpawnUnitAction)
                {
                    // Barn cancel is already handled via CancelAction event
                }
                currAction = null;
                // Reopen action menu for the unit
                if (unit != null) actionMenu.ShowMenu(unit);
                break;
            case InteractionState.TargetSelection:
                //clear target highlights then go back to action selection
                optionsMap.ClearAllTiles();
                validLocations.Clear();
                currAction = null;
                if (unit != null)
                {
                    actionMenu.ShowMenu(unit);
                }
                break;
            case InteractionState.FeedTargeting:
                // Just go back, nothing to clean up
                cropPicker.OnCropSelected.RemoveListener(OnPlantSelected);
                cropPicker.CancelPicking();
                feedManager.CancelFeed();
                selectedEntity = null;

                ResetData();
                isFeeding = false;
                break;
            default:
                break;
        }
        state = previous;
        OnStateChanged?.Invoke(state);
    }

    public void StopAction()
    {

        if (afterLocation != prevLocation && (afterLocation != null && prevLocation != null))
        {
            tileManager.MoveEntity(afterLocation, prevLocation);
        }

        ResetData();
    }

    private void ResetData()
    {
        stateHistory.Clear();
        optionsMap.ClearAllTiles();
        validLocations.Clear();

        ClearHoverSprite(); // call BEFORE nulling selectedEntity
        selectedEntity = null;
        selectedPosition = new Vector3Int(0, 0, -1);
        currAction = null;

        // Also reset state to Selection to prevent stale state issues
        state = InteractionState.Selection;
        OnStateChanged?.Invoke(state);
    }

    public void SelectAction()
    {
        SelectAction(currAction);
    }

    public void SelectAction(EntityAction action)
    {
        if (isFeeding) return;

        //Check for unique UI ish actions for not doing direct things
        if (action == null)
        {
            Debug.LogError("NO ACTION SELECTED!");
        }
        //once this is called, shift to tile selection based on target tiles
        if (action is WaitAction)
        {
            //Don't do anything, consider the unit moved and don't do anything else
            state = InteractionState.Selection;
            OnStateChanged?.Invoke(state);
            selectedEntity.Deactivate();
            ResetData();
            return;
        }
        else if (action is Cancel)
        {

            //Undo the movement from the previous action and return
            StopAction();
            return;
        }
        //For planting action, you must first determine which seed to plant via the UI
        if (action is PlantAction)
        {
            currAction = action;
            PushState(InteractionState.DecisionSelection);
            cropPicker.OnCropSelected.AddListener(OnPlantSelected);
            cropPicker.StartPicking(false);
            return;
        }
        if (action is SpawnUnitAction)
        {
            currAction = action;
            PushState(InteractionState.DecisionSelection);
            // Barn UI is already opened via structure interaction,
            // OnPurchaseComplete fires OnDecisionComplete when done
            return;
        }


        currAction = action;

        GetTargets();
    }
    //Get the availible targets from the current action, then switch to selecting one
    private void GetTargets()
    {
        validLocations = currAction.GetValidTargets(selectedEntity);
        //Highlight the selectable locations
        foreach (Vector3Int pos in validLocations)
        {
            optionsMap.SetTile(pos, optionTile);
        }
        //Otherwise, perform the action on the selected tile
        PushState(InteractionState.TargetSelection);
    }

    public void StartFeedTargeting()
    {
        // Reset any prior selection cleanly
        ResetData();

        validLocations = new List<Vector3Int>();
        List<Unit> units = GameManager.Instance.GetAllFriendlyUnits(); ;
        //get position for each friendly unit
        foreach (Unit unit in units)
        {
            if (!unit.isFed)
            {

                validLocations.Add(unit.GetGridPos());
                optionsMap.SetTile(unit.GetGridPos(), optionTile);
            }
        }

        PushState(InteractionState.FeedTargeting);
        isFeeding = true;
    }

    private void TryFeedAtPosition(Vector3Int pos)
    {
        Entity entity = tileManager.GetTileDataAt(pos)?.GetOccupyingEntity();
        Unit unit = entity as Unit;

        if (unit == null || unit.isEnemy)
        {
            Debug.Log("No valid unit to feed here");
            // Stay in FeedTargeting so the player can try again,
            // or cancel with the undo button
            return;
        }

        //selectedEntity = unit;
        // Pop FeedTargeting before opening feed UI so undo works cleanly
        state = stateHistory.Count > 0 ? stateHistory.Pop() : InteractionState.Selection;
        feedManager.OpenFeedUI(unit);
        optionsMap.ClearAllTiles();
        optionsMap.SetTile(unit.GetGridPos(), optionTile);
    }

    //Additional picker step needed for the plant action after crop is picked from UI
    private void OnPlantSelected(int cropID)
    {
        //Remove this listner now that the plant has been picked
        cropPicker.OnCropSelected.RemoveListener(OnPlantSelected);
        //actually set the index to plant
        PlantAction plantAct = currAction as PlantAction;
        plantAct.SetCropID(cropID);
        //Debug.Log("PlantAct is linked with ID " + plantAct.cropID);
        //Get the availible targets from the current action, then switch to selecting one
        OnDecisionComplete();
    }
    private void OnUnitSelected(int unitID)
    {
        //actually set the index to plant
        SpawnUnitAction spawnAct = currAction as SpawnUnitAction;
        if (spawnAct != null)
        {
            spawnAct.SetSpawnUnit(unitID);
        }
        //Debug.Log("PlantAct is linked with ID " + plantAct.cropID);
        //Get the availible targets from the current action, then switch to selecting one
        OnDecisionComplete();
    }

    private void OnDecisionComplete()
    {
        if (stateHistory.Count > 0)
            stateHistory.Pop(); // remove DecisionSelection from history
        GetTargets();
    }

    private void ShowUnitOptions(Entity entity)
    {
        Unit unit = entity as Unit;
        if (unit == null)
        {
            Debug.LogError("This isn't a unit, can't show options");
            return;
        }
        ShowUnitOptions(unit);
    }

    private void ShowFeedOptions(InputAction.CallbackContext context)
    {
        // Already guards, but make this more explicit:
        if (state != InteractionState.Selection) return;
        StartFeedTargeting();
    }

    private void StopFeeding()
    {
        StopAction();
        isFeeding = false;
    }

    private void OnEnable()
    {
        input = new AgainstTheGrainInput();
        input.Enable();
        input.Gameplay.Select.performed += OnSelect;
        input.Gameplay.Feed.performed += ShowFeedOptions;
        input.Gameplay.Cancel.performed += UndoAction;
    }

    private void OnDisable()
    {
        input.Gameplay.Select.performed -= OnSelect;
        input.Gameplay.Feed.performed -= ShowFeedOptions;
        input.Gameplay.Cancel.performed -= UndoAction;
        input.Disable();

        //prevent bugs on scene transitions
        FeedManager.OnFeedingComplete -= StopFeeding;
    }

    public void DisableInputs()
    {
        isInputOn = false;
        input.Disable();
    }

    public void EnableInputs()
    {
        isInputOn = true;
        input.Enable();
    }
}
