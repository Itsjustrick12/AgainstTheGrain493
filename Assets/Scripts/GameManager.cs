using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
public class GameManager : MonoBehaviour
{
    [Header("General Settings")]
    public TileManager tileManager;
    //Needed for pause logic
    private UnitInteractionSystem interactionSystem;
    public ActionMenu actionMenu;

    //Transforms for sorting
    public Transform friendlyUnits;
    public Transform enemyUnits;
    public Transform cropContainer;
    public Transform structureContainter;

    public Tilemap entityMap;
    private AgainstTheGrainInput input;

    public static event Action StartPlayerTurn;

    public GameObject pauseScreen;
    public GameObject winScreen;
    public GameObject loseScreen;
    
    public bool isPlayerTurn = true;
    public bool isPaused = false;

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();
        interactionSystem = FindFirstObjectByType<UnitInteractionSystem>();
        
    }

    public void Start()
    {
        //actionMenu = FindFirstObjectByType<ActionMenu>();
        isPlayerTurn = true;
        SpawnStartingUnits();
    }
    
    public void BeginEnemyTurn(InputAction.CallbackContext context)
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    public void BeginPlayerTurn()
    {
        isPlayerTurn = true;
        // Call this whenever a turn/day ends
        //Debug.Log("Turn advanced!");
        List<Unit> friendlies = GetAllFriendlyUnits();
        List<Structure> structures = GetAllStructures();
        //Reactivate the friendly units
        foreach (Unit unit in friendlies)
        {
            unit.Activate();
        }
        //Reactivate Structures
        foreach (Structure structure in structures)
        {
            structure.Activate();
        }
        StartPlayerTurn?.Invoke();
    }

    public void SpawnStartingUnits()
    {
        //loop over a placeholder tilemap for placing units 
        int size = 16;
        for (int i = -size / 2; i < size / 2; i++)
        {
            for (int j = -size / 2; j < size / 2; j++)
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0);
                if (entityMap.HasTile(tilePos))
                {
                    TileBase temp = entityMap.GetTile(tilePos);
                    UnitInfo unitInfo = UnitDatabase.Instance.GetUnitInfoFromTile(temp);
                    CropInfo cropInfo = CropDatabase.Instance.GetCropInfoFromTile(temp);
                    StructureInfo structInfo = StructureDatabase.Instance.GetStructureInfoFromTile(temp);

                    if (unitInfo != null)
                    {
                        SpawnUnitOnTile(unitInfo, tilePos);
                    }
                    else if (cropInfo != null)
                    {
                        SpawnCropOnTile(cropInfo, tilePos);
                    }
                    else if (structInfo != null)
                    {
                        SpawnStructureOnTile(structInfo, tilePos);
                    }

                    entityMap.SetTile(tilePos, null);
                }
            }
        }
    }

    public void SpawnUnitOnTile(UnitInfo unitInfo, Vector3Int pos)
    {
        TileData data = tileManager.GetTileDataAt(pos);
        if (data == null || data.HasOccupant())
        {
            //Dont place anything if somethings already here
            return;
        }
        //spawn the new object
        GameObject obj = Instantiate(unitInfo.prefab);
        if (unitInfo.isEnemy)
        {
            obj.transform.parent = enemyUnits;
        }
        else
        {
            obj.transform.parent = friendlyUnits;
        }
        //Update the position
        Unit unitRef  = obj.GetComponent<Unit>();
        if (unitRef != null) {

            tileManager.PlaceEntityOnTile(pos, unitRef);
            unitRef.UpdateTransform(pos);
        }

    }

    public void SpawnStructureOnTile(StructureInfo structInfo, Vector3Int pos)
    {
        TileData data = tileManager.GetTileDataAt(pos);
        if (data == null || data.HasOccupant())
        {
            //Dont place anything if somethings already here
            return;
        }
        //spawn the new object
        GameObject obj = Instantiate(structInfo.prefab);
        obj.transform.SetParent(structureContainter);
        //Update the position
        Structure structRef = obj.GetComponent<Structure>();
        if (structRef != null)
        {

            tileManager.PlaceEntityOnTile(pos, structRef);
            structRef.UpdateTransform(pos);
        }

    }

    public void SpawnCropOnTile(CropInfo cropInfo, Vector3Int pos)
    {
        TileData data = tileManager.GetTileDataAt(pos);
        if (data == null || data.HasOccupant() || data.type == TileType.Grass)
        {
            //Dont place anything if somethings already here
            return;
        }
        //spawn the new object
        GameObject obj = Instantiate(CropDatabase.Instance.cropPrefab);
        if (obj != null)
        {
            obj.transform.parent = cropContainer;
        }
        //Update the position
        Crop cropRef = obj.GetComponent<Crop>();
        if (cropRef != null)
        {
            //Assign the crop to mirror here
            cropRef.Intialize(cropInfo);
            //Assign to the tiledata
            tileManager.PlaceEntityOnTile(pos, cropRef);
            cropRef.UpdateTransform(pos);
        }

    }

    private IEnumerator EnemyTurnRoutine()
    {
        isPlayerTurn = false;
        List<Unit> tempunits = GetAllEnemyUnits();

        foreach (Unit unit in tempunits)
        {
            unit.DoTurn();
            yield return new WaitForSeconds(0.5f); // pause between each enemy
        }

        BeginPlayerTurn();
    }


    //Uses the transform containers to return all friendly units
    public List<Unit> GetAllFriendlyUnits()
    {
        return new List<Unit>(friendlyUnits.GetComponentsInChildren<Unit>());
    }

    public List<Unit> GetAllEnemyUnits()
    {
        return new List<Unit>(enemyUnits.GetComponentsInChildren<Unit>());
    }

    public List<Structure> GetAllStructures()
    {
        return new List<Structure>(structureContainter.GetComponentsInChildren<Structure>());
    }

    public List<Crop> GetAllCrops()
    {
        return new List<Crop>(cropContainer.GetComponentsInChildren<Crop>());
    }

    private void OnEnable()
    {
        input = new AgainstTheGrainInput();
        input.Gameplay.AdvanceTurn.performed += BeginEnemyTurn;
        Entity.OnEntityDestroyed += CheckEndState;
        input.Gameplay.Pause.performed += TogglePause;
        input.Enable();

    }

    private void OnDisable()
    {
        Entity.OnEntityDestroyed -= CheckEndState;
        input.Gameplay.AdvanceTurn.performed -= BeginEnemyTurn;
        input.Gameplay.Pause.performed -= TogglePause;
        input.Disable();
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else
        {
            UnPauseGame();
        }
    }

    public bool IsFriendlyDefeated()
    {
        //determine whether or not the friendly units can still play
        //Determine if there's any units that can still attack
        List<Unit> units = GetAllFriendlyUnits();
        //basic check, is there any friendly units?
        if (units.Count == 0) return true;

        //Deteremine whether or not theres a unit that can attack
        //foreach (Unit unit in units)
        //{
        //    if (unit.CanAttack())
        //    {
        //        return false;
        //    }
        //}
        return false;
    }

    public bool IsEnemyDefeated()
    {
        //determine whether or not the friendly units can still play
        //Determine if there's any units that can still attack
        List<Unit> units = GetAllEnemyUnits();
        //basic check, is there any friendly units?
        if (units.Count == 0) return true;

        //Deteremine whether or not theres a unit that can attack
        foreach (Unit unit in units)
        {
            if (unit.CanAttack())
            {
                return false;
            }
        }
        return true;
    }

    public void CheckEndState()
    {
        if (IsEnemyDefeated())
        {
            Debug.Log("You win!");
            ShowWinScreen();
        }
        else if (IsFriendlyDefeated())
        {
            Debug.Log("You Lose!");
            GameOver();
        }
    }

    public void CheckEndState(Entity entity)
    {
        CheckEndState();
    }

    //Display lose Screen
    public void GameOver()
    {
        //probably do other things later but just want to test functionality
        loseScreen.SetActive(true);
    }

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
    }

    public void PauseGame()
    {
        isPaused = true;
        interactionSystem.DisableInputs();
        actionMenu.TurnOffInput();
        pauseScreen.SetActive(true);
    }

    public void UnPauseGame()
    {
        isPaused = false;
        interactionSystem.EnableInputs();
        actionMenu.TurnOnInput();
        pauseScreen.SetActive(false);
    }

    public void HideAllUIScreens()
    {
        loseScreen.SetActive(false);
        winScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    
}
