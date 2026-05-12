using UnityEngine;
using UnityEngine.Events;

public class TurnChangeUI : MonoBehaviour
{
    public static UnityEvent TurnAnimationEnd = new UnityEvent();
    public Animator anim;
    public EnemyTurnOverlay enemyOverlay;

    //for the sounds made by ahad for each turn
    public AudioClip player;
    public AudioClip enemy;

    public void EndAnimation()
    {
        TurnAnimationEnd?.Invoke();
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

    public void PlayPlayerTurnSFX()
    {
        SoundManager.Instance.PlaySound(player);
    }

    public void PlayEnemyTurnSFX()
    {
        SoundManager.Instance.PlaySound(enemy);
    }
}
