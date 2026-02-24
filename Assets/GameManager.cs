using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class GameManager : MonoBehaviour
{
    public TileManager tileManager;

    //Transforms for sorting
    public Transform friendlyUnits;
    public Transform enemyUnits;
    public Transform cropContainer;

    public Tilemap entityMap;
    private AgainstTheGrainInput input;

    public static event Action StartPlayerTurn;

    public GameObject pauseScreen;
    public GameObject winScreen;
    public GameObject loseScreen;

    public bool isPlayerTurn = true;

    public void Awake()
    {
        tileManager = FindFirstObjectByType<TileManager>();

    }

    public void Start()
    {
        isPlayerTurn = true;
        SpawnStartingUnits();
    }
    
    public void BeginEnemyTurn(InputAction.CallbackContext context)
    {
        EnemyTurn();
    }

    public void BeginPlayerTurn()
    {
        // Call this whenever a turn/day ends
        Debug.Log("Turn advanced!");
        List<Unit> friendlies = GetAllFriendlyUnits();
        //Reactivate the friendly units
        foreach (Unit unit in friendlies)
        {
            unit.Activate();
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
                    if (unitInfo != null)
                    {
                        SpawnUnitOnTile(unitInfo, tilePos);
                    }
                    else if (cropInfo != null)
                    {
                        SpawnCropOnTile(cropInfo, tilePos);
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

    public void EnemyTurn()
    {
        isPlayerTurn = false;
        List<Unit> tempunits = GetAllEnemyUnits();

        foreach(Unit unit in tempunits)
        {
            unit.DoTurn();

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

    public List<Crop> GetAllCrops()
    {
        return new List<Crop>(cropContainer.GetComponentsInChildren<Crop>());
    }

    private void OnEnable()
    {
        input = new AgainstTheGrainInput();
        input.Gameplay.AdvanceTurn.performed += BeginEnemyTurn;
        input.Enable();

    }

    private void OnDisable()
    {
        input.Gameplay.AdvanceTurn.performed -= BeginEnemyTurn;
        input.Disable();
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
        pauseScreen.SetActive(true);
    }

    public void HideAllUIScreens()
    {
        loseScreen.SetActive(false);
        winScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }
}
