using UnityEngine;

public class CropButtonArrow : MonoBehaviour
{
    [SerializeField] private RectTransform arrowRect;
    [SerializeField] private float bounceDistance = 10f;
    [SerializeField] private float bounceSpeed = 0.1f;

    private const float IconOnlyX = 64f;
    private const float ExpandedX = 128f;

    private Vector2 basePosition;
    private bool isVisible = false;

    private void Awake()
    {
        if (arrowRect != null)
        {
            basePosition = new Vector2(IconOnlyX, arrowRect.anchoredPosition.y);
            arrowRect.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isVisible || arrowRect == null) return;
        float offset = Mathf.Sin(Time.time * bounceSpeed) * bounceDistance;
        arrowRect.anchoredPosition = basePosition + new Vector2(offset, 0f);
    }

    public void Show()
    {
        isVisible = true;
        arrowRect.gameObject.SetActive(true);
    }

    public void Hide()
    {
        isVisible = false;
        arrowRect.gameObject.SetActive(false);
    }

    public void SetIconOnly(bool iconOnly)
    {
        float x = iconOnly ? IconOnlyX : ExpandedX;
        basePosition = new Vector2(x, arrowRect.anchoredPosition.y);
    }
}