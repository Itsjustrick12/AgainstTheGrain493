using UnityEngine;
using UnityEngine.Events;

public class TurnChangeUI : MonoBehaviour
{
    public static UnityEvent TurnAnimationEnd = new UnityEvent();
    public Animator anim;
    public EnemyTurnOverlay enemyOverlay;
    public void EndAnimation()
    {
        TurnAnimationEnd?.Invoke();
        anim.SetTrigger("idle");
    }

    public void PlayPlayerTurn()
    {
            enemyOverlay.PlayerTurn();
            anim.SetTrigger("playerTurn");
    }

    public void PlayEnemyTurn()
    {
        enemyOverlay.EnemyTurn();
        anim.SetTrigger("enemyTurn");
    }
}
