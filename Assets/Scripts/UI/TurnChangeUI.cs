using UnityEngine;
using UnityEngine.Events;

public class TurnChangeUI : MonoBehaviour
{
    public static UnityEvent TurnAnimationEnd = new UnityEvent();
    public Animator anim;
    public void EndAnimation()
    {
        TurnAnimationEnd?.Invoke();
        anim.SetTrigger("idle");
    }

   public void PlayPlayerTurn()
   {
        anim.SetTrigger("playerTurn");
   }

    public void PlayEnemyTurn()
    {
        anim.SetTrigger("enemyTurn");
    }
}
