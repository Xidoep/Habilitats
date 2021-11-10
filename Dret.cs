using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Dret : EstatPlayer
    {
        Rigidbody rb;

        [SerializeField] UI ui;
        [SerializeField] int velocitat;
        //public bool esglao;

        Rigidbody otherRigidbody;
        ConfigurableJoint joint;

        //Vector3 PujarSiEsglao => (info.Esglao(transform) ? Vector3.up * 2 : Vector3.zero);
        Vector3 PujarSiEsglao => (Entorn.Buscar.Terra.HiHaEsglao(transform) ? Vector3.up * 2 : Vector3.zero);

        Vector3 velocitatActual;
        public override string ToString() => "Dret";

        bool apretar;

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            Emparentar();

            Preparacio.Preparar = 0.15f;
            Animacio.Dret();

        }

        void Emparentar() //Mentenir per si vull enganxarme a un objecte gran sense estar condicionat pel seu moviment
        {
            /*
            if (!Entorn.BuscarTerra(transform).Impactat())
                return;

            if (otherRigidbody == Entorn.BuscarTerra(transform).collider.GetComponent<Rigidbody>())
                return;

            otherRigidbody = Entorn.BuscarTerra(transform).collider.GetComponent<Rigidbody>();
            if (otherRigidbody == null)
                return;

            if (joint != null) Destroy(joint);

            joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = otherRigidbody;

            transform.SetParent(Entorn.BuscarTerra(transform).collider.transform);
            */
        }

        internal override void EnSortir()
        {
            if (joint != null) Destroy(joint);

            ui.forat.Amagar();
            ui.paret.Amagar();
        }

        internal override void EnUpdate()
        {
            rb.isKinematic = Entorn.Buscar.Terra.HiHaEsglao(transform) && !Inputs.MovimentZero;


            Resistencia.Recuperar();
            Emparentar();
            Debug.DrawRay(transform.position + transform.up, Entorn.Buscar.Terra.InclinacioForward(transform), Color.blue);

            if (Inputs.MovimentZero)
            {
                Animacio.MovimentY(0);
                if (Entorn.Buscar.Dret.CantonadaForat(transform).Hitted()) 
                    ui.forat.Mostrar(Entorn.Buscar.Dret.CantonadaForat(transform).point, 0.5f);
                if (Entorn.Buscar.Dret.Endevant(transform).Hitted())
                    ui.paret.Mostrar(Entorn.Buscar.Dret.Endevant(transform).point, 1);

            }
            else
            {
                Animacio.MovimentY(velocitatActual.magnitude / velocitat);
                apretar = Entorn.Buscar.Dret.Endevant(transform).Hitted();

                ui.forat.Amagar();
                ui.paret.Amagar();
            }

            Animacio.NoTerra(transform);
        }
        internal override void EnFixedUpdate()
        { 
            if (!Inputs.MovimentZero)
            {
                transform.Orientar(20);
                Moviment();
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            }
        }

        void Moviment()
        {
            Debug.DrawRay(transform.position, Entorn.Buscar.Terra.InclinacioForward(transform));

            velocitatActual = ((Entorn.Buscar.Terra.InclinacioForward(transform) + PujarSiEsglao) *
                velocitat *
                Inputs.Moviment.sqrMagnitude *
                Mathf.Clamp01(1 - Vector3.Dot(transform.up, Entorn.Buscar.Terra.InclinacioForward(transform))));

            /*Animator.SetFloat(MOVIMENY_Y, ((Entorn.EndevantAmbInclinacio(transform) + PujarSiEsglao) *
                velocitat *
                Inputs.Moviment.sqrMagnitude *
                Mathf.Clamp01(1 - Vector3.Dot(transform.up, Entorn.EndevantAmbInclinacio(transform)))).magnitude / velocitat);*/

           

            /*transform.position +=
                (Entorn.EndevantAmbInclinacio(transform) + PujarSiEsglao) *
                Time.fixedDeltaTime *
                velocitat *
                Inputs.Moviment.sqrMagnitude *
                Mathf.Clamp01(1 - Vector3.Dot(transform.up, Entorn.EndevantAmbInclinacio(transform)));*/

            transform.position += velocitatActual * Time.fixedDeltaTime;
            //rb.velocity = Vector3.zero;
        }

    }
}