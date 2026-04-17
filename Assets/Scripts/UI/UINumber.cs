using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;

[RequireComponent (typeof(Image))]
public class UINumber : MonoBehaviour
{
    public Image sprite;
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
