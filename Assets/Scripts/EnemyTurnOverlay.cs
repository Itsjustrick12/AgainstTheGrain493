using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyTurnOverlay : MonoBehaviour
{
    public Image overlay;
    public float fadeDuration = 0.5f;
    public float targetAlpha = 0.5f;
    public float playerTurnDelay = 0f;
    public float enemyTurnDelay = 0f;
    private float delay = 0f;

    private void Start()
    {
        Color c = overlay.color;
        c.a = 190f/255f;
        overlay.color = c;
    }

    public void EnemyTurn()
    {
        delay = enemyTurnDelay;
        FadeTo(targetAlpha);
    }

    public void PlayerTurn()
    {
        delay = playerTurnDelay;
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

        yield return new WaitForSeconds(delay);
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