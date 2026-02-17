using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Farmer, Animal, Enemy
}
public class Unit : MonoBehaviour
{
    [Header("General Settings")]
    public int ID;
    public UnitType type;
    public bool isActive = false;
    public SpriteRenderer sprite;
    private Vector3Int currentPosition;
    public bool isEnemy = false;


    [Header("Stats")]
    public int maxHealth = 10;
    public int currentHealth = 10;
    public int strength = 1;
    public int movementRange = 3;

    int GetHealth()
    {
        return currentHealth;
    }

    public void GetHealth(int healthValue){ 
        currentHealth = healthValue;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Unit has Died!");
    }

    int GetStrength()
    {
        return strength;
    }

    void GetStrength(int strengthValue)
    {
        strength = strengthValue;
    }

    public void SetPosition(Vector3Int pos)
    {
        currentPosition = pos;
    }

    public Vector3Int GetPosition()
    {
        return currentPosition;
    }

}

