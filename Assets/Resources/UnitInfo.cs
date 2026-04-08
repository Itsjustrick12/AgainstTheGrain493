//Entries for Database system used by Level Editor and other UI
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewUnit", menuName = "AgainstTheGrain/Entities/Unit")]
public class UnitInfo : EntityInfo
{
    [Header("Unit Specific")]
    public bool isEnemy;
    public int attackRange = 1;
    public int strength = 5;
    public int moveRange = 3;
    [Header("Unit Sounds")]
    public AudioClip attackSound;
    public AudioClip pickupSound;
    public AudioClip placeSound;
}