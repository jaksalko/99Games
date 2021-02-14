using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAnimationState : StateMachineBehaviour
{
    public event System.Action ActionEnd;
    public Player player;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (animator.GetInteger("action") != 0)
            ActionEnd?.Invoke();
            
    }
}
