using TMPro;
using UnityEngine;
//Right now only interface with the wheat crop
public class BarnUIMenu : MonoBehaviour
{
    public AnimalButton[] buttons;
    public EconomyManager econManager;
    public TextMeshProUGUI grainText;
    public TextMeshProUGUI coinText;

    public GameObject uiPanel;

    private void OnDisable()
    {
        Structure.OnBarnInteraction -= ShowMenu;
    }

    public void Awake()
    {
        econManager = EconomyManager.Instance;
        Structure.OnBarnInteraction += ShowMenu;
    }

    public void Start()
    {
        foreach (AnimalButton button in buttons)
        {
            button.UpdateButton();
        }
        
    }

    public void ShowMenu()
    {
        uiPanel.SetActive(true);
        UpdateCropText(econManager.GetHarvestedCrops(1));
        UpdateCoinText(econManager.GetCoins());
        
    }

    public void HideMenu()
    {
        uiPanel.SetActive(false);
    }

    public void UpdateCropText(int grainAmount)
    {
        grainText.text = grainAmount.ToString();
    }

    public void UpdateCoinText(int coinAmount)
    {
        coinText.text = coinAmount.ToString();
    }

    public void SellCrop(int id)
    {
        if (econManager.SellHarvestedCrops(id)){
            UpdateCropText(econManager.GetHarvestedCrops(id));
            UpdateCoinText(econManager.GetCoins());
        }
        else
        {
            Debug.Log("You have nothing to sell");
        }
    }
}
