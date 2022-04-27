using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;

namespace Moviment3D
{
    public class Dret : EstatPersonatge
    {
        [SerializeField] UI ui;
        [SerializeField] UI_Contextual contextual;
        [SerializeField] InputActionReference climbInputAction;
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

        bool climbContextualUiShowm;

        internal override void EnEntrar()
        {
            //Emparentar();
            MantenirAcceleracioSiInput();

            i.Preparacio.Preparar = 0.15f;
            //Animacio.Dret();
            i.Animacio.Dret();
            climbContextualUiShowm = false;
        }

        

        internal override void EnSortir()
        {
            //Desemparentar();
            if (climbContextualUiShowm)
            {
                climbContextualUiShowm = false;
                contextual.Hide(climbInputAction);
            }
            

            acceleracio = 0;
            velocitatActual = Vector3.zero;
        }
       

       
        internal override void EnUpdate()
        {
            TornarKinematicSiTrobaEsglao();

            //Resistencia.Recuperar();
            i.Resistencia.Recuperar();
            //Emparentar();
            Debugar.DrawRay(transform.position + transform.up, Entorn.Buscar.Terra.InclinacioForward(transform), Color.blue);

            MostrarUIContextual();

            Apretar();

            Acceleracio();
            Orientacio();

            Animar();
            i.Animacio.MovimentY(Mathf.Max(velocitatActual.magnitude / velocitat, i.Dinamic.Velocitat.magnitude * 30));

           
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
            Debugar.DrawRay(transform.position, MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment));
            Debugar.DrawRay(transform.position, Entorn.Buscar.Terra.InclinacioForward(transform), Color.red);

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

        void TornarKinematicSiTrobaEsglao() => i.Dinamic.Rigidbody.isKinematic = Entorn.Buscar.Terra.HiHaEsglao(transform) && !i.Inputs.MovimentZero;

        void MostrarUIContextual()
        {
            //contextual.Show(climbInputAction);
            if (i.Inputs.MovimentZero)
            {
                if ((Entorn.Buscar.Dret.CantonadaForat(transform).Hitted() || Entorn.Buscar.Dret.Endevant(transform).Hitted()) && !climbContextualUiShowm)
                {
                    climbContextualUiShowm = true;
                    contextual.Show(climbInputAction);
                }

                /*if (Entorn.Buscar.Dret.CantonadaForat(transform).Hitted())
                    ui.forat.Mostrar(Entorn.Buscar.Dret.CantonadaForat(transform).point, 0.5f);
                if (Entorn.Buscar.Dret.Endevant(transform).Hitted())
                    ui.paret.Mostrar(Entorn.Buscar.Dret.Endevant(transform).point, 1);*/
            }
            else
            {
                if (climbContextualUiShowm)
                {
                    climbContextualUiShowm = false;
                    contextual.Hide(climbInputAction);
                }

                apretar = Entorn.Buscar.Dret.Endevant(transform).Hitted();

                //ui.forat.Amagar();
                //ui.paret.Amagar();
            }
        }

        void Animar()
        {
            //Animacio.MovimentY(Mathf.Max(velocitatActual.magnitude / velocitat, Dinamic.Velocitat.magnitude * 30));
            
            //if (!Inputs.Saltar)
            //    Animacio.NoTerra(transform);
        }

        void MantenirAcceleracioSiInput()
        {
            if (i.Inputs.AreActived)
            {
                if (!i.Inputs.MovimentZero)
                {
                    acceleracio = new Vector3(i.Dinamic.Velocitat.x, 0, i.Dinamic.Velocitat.z).magnitude * 100;
                }
            }
        }
        void Acceleracio()
        {
            if (!i.Inputs.MovimentZero)
            {
                input = i.Inputs.Moviment;

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
            if (i.Inputs.MovimentZero)
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

            dinamicable.Push(i.Dinamic.Rigidbody);
        }
        void ReleaseDnimaicable()
        {
            if (dinamicable == null)
                return;

            dinamicable.Release(i.Dinamic.Rigidbody);
            dinamicable = null;
        }
        void Orientacio()
        {
            if (!i.Inputs.MovimentZero)
            {
                transform.Orientar(i.Inputs.Moviment, 20);
            }
            else
            {
                transform.Orientar(i.Inputs.Moviment, 4);
            }
        }



        public void C_Terra(Estat.Condicio condicio)
        {
            if (Entorn.Buscar.Terra.Hit(transform).Hitted() &&
                Entorn.Buscar.Terra.Hit(transform).normal.Pla() &&
                i.Preparacio.Preparat &&
                !Entorn.Buscar.Terra.EsRelliscant(transform))

                Estat.Sortida(condicio);
        }
        public void C_Esglao(Estat.Condicio condicio)
        {
            if (Entorn.Buscar.Terra.HiHaEsglao(transform) &&
                !i.Inputs.Saltar)

                Estat.Sortida(condicio);
        }
        public void C_NoEsc(Estat.Condicio condicio)
        {
            if (i.Inputs.Deixar &&
                i.Preparacio.Preparat &&
                Entorn.Buscar.Terra.Hit(transform).Hitted())

                Estat.Sortida(condicio);
        }
        public void C_NoRelliscar(Estat.Condicio condicio)
        {
            if (!Entorn.Buscar.Terra.EsRelliscant(transform) &&
                i.CoyoteTime.Temps(!Entorn.Buscar.Terra.EsRelliscant(transform), 0.25f))
            {
                i.CoyoteTime.Stop();
                Estat.Sortida(condicio);
            }

        }

        private void OnValidate()
        {
            contextual = XS_Editor.LoadAssetAtPath<UI_Contextual>("Assets/XidoStudio/Inputs/Contextual/Basics/Contextual.asset");
        }

    }



}