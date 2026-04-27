using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyTurnOverlay : MonoBehaviour
{
    public Image overlay;
    public float fadeDuration = 0.5f;
    public float targetAlpha = 0.5f;

    private void Start()
    {
        Color c = overlay.color;
        c.a = 0f;
        overlay.color = c;
    }

    private void OnEnable()
    {
        GameManager.StartEnemyTurn += OnEnemyTurn;
        GameManager.StartPlayerTurn += OnPlayerTurn;
    }

    private void OnDisable()
    {
        GameManager.StartEnemyTurn -= OnEnemyTurn;
        GameManager.StartPlayerTurn -= OnPlayerTurn;
    }

    void OnEnemyTurn()
    {
        FadeTo(targetAlpha);
    }

    void OnPlayerTurn()
    {
        FadeTo(0f);
    }

    void FadeTo(float target)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(target));
    }

    IEnumerator FadeRoutine(float target)
    {
        float start = overlay.color.a;
        float time = 0f;
        Color c = overlay.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(start, target, time / fadeDuration);
            overlay.color = c;
            yield return null;
        }

        c.a = target;
        overlay.color = c;
    }
}