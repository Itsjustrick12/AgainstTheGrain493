using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;

public class FloatingNumber : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float lifetime = 1f;
    public int number = 0;
    public UINumber[] digits;
    public float mx = 1f;
    public float b = 0.6f;
    public float speedMult = 1f;
    public float speedMax = 10f;
    public float yDistance = 0.5f;

    public float baseWidth = 16f;
    //For every digit, add this amount to the width to scale it up
    public float digitWidth = 16f;

    protected int currentAmount = 0;
    [SerializeField] protected bool iconOnlyMode = false;

    void Awake()
    {
    }

    public IEnumerator SetNum(int x, int damage, Vector3 pos)
    {
        MovePanel(pos, x);
        UpdateCounter(damage);

        float speed = damage * speedMult;
        if(speed > speedMax)
        {
            speed = speedMax;
        }
        if(x > 0)
        {
            speed = speed * -1;
        }

        float time = 0f;
        RectTransform rect = GetComponent<RectTransform>();

        while (time < 360)
        {

            rect.position += new Vector3(
                speed * yDistance,
                Mathf.Sin((time / 360f) * 2f * Mathf.PI) * Mathf.Abs(speed * x),
                0
                );
            time += 15;
            yield return new WaitForSeconds(1f / 60f);
        }

        Destroy(gameObject);
    }

    public virtual void UpdateCounter(int newValue)
    {
        bool isNegative = false;
        bool isZero = false;
        int currentAmount = newValue;

        if (currentAmount < 0)
        {
            isNegative = true;
            currentAmount = Mathf.Abs(currentAmount);
        }
        else if (currentAmount == 0)
        {
            isZero = true;
        }

        string valueStr = currentAmount.ToString();
        int digitCount = valueStr.Length;

        if (!isZero)
            digitCount++;

        int digi = 0;

        if (!isZero)
        {
            digits[0].gameObject.SetActive(true);
            digits[0].UpdateDigit(isNegative ? 11 : 10);
            if (isZero)
                    digits[0].sprite.color = Color.blue;
                else if (isNegative)
                    digits[0].sprite.color = Color.green;
                else
                    digits[0].sprite.color = Color.red;
            digi++;
        }

        for (int i = digi; i < digits.Length; i++)
        {
            if (i < digitCount)
            {
                RectTransform rt = digits[i].GetComponent<RectTransform>();
                rt.localPosition = new Vector3((i - digi) * digitWidth, 0f, 0f);

                if (isZero)
                    digits[i].sprite.color = Color.blue;
                else if (isNegative)
                    digits[i].sprite.color = Color.green;
                else
                    digits[i].sprite.color = Color.red;

                digits[i].gameObject.SetActive(true);

                int digitValue = isZero ? 0 : valueStr[i - digi] - '0';
                digits[i].UpdateDigit(digitValue);
            }
            else
            {
                digits[i].gameObject.SetActive(false);
            }
        }
    }

    public void MovePanel(Vector3 pos, int x)
    {
        x = x * -1;
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 offset = new Vector3((float)(x / Mathf.Abs(x)) * mx - b, -7f, 0f);
        Vector3 desiredWorldPos = pos + offset;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(desiredWorldPos);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out localPoint
        );

        rect.anchoredPosition = localPoint;
    }
}
