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
    public TMP_Text textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public IEnumerator SetNum(int x, int damage, Vector3 pos)
    {
        transform.position = new Vector3(pos.x + 1, pos.y + 1, 10);
        if(damage > 0)
        {
            textMesh.text = x.ToString();
            textMesh.color = Color.red;
        }
        else if(damage < 0)
        {
            textMesh.text = "+" + x.ToString();
            textMesh.color = Color.green;
        }
        else
        {
            textMesh.text = x.ToString();
            textMesh.color = Color.blue;
        }
        textMesh.outlineWidth = 0.2f;
        textMesh.outlineColor = Color.black;
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
}
