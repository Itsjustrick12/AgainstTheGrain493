using TMPro;
using UnityEngine;

public class BarnUIMenu : MonoBehaviour
{
    public AnimalButton[] buttons;

    public TextMeshProUGUI grainText;
    public TextMeshProUGUI moneyText;

    public void Start()
    {
        foreach (AnimalButton button in buttons)
        {
            button.UpdateButton();
        }
    }

    public void ShowMenu(int grainAmount, int moneyAmount)
    {
        grainText.text = grainAmount.ToString();
        moneyText.text = moneyAmount.ToString();
    }

    public void SellAllCrops()
    {
        //Do something with the economy manager here
    }
}
