using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    ATTACK,
    HURT,
    SELECT,
    PLACE,
    DEATH
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    //[SerializeField] private AudioClip[] soundList;
    public static SoundManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayEntitySound(Entity entity, SoundType type)
    {
        Unit tempUnit = entity as Unit;
        int unitID = tempUnit.ID;
        if(tempUnit != null)
        {
            UnitInfo info = UnitDatabase.Instance.GetUnitInfo(unitID);
            switch (type)
            {
                case SoundType.HURT:
                    PlaySound(info.hurtSound);
                    break;
                case SoundType.DEATH:
                    PlaySound(info.deathSound);
                    break;
                case SoundType.ATTACK:
                    PlaySound(info.attackSound);
                    break;
                case SoundType.SELECT:
                    PlaySound(info.pickupSound);
                    break;
                case SoundType.PLACE:
                    PlaySound(info.pickupSound);
                    break;
                default:
                    break;
            }
        }
    }

    /*
    public static void PlaySound(SoundType sound , float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
    */
}
