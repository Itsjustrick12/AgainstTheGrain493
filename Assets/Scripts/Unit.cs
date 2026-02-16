using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Unit")]
public class Unit : ScriptableObject
{
    [Header("General Settings")]
    public int ID;
    public UnitType type;
    public bool isActive = false;
    public SpriteRenderer sprite;
    public Vector3Int currentPosition;
    public bool isEnemy = false;


    [Header("Stats")]
    public int maxHealth = 10;
    public int currentHealth = 10;
    public int strength = 0;
    public int movementRange = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int getHealth()
    {
        return currentHealth;
    }

    void setHealth(int healthValue){ 
        currentHealth = healthValue;
    }

     int getStrength()
    {
        return strength;
    }
    void setStrength(int strengthValue)
    {
        strength = strengthValue;
    }


}
public enum UnitType
{
    Farmer, Animal, Enemy
}
