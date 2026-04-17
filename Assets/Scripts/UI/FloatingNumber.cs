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

    public float baseWidth = 32f;
    //For every digit, add this amount to the width to scale it up
    public float digitWidth = 32f;

    protected int currentAmount = 0;
    [SerializeField] protected bool iconOnlyMode = false;

    void Awake()
    {
    }

    public IEnumerator SetNum(int x, int damage, Vector3 pos)
    {
        transform.position = new Vector3(pos.x + 1, pos.y + 1, 10);
        UpdateCounter(damage);
        Debug.Log("showNumber");
        float speed = damage * .02f;
        if(speed > .01f) speed = .01f;
        float elapsed = 0f;
        float duration = 1f;
        float time = 0;
        
        while (time < 360)
        {
            time+=30;
            transform.position += new Vector3(speed * 3, Mathf.Sin((time / 360f) * 2f * Mathf.PI) * speed * Mathf.Abs(x), 0);

            elapsed += Time.deltaTime;
            yield return new WaitForSeconds(duration / 60f);
        }

        Destroy(gameObject);
    }

    public virtual void UpdateCounter(int newValue)
    {
        //creates flags
        bool isNegative = false;
        bool isZero = false;
        int currentAmount = Mathf.Max(0, newValue);

        if(currentAmount < 0)
        {
            isNegative = true;
            currentAmount = Mathf.Abs(currentAmount);
        }
        else if(currentAmount == 0)
        {
            isZero = true;
        }

        string valueStr = currentAmount.ToString();
        int digitCount = valueStr.Length;

        if(!isZero)
        {
            digitCount++;
        }

        int digi = 0;
        if(!isZero)
        {
            digits[0].gameObject.SetActive(true);
            if(isNegative)
            {
                digits[0].UpdateDigit(10);
            }
            else
            {
                digits[0].UpdateDigit(11);
            }
            digi++;
        }
        


        // Update digit sprites
        for (int i = digi; i < digits.Length; i++)
        {
            if (i < digitCount)
            {
                digits[i].transform.position = new Vector3(transform.position.x + digi * digitWidth, transform.position.y, transform.position.z);
                if(isZero)
                {
                    digits[i].sprite.color = Color.blue;
                }
                else if(isNegative)
                {
                    digits[i].sprite.color = Color.red;
                }
                else
                {
                    digits[i].sprite.color = Color.green;
                }
                digits[i].gameObject.SetActive(true);

                digits[i].UpdateDigit(i);
            }
            else
            {
                digits[i].gameObject.SetActive(false);
            }
        }
    }
}
