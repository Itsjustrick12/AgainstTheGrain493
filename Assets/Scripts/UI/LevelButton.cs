using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Scene Info")]
    public int sceneNumber;
    public int levelIndex;

    [Header("UI Components")]
    public Button button;
    public TextMeshProUGUI buttonText;
    public Image buttonImage;

    [Header("Sprites")]
    public Sprite lockedSprite;
    public Sprite unlockedSprite;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateUI()
    {
        bool isUnlocked = IsLevelUnlocked();

        buttonText.text = levelIndex.ToString();

        buttonText.gameObject.SetActive(isUnlocked);

        button.interactable = isUnlocked;

        buttonImage.sprite = isUnlocked ? unlockedSprite : lockedSprite;
    }

    bool IsLevelUnlocked()
    {
        return true;//PlayerPrefs.GetInt($"Level_{levelIndex}_Unlocked", 0) == 1;
    }

    public void LoadScene()
    {
        if (!IsLevelUnlocked()) return;

        SceneManager.LoadScene(sceneNumber);
    }
}
