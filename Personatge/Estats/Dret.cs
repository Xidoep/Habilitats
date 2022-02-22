using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Dret : Estat
    {
        Rigidbody rb;

        [SerializeField] UI ui;
        [SerializeField] int velocitat;
        //public bool esglao;

        Rigidbody otherRigidbody;
        ConfigurableJoint joint;

        //Vector3 PujarSiEsglao => (info.Esglao(transform) ? Vector3.up * 2 : Vector3.zero);
        Vector3 PujarSiEsglao => (Entorn.Buscar.Terra.HiHaEsglao(transform) ? Vector3.up * 2 : Vector3.zero);

        Vector2 input;
        Vector3 velocitatActual;
        float acceleracio;
        public override string ToString() => "Dret";

        Dinamicable dinamicable;

        [SerializeField] bool apretar;

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            //Emparentar();
            MantenirAcceleracioSiInput();

            Preparacio.Preparar = 0.15f;
            Animacio.Dret();

        }

        

        internal override void EnSortir()
        {
            //Desemparentar();

            UIContextual_Amagar();

            acceleracio = 0;
            velocitatActual = Vector3.zero;
        }
       

       
        internal override void EnUpdate()
        {
            TornarKinematicSiTrobaEsglao();

            Resistencia.Recuperar();

            //Emparentar();
            Debug.DrawRay(transform.position + transform.up, Entorn.Buscar.Terra.InclinacioForward(transform), Color.blue);

            MostrarUIContextual();

            Apretar();

            Acceleracio();
            Orientacio();

            Animar();
        }
        internal override void EnFixedUpdate()
        {
            if (acceleracio > 0)
            {
                Moviment();
            }
        }

        void Moviment()
        {
            Debug.DrawRay(transform.position, MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment));

            velocitatActual = ((Entorn.Buscar.Terra.InclinacioForward(transform) + PujarSiEsglao) *
                velocitat *
                //(Inputs.Moviment.sqrMagnitude) *
                (input.sqrMagnitude * acceleracio) *
                Mathf.Clamp01(1 - Vector3.Dot(transform.up, Entorn.Buscar.Terra.InclinacioForward(transform)))
                );

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

        void Emparentar() //Mentenir per si vull enganxarme a un objecte gran sense estar condicionat pel seu moviment
        {

            /*if (!Entorn.Buscar.Terra.Hit(transform).Hitted())
                return;

            if (otherRigidbody == Entorn.Buscar.Terra.Hit(transform).collider.GetComponent<Rigidbody>())
                return;

            otherRigidbody = Entorn.Buscar.Terra.Hit(transform).collider.GetComponent<Rigidbody>();
            if (otherRigidbody == null)
                return;

            if (joint != null) Destroy(joint);

            joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = otherRigidbody;
            */
            if (!Entorn.Buscar.Terra.Hit(transform).Hitted())
                return;

            transform.SetParent(Entorn.Buscar.Terra.Hit(transform).collider.transform);

        }

        void Desemparentar()
        {
            if (joint != null) Destroy(joint);
        }

        void TornarKinematicSiTrobaEsglao() => rb.isKinematic = Entorn.Buscar.Terra.HiHaEsglao(transform) && !Inputs.MovimentZero;

        void MostrarUIContextual()
        {
            if (Inputs.MovimentZero)
            {
                //Animacio.MovimentY(0);
                if (Entorn.Buscar.Dret.CantonadaForat(transform).Hitted())
                    ui.forat.Mostrar(Entorn.Buscar.Dret.CantonadaForat(transform).point, 0.5f);
                if (Entorn.Buscar.Dret.Endevant(transform).Hitted())
                    ui.paret.Mostrar(Entorn.Buscar.Dret.Endevant(transform).point, 1);
                //Animacio.MovimentY(0);
            }
            else
            {
                //Animacio.MovimentY(velocitatActual.magnitude / velocitat);
                apretar = Entorn.Buscar.Dret.Endevant(transform).Hitted();

                ui.forat.Amagar();
                ui.paret.Amagar();
            }
        }

        void UIContextual_Amagar()
        {
            ui.forat.Amagar();
            ui.paret.Amagar();
        }

        void Animar()
        {
            Animacio.MovimentY(Mathf.Max(velocitatActual.magnitude / velocitat, Dinamic.Velocitat.magnitude * 30));
            //if (!Inputs.Saltar)
            //    Animacio.NoTerra(transform);
        }

        void MantenirAcceleracioSiInput()
        {
            if (Inputs.AreActived)
            {
                if (!Inputs.MovimentZero)
                {
                    acceleracio = new Vector3(Dinamic.Velocitat.x, 0, Dinamic.Velocitat.z).magnitude * 100;
                }
            }
        }
        void Acceleracio()
        {
            if (!Inputs.MovimentZero)
            {
                input = Inputs.Moviment;

                acceleracio += Time.deltaTime * 4;
                acceleracio = Mathf.Clamp01(acceleracio);
            }
            else
            {
                acceleracio -= Time.deltaTime * 6;
                acceleracio = Mathf.Clamp01(acceleracio);
            }
        }

        RaycastHit endevantAprop;
        void Apretar()
        {
            if (Inputs.MovimentZero)
            {
                if (!apretar)
                    return;

                apretar = false;

                ReleaseDnimaicable();
                return;
            }

            endevantAprop = Entorn.Buscar.Dret.EndevantAprop(transform);
            if (!endevantAprop.Hitted())
            {
                ReleaseDnimaicable();
                return;
            }

            apretar = Vector3.Dot(transform.forward, endevantAprop.normal) < 0.3f;

            dinamicable = endevantAprop.collider.gameObject.GetComponent<Dinamicable>();
            if (dinamicable == null)
                return;

            dinamicable.Push(rb);
        }
        void ReleaseDnimaicable()
        {
            if (dinamicable == null)
                return;

            dinamicable.Release(rb);
            dinamicable = null;
        }
        void Orientacio()
        {
            if (!Inputs.MovimentZero)
            {
                transform.Orientar(20);
            }
            else
            {
                transform.Orientar(4);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position + transform.up * (0.9f + 0.35f - 0.1f) - (transform.up * 0.9f), 0.3f);
            //Gizmos.DrawSphere(Entorn.Buscar.Terra.Unic(transform).point, 0.4f);
        }
    }

   
  
}