using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moviment3D;

public class Animacio_DretPeus : StateMachineBehaviour
{
    //[SerializeField] bool dreta;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    bool checking;
    float offset;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //offset = 0;
        //Animacio.Dreta(false);
        //checking = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*if (!checking)
            return;

        if(stateInfo.normalizedTime - offset > stateInfo.length * 0.5f)
        {
            Animacio.Dreta(true);
        }
        else Animacio.Dreta(false);

        if(stateInfo.normalizedTime - offset > stateInfo.length)
        {
            offset = stateInfo.normalizedTime;
        }
        */
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        //checking = false;
    }
    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
