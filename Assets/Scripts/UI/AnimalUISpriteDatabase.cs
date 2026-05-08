using UnityEngine;

[CreateAssetMenu(
    fileName = "AnimalUISpriteDatabase",
    menuName = "AgainstTheGrain/UISpriteDatabase"
)]
//THIS CLASS WAS CREATED WITH AI
public class AnimalUISpriteDatabase : ScriptableObject
{
    [System.Serializable]
    //Used to get the uiButton sprites for the given animal ID
    public class AnimalSpriteSet
    {
        public int entityID;

        [Header("Button Sprites")]
        public Sprite normalSprite;
        public Sprite highlightSprite;
        public Sprite unavailableSprite;

        public Sprite[] GetSprites()
        {
            return new Sprite[]
            {
                normalSprite,
                highlightSprite,
                unavailableSprite
            };
        }
    }

    public AnimalSpriteSet[] animals;

    public AnimalSpriteSet GetSpriteSet(int entityID)
    {
        foreach (AnimalSpriteSet set in animals)
        {
            if (set.entityID == entityID)
            {
                return set;
            }
        }

        Debug.LogWarning($"No sprite set found for entity ID: {entityID}");
        return null;
    }
}