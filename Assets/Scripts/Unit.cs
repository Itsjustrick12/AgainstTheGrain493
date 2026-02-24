using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum UnitType
{
    Farmer, Animal, Enemy
}
public class Unit : Entity
{
    [Header("General Settings")]
    public int ID;
    public UnitType type;
    public bool isEnemy = false;


    [Header("Stats")]
    public int maxHealth = 10;
    public int currentHealth = 10;
    public int strength = 1;
    //Only increase for ranged attackers
    public int attackRange = 1;

    public int movementRange = 3;

    //Hidden logic for determining what a unit is able to do, define by the unit database
    private List<UnitAction> actions = new();

    public override void Awake()
    {
        base.Awake();
        InitializeActions();
    }

    public void InitializeActions(List<UnitAction> unitActions)
    {
        actions = unitActions;
    }

    public void InitializeActions()
    {
        //This references the action set defined in the unit database
        actions = UnitDatabase.Instance.GetActions(ID);
    }

    public List<UnitAction> GetAvailableActions()
    {
        foreach (var action in actions)
        {
            Debug.Log($"Action: {action.GetName()} | IsPossible: {action.IsPossible(this)}");
        }
        //Return all the actions that are currently possible given the Unit's information (and generally position)
        return actions.Where(action => action.IsPossible(this)).ToList();
    }

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
        Debug.Log("Unit hit for " + damage + " damage!");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetAttackRange()
    {
        return attackRange;
    }

    public bool IsSameTeamAs(Unit diffUnit)
    {
        if (diffUnit == null)
        {
            return false;
        }

        //Check if same side, either both enemies or both not
        if ((isEnemy && diffUnit.isEnemy) || (!isEnemy && !diffUnit.isEnemy))
        {
            return true;
        }
        //Otherwise, different teams
        return false;
    }

    public bool IsSameTeamAs(Entity entity)
    {
        if (entity == null)
        {
            Debug.LogError("The passed entity is null, can't compare teams");
            return false;
        }

        //Cast entity to unit
        Unit unitCheck = entity as Unit;


        return IsSameTeamAs(unitCheck);
    }

    public void Die()
    {
        Debug.Log("Unit has Died!");
        DestroyEntity();
    }

    public int GetStrength()
    {
        return strength;
    }

    public void SetStrength(int strengthValue)
    {
        strength = strengthValue;
    }

}

