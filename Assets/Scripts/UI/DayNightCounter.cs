using UnityEngine;
using UnityEngine.UI;

public class DayNightCounter : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite daySprite;
    [SerializeField] private Sprite nightSprite;
    [SerializeField] RectTransform imageTransform;
    [SerializeField] UINumber[] numbers;
    public int dayNumber = 0;

    

    private void OnEnable()
    {
        GameManager.StartEnemyTurn += StartNight;
        GameManager.StartPlayerTurn += StartDay;
    }

    private void OnDisable()
    {
        GameManager.StartEnemyTurn -= StartNight;
        GameManager.StartPlayerTurn -= StartDay;
    }

    public void StartNight()
    {
        image.sprite = nightSprite;
    }

    public void StartDay()
    {
        dayNumber++;
        image.sprite = daySprite;
        UpdateDayDisplay();
    }

    private void UpdateDayDisplay()
    {
        string valueStr = dayNumber.ToString();
        int digitCount = valueStr.Length;

        for (int i = 0; i < numbers.Length; i++)
        {
            if (i < digitCount)
            {
                numbers[i].gameObject.SetActive(true);
                int digit = valueStr[i] - '0';
                numbers[i].UpdateDigit(digit);
            }
            else
            {
                numbers[i].gameObject.SetActive(false);
            }
        }
    }

}
