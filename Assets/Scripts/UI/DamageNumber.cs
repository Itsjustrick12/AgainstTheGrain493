using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;

public class DamageNumber : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float lifetime = 1f;
    public int number = 0;
    public TextMeshPro textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public IEnumerator SetNum(int x, int damage, Vector3Int pos)
    {
        transform.position = pos;
        Renderer rend = GetComponent<Renderer>();
        if(damage > 0)
        {
            rend.material.color = Color.green;
        }
        if(damage < 0)
        {
            rend.material.color = Color.red;
        }
        else
        {
            rend.material.color = Color.blue;
        }
        float speed = damage * .02f;
        if(speed > .005f) speed = .01f;
        float elapsed = 0f;
        float duration = 1f;
        float time = 0;
        
        while (time < 360)
        {
            time+=30;
            transform.position += new Vector3(Mathf.Sin((time / 360f) * 2f * Mathf.PI) * speed * x, x / 2, 0);

            elapsed += Time.deltaTime;
            yield return new WaitForSeconds(duration / 60f);
        }

        Destroy(gameObject);
    }
}