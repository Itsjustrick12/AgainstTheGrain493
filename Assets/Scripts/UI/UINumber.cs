using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class UINumber : MonoBehaviour
{
    Image sprite;
    public Sprite[] numberSprites;
    private void Awake()
    {
        sprite = GetComponent<Image>();
    }

    public void UpdateDigit(int digit)
    {
        sprite.sprite = numberSprites[digit];
    }
}
