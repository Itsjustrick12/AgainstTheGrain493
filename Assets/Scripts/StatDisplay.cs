using TMPro;
using UnityEngine;

public class StatDisplay : MonoBehaviour
{
    public TextMeshProUGUI text;
    public void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(int amount)
    {
        text.text = amount.ToString();
    }
}
