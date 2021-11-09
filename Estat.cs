using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XS_Utils;


public abstract class Estat : MonoBehaviour
{
    [SerializeField] private Condicio[] sortides;
    //internal Informacio info;

    internal abstract void EnEntrar();
    internal abstract void EnSortir();
    internal abstract void EnUpdate();
    internal virtual void EnFixedUpdate() { }

    public void OnEnable()
    {
        //if (info == null) info = GetComponent<Informacio>();

        EnEntrar();
        for (int i = 0; i < sortides.Length; i++)
        {
            sortides[i].My = this;
        }
    }
    private void OnDisable() 
    {
    //    if (Application.isEditor)
    //       return;

        EnSortir();
    }
    private void Update() => EnUpdate();
    private void FixedUpdate() => EnFixedUpdate();

    private void LateUpdate()
    {
        for (int i = 0; i < sortides.Length; i++)
        {
            sortides[i].Invocar(sortides[i]);
        }
    }

    //FALTA: Potser cal un override amb el nom de l'animacio a fer play
    internal static void Sortida(Condicio hab)
    {
        hab.My.enabled = false;
        hab.Objectiu.enabled = true;
    }


    [System.Serializable]
    public class Condicio
    {
        //PUBLIQUES
        private Estat my;
        [SerializeField] private Estat objectiu;
        [SerializeField] private UnityEvent<Condicio> condicions;

        public Estat My { get => my; set => my = value; }

        public Estat Objectiu => objectiu;

        public void Invocar(Condicio condicio)
        {
            condicions.Invoke(condicio);
        }
    }

}

public abstract class EstatPlayer : Estat
{
    //internal Moviment3D.Informacio info;
    //internal Rigidbody rb;

    //internal Transform camara;

    private new void OnEnable()
    {
        //if (info == null) info = GetComponent<Moviment3D.Informacio>();
        //if (rb == null) rb = GetComponent<Rigidbody>();
        //if (camara == null) camara = info.Camara;

        base.OnEnable();
    }
}

public abstract class EstatIA : Estat
{
   /*internal IA.InformacioIA info;

    private new void OnEnable()
    {
        if (info == null) info = GetComponent<IA.InformacioIA>();

        base.OnEnable();
    }*/
}
