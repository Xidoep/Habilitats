using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Anim : ScriptableObject
{
    protected Animator animator;

    public void Trigger(int parametre)
    {
        if (!animator)
            return;

        animator.SetTrigger(parametre);
    }

    public void Float(int parametre, float valor)
    {
        animator.SetFloat(parametre, valor);
    }

    public void Bool(int parametre, bool valor)
    {
        animator.SetBool(parametre, valor);
    }
}
