using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public TileHelper tileHelper;
    [SerializeField] FeedManager feedManager;
    private PickCropUI cropPicker; 
    public Tilemap optionsMap;

    public Tilemap arrowMap;
    public TileBase arrowTile;
    public AIManager aiManager;
    public TileBase optionTile;
    //Stores the location of the current tile selected
    [SerializeField]private ActionMenu actionMenu;

    public InteractionState state;

    //Get reference to the unit/location we are currently interacting with
    public Vector3Int selectedPosition;

    //Needed for reseting movement with cancel action
    public Vector3Int prevLocation;
    public Vector3Int afterLocation;
    public Vector3Int lastLocation;

    public Entity selectedEntity;

    private Vector3 offset = new Vector3(0.5f, 0.75f, 0);

    //Change this later but for now default Unity UI interactions is good
    private AgainstTheGrainInput input;

    [SerializeField] List<Vector3Int> validLocations;
    private EntityAction currAction;

    public bool isInputOn = true;

    public UnitInfoPanel infoPanel;
    public bool isFeeding = false;
    public bool isMoving = false;

    private Stack<InteractionState> stateHistory = new Stack<InteractionState>();

    public static event System.Action<InteractionState> OnStateChanged;

    //Used to reverse movement path for clean walkback
    private List<Vector3Int> lastMovePath = new List<Vector3Int>();

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        tileHelper = FindFirstObjectByType<TileHelper>();
        cropPicker = FindFirstObjectByType<PickCropUI>();
        aiManager = FindFirstObjectByType<AIManager>();
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
            //Only select valid tiles when there a valid tiles to check
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
        if(GetCurrentTile() != lastLocation && state == InteractionState.Movement)
        {
            SetArrow();
        }
        lastLocation = GetCurrentTile();
        Vector3Int pos = GetCurrentTile();
        Entity potentialEntity = tileManager.GetTileDataAt(pos).GetOccupyingEntity();
        if(selectedEntity == null && potentialEntity != null)
        {
            Unit unit = potentialEntity as Unit;
            if (unit != null) { 
            
                infoPanel.ShowPanel(unit);
            }
            else
            {
                infoPanel.HidePanel();
            }
            //Debug.Log("There is an entity Here");
        }
        else
        {
            infoPanel.HidePanel();
        }
    }

    public void SetArrow()
    {
        arrowMap.ClearAllTiles();

        //Debug.Log("SetArrow");
        List<Vector3Int> path = tileHelper.TilePath(selectedEntity.GetGridPos(), GetCurrentTile(), selectedEntity as Unit);

        // Prepend current position so the full path including origin is stored
        List<Vector3Int> fullPath = new List<Vector3Int>();
        fullPath.Add(selectedEntity.GetGridPos());
        fullPath.AddRange(path);
        lastMovePath = fullPath;

        if (path.Count > 1)
        {
            for(int i = 0; i < path.Count; i++)
            {
                arrowMap.SetTile(path[i], arrowTile);
            }
        }

    }

    //Modify state with this to keep track of history for undo functionality
    //State should rarely be modified directly to preserve state history for the undo function
    private void PushState(InteractionState newState)
    {
        stateHistory.Push(state);
        state = newState;
        OnStateChanged?.Invoke(state);
    }
    //Attempt to find an entity on the current tile
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
    //This is called the second the user clicks their mouse, so the logic has to be really tight
    public void OnSelect(InputAction.CallbackContext context)
    {
        Vector3Int pos = GetCurrentTile();
        if (isMoving) return;
        switch (state)
        {
            case InteractionState.Selection:
                //Don't accept inputs while feeding a unit
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
                    AskEndTurn();

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
                        //switch to interaction selection phase
                        ShowUnitOptions(toData.GetOccupyingEntity());

                        //Needed for the cancel action to work
                        prevLocation = selectedPosition;
                        afterLocation = pos;
                        SoundManager.Instance.PlayEntitySound(selectedEntity, SoundType.PLACE);
                        return;
                    }
                    //Only place if entity can go to new tile
                    else if (toData.CanPlaceEntity() && IsInRange(pos))
                    {
                        //tileManager.MoveEntity(selectedPosition, pos);
                        prevLocation = selectedPosition;
                        afterLocation = pos;
                        Unit unitCheck = selectedEntity as Unit;
                        if (unitCheck == null) return;

                        StartCoroutine(WaitForMoveAndShowOptions(unitCheck, toData));
                        return;
                    }
                }
                else
                {
                    //Place the unit back or just dont do anything
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

    private IEnumerator WaitForMoveAndShowOptions(Unit unit, TileData toData)
    {
        DisableInputs();
        Debug.Log("WaitForMoveAndShowOptions");
        isMoving = true;
        //grab the path and store incase we undo
        //move along path
        yield return StartCoroutine(unit.Move(lastMovePath));

        yield return new WaitUntil(() => !unit.isMoving);
        isMoving = false;

        arrowMap.ClearAllTiles();
        EnableInputs();
        ShowUnitOptions(unit);
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

    private void ShowUnitOptions(Unit unit)
    {
        if (unit == null)
        {
            //Debug.LogError("THERES NO UNIT TO SHOW OPTIONS FOR");
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
        Unit unit = selectedEntity as Unit;
        if(unit != null && unit.isMoving)
        {
            return;
        }
        if (stateHistory.Count == 0)
        {
            //can't undo if there's no history
            return;
        }
        //get the previous state type then do the appropriate undo action to get to that state
        InteractionState previous = stateHistory.Pop();
        switch (state)
        {

            case InteractionState.Movement:
                //clear movement tiles and deselect the unit
                optionsMap.ClearAllTiles();
                validLocations.Clear();
                arrowMap.ClearAllTiles();
                selectedEntity = null;
                selectedPosition = new Vector3Int(0, 0, 1);
                break;
            case InteractionState.ActionSelection:
                actionMenu.HideMenu();
                if (afterLocation != prevLocation)
                {
                    //tileManager.MoveEntity(afterLocation, prevLocation);
                    isMoving = true;
                    Debug.Log("UndoAction");
                    tileManager.MoveEntity(afterLocation, prevLocation);
                    isMoving = false;
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
                else if (isFeeding)
                {
                    // Player backed out of crop picker during feed flow
                    feedManager.CancelFeed();
                    cropPicker.CancelPicking(); // safe � CancelPicking guards !picking internally
                    selectedEntity = null;
                    isFeeding = false;
                    ResetData();
                    return; // ResetData sets state to Selection, skip the state = previous line
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
                //Clean up via the PickCropUI class and ensure all the UIs are closed properly
                cropPicker.OnCropSelected.RemoveListener(OnPlantSelected);
                cropPicker.CancelPicking();
                feedManager.CancelFeed();
                ResetData();
                //Set flag back to false to re-enable functionality
                isFeeding = false;
                break;
            default:
                break;
        }
        state = previous;
        OnStateChanged?.Invoke(state);
    }
    //Regular stop, just cancel the action and move if you need to
    public void StopAction()
    {
        if (afterLocation != prevLocation
            && afterLocation != new Vector3Int(0, 0, -1)
            && prevLocation != new Vector3Int(0, 0, -1))
        {
            Unit unit = tileManager.GetUnitOnTile(afterLocation);
            if (unit != null)
            {
                tileManager.MoveEntity(afterLocation, prevLocation);
            }
        }
        ResetData();
    }
    //desync'd walk back undo ONLY USED by CANCEL
    private IEnumerator StopActionRoutine()
    {
        if (afterLocation != prevLocation
            && afterLocation != new Vector3Int(0, 0, -1)
            && prevLocation != new Vector3Int(0, 0, -1))
        {
            Unit unit = tileManager.GetUnitOnTile(afterLocation);

            if (unit != null)
            {
                DisableInputs();
                isMoving = true;
                //Reverse the arrow path for consistency purposes instead of computing the path again
                List<Vector3Int> reversedPath = new List<Vector3Int>(lastMovePath);
                reversedPath.Reverse();
                yield return StartCoroutine(unit.Move(reversedPath));
                //Wait for animation to finish
                yield return new WaitUntil(() => !unit.isMoving);

                EnableInputs();
            }
        }//reset flag in case their is no move back
        isMoving = false;
        ResetData();
    }

    private void ResetData()
    {
        stateHistory.Clear();
        optionsMap.ClearAllTiles();
        arrowMap.ClearAllTiles();

        validLocations.Clear();

        selectedEntity = null;
        selectedPosition = new Vector3Int(0, 0, -1);
        prevLocation = new Vector3Int(0, 0, -1);
        afterLocation = new Vector3Int(0, 0, -1);
        currAction = null;

        // Also reset state to Selection to prevent stale state issues
        state = InteractionState.Selection;
        OnStateChanged?.Invoke(state);
    }

    public void SelectAction()
    {
        SelectAction(currAction);
    }
    //Once an action is reported from the Action Menu UI script, handle the logic to perform the action
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
            isMoving = true;
            StartCoroutine(StopActionRoutine());
            return;
        }
        else if (action is EndTurnAction)
        {

            //Undo the movement from the previous action and return
            GameManager.Instance.BeginEnemyTurn();
            ResetData();
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
        //Determine where the user needs to click
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

    public void AskEndTurn(InputAction.CallbackContext context)
    {
        if (state == InteractionState.Selection)
        {
            AskEndTurn();
        }
    }

    public void AskEndTurn()
    {
        selectedEntity = null;
        //Add the logic for pulling up end turn
        PushState(InteractionState.ActionSelection);
        actionMenu.ShowDefaultMenu();
    }

    //This method tries to see if there is a Unit that can be fed at the current tile
    // If there is, open the feed picker and switch state
    private void TryFeedAtPosition(Vector3Int pos)
    {
        Entity entity = tileManager.GetTileDataAt(pos)?.GetOccupyingEntity();
        Unit unit = entity as Unit;

        if (unit == null || unit.isEnemy)
        {
            Debug.Log("No valid unit to feed here");
            return;
        }
        //necessary boolean logic to get previous state or selection if there is no history
        state = stateHistory.Count > 0 ? stateHistory.Pop() : InteractionState.Selection;
        feedManager.OpenFeedUI(unit);

        //Show only the newly selected Unit using the selection tiles.
        optionsMap.ClearAllTiles();
        optionsMap.SetTile(unit.GetGridPos(), optionTile);

        // Push so undo can cancel out of the crop picker during feeding
        PushState(InteractionState.DecisionSelection);
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

    //Basically represents the UI intermediary state of needing to do something AFTER an action is selected
    //Currently necessary for planting and unit spawning due to needing to pick said crop or unit
    private void OnDecisionComplete()
    {
        if (stateHistory.Count > 0)
            stateHistory.Pop(); // remove DecisionSelection from history
        //With additional decision made, select where to spawn crop/unit
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
        //need economy manager to be able to feed
        if (!EconomyManager.Instance.HasACrop()) return;

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
        input.Gameplay.AdvanceTurn.performed += AskEndTurn;

    }

    private void OnDisable()
    {
        input.Gameplay.Select.performed -= OnSelect;
        input.Gameplay.Feed.performed -= ShowFeedOptions;
        input.Gameplay.Cancel.performed -= UndoAction;
        input.Gameplay.AdvanceTurn.performed -= AskEndTurn;
        input.Disable();

        //prevent bugs on scene transitions
        FeedManager.OnFeedingComplete -= StopFeeding;
    }

    public void DisableInputs()
    {
        isInputOn = false;
        hoverMap.ClearAllTiles();
        input.Disable();
    }

    public void EnableInputs()
    {
        isInputOn = true;
        input.Enable();
    }
}
