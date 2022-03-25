using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Escalar : Estat
    {
        [SerializeField] Info info;
        [SerializeField] int velocitat;
        [SerializeField] AnimationCurve velocitatMovimentAjupit;
        [SerializeField] Rig rig;
        [SerializeField] Transform ikMaDreta;
        [SerializeField] Transform ikMaEsquerra;
        [SerializeField] Transform ikPeuDreta;
        [SerializeField] Transform ikMPeuEsquerra;

        Transform helper;
        Rigidbody rb;
        Rigidbody otherRigidbody;
        ConfigurableJoint joint;

        Vector3 posicioInicial;
        Vector3 forwardInicial;
        Quaternion rotacioInicial;
        Quaternion rotacioFinal;

        float temps;
        bool pla;
        float plaSmooth = 1;
        bool enPosicio;

        bool inputSaltarFlanc;

        bool velocitatEntrada = true;
        bool reenganxarCantondaSuperior = false;

        bool Pla => helper.forward.Pla();
        float SumarTemps => Time.deltaTime * velocitat * (1 - Mathf.Clamp(Vector3.Dot(helper.forward, Vector3.down), 0, .5f) * 0.5f) * (reenganxarCantondaSuperior ? 0.5f : velocitatEntrada ? 2 : 1);

        RaycastHit puntInicial;

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            PrepararRigidBody(true);
            CrearHelper(puntInicial);            

            Inputs.SaltEscalantPreparat = false;
            Inputs.SaltEscalantReenganxarse = false;
            temps = 0;
            velocitatEntrada = true;
            inputSaltarFlanc = false;
            enPosicio = false;
            Preparacio.Preparar = 0.25f;
            pla = false;

            Animacio.Pla(pla);
            if (!reenganxarCantondaSuperior)
                Animacio.Escalar();
            else Animacio.ReenganxarCantondaSuperior();

            Animacio.Moviment(Vector2.zero);
            Animacio.SaltPreparat(false);

            IKs.Iniciar(helper, Entorn.capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
            if (!pla) IKs.Capturar(Vector2.zero);

            Dinamic.Stop();

        }

        internal override void EnSortir()
        {
            PrepararRigidBody(false);
            DestruirHelper();
            enPosicio = false;
            Preparacio.Preparar = 0.25f;

            transform.SetParent(null);

            if (joint != null) Destroy(joint);

            Animacio.Moviment(Vector2.zero);
            IKs.Apagar();
        }

        internal override void EnUpdate()
        {
            if (!Preparacio.Preparat && Inputs.Escalar) Preparacio.Preparar = 0.25f;

            if (!enPosicio) Desplacar();
            else Quiet();

            Animacio.Pla(pla);
            IKs.Debug();
        }


        void ComprovarPla()
        {
            if (helper.forward.Pla())
            {
                if (!pla)
                {
                    pla = true;
                    IKs.Apagar();
                }
            }
            else
            {
                if (pla)
                {
                    pla = false;
                    IKs.Capturar(Vector2.zero);
                }
            }
        }

        

        void Desplacar()
        {
            temps += SumarTemps;
              
            transform.localPosition = Vector3.Lerp(posicioInicial, helper.localPosition, temps);

            ComprovarPla();

            Animacio.EnMoviment(true);
            if (!pla)
            {
                if (plaSmooth < 1) plaSmooth = temps;
                //Resistencia.Gastar();
                info.Resist.Gastar();
                IKs.Actualitzar(temps);
            }
            else 
            {
                if (plaSmooth > 0) plaSmooth = 1 - temps;
                OrientacioPla();
            }

            transform.rotation = Quaternion.Slerp(
                Quaternion.Slerp(rotacioInicial, rotacioFinal, temps),
                Quaternion.Slerp(rotacioInicial, Quaternion.LookRotation(-helper.forward), temps),
                plaSmooth);


            if (temps > 1)
            {
                enPosicio = true;
                Inputs.SetHelperVectors = helper;

                Animacio.EnMoviment(false);

               if (!pla) IKs.Actualitzar(1);
            }
        }
        
       


        void Quiet()
        {
            ComprovarPla();

            //if (!pla) Resistencia.GastarLentament();
            //else Resistencia.RescuperarLentament();

            if (!pla) info.Resist.GastarLentament();
            else info.Resist.RescuperarLentament();

            Quiet_ComençarMoviment();

            if (!Inputs.Saltar && inputSaltarFlanc) inputSaltarFlanc = false;

            if (!pla)
                if (plaSmooth < 1) plaSmooth += Time.deltaTime * 0.5f;
            else if (plaSmooth > 0) plaSmooth -= Time.deltaTime * 0.5f;
        }

        void OrientacioVertical()
        {
            helper.rotation = Quaternion.Euler(helper.eulerAngles.x, helper.eulerAngles.y, 0);
            transform.rotation = Quaternion.LookRotation(-helper.forward);
        }
        void OrientacioPla() => transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        void Quiet_PrepararSalt()
        {
            if (!inputSaltarFlanc)
            {
                inputSaltarFlanc = true;
                Inputs.SaltEscalantPreparat = true;
                Animacio.SaltPreparat(true);
            }

            if (pla) transform.Orientar(10);
            else Animacio.Moviment(Inputs.Moviment.normalized);

            if (Inputs.Deixar)
            {
                inputSaltarFlanc = false;
                Inputs.SaltEscalantPreparat = false;
                Animacio.SaltPreparat(false);
            }
        }
    
        
        void Quiet_ComençarMoviment()
        {

            if (Inputs.Moviment != Vector2.zero && Entorn.Escalant.Moviment(helper, pla, Inputs.Moviment).Hitted())
            {
                PosicionarHelper(Entorn.Escalant.Moviment(helper, pla, Inputs.Moviment));

                Animacio.EnMoviment(true);

                if (!pla)
                    IKs.Capturar(Inputs.Moviment * 0.5f);

                rotacioFinal = MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment).ToQuaternion();

                temps = 0;
                velocitatEntrada = false;
                enPosicio = false;
            }
        }



        void CrearHelper(RaycastHit hit)
        {
            helper = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            helper.localScale = Vector3.one * 0.3f;

            otherRigidbody = hit.collider.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                joint = gameObject.AddComponent<ConfigurableJoint>();
                joint.connectedBody = otherRigidbody;
            }

            PosicionarHelper(hit, -0.4f);
            rotacioFinal = transform.rotation;
        }
        void PosicionarHelper(RaycastHit hit, float offestVertical = 0)
        {
            helper.SetParent(hit.collider.transform);
            transform.SetParent(hit.collider.transform);

            posicioInicial = transform.localPosition;
            forwardInicial = transform.forward;
            rotacioInicial = transform.rotation;

            if(!helper.forward.PropDe1()) 
                if(helper.forward.Pla()) Entorn.HitNormal(ref hit, helper);
            
            helper.forward = hit.normal;
            helper.position = hit.point + (!Pla ? (hit.normal * 0.4f + helper.up * offestVertical) : Vector3.zero);
        }
        void PrepararRigidBody(bool esc)
        {

            rb.useGravity = !esc;
            rb.isKinematic = esc;
            if (esc) rb.velocity = Vector3.zero;
        }

        void DestruirHelper()
        {
            Inputs.SetHelperVectors = helper;
            Destroy(helper.gameObject);
        }



        public void C_TrobarCantonadaSuperior(Estat.Condicio condicio) 
        {
            /* if (info.Preparat && entorn.BuscarCantonadaSuperior(transform).Impactat() && info.Preparat)
             {
                 reenganxat = true;
                 info.Resistencia = Mathf.Max(info.Resistencia, 0.1f);
                 CrearHelper(entorn.BuscarCantonadaSuperior(transform));
                 Estat.Sortida(condicio);
             }*/

            /*if (Preparacio.Preparat && Entorn.Escalant.Buscar.CantonadaSuperior(transform).Impactat())
            {
                reenganxat = true;
                Resistencia.NoBuidarDelTot();
                CrearHelper(Entorn.Escalant.Buscar.CantonadaSuperior(transform));
                Estat.Sortida(condicio);
            }*/

            if (Preparacio.Preparat) Entorn.Escalant.Buscar.CantonadaSuperior(transform, (RaycastHit hit) => 
            {
                reenganxarCantondaSuperior = hit.normal.PropDe1();
                //Resistencia.NoBuidarDelTot();
                info.Resist.NoBuidarDelTot();
                CrearHelper(hit);
                Estat.Sortida(condicio);
            });
        }

        public void C_Esc(Estat.Condicio condicio)
        {
            if (Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar(transform).Hitted() &&
                Preparacio.Preparat)
            {
                puntInicial = Entorn.Buscar.Dret.OnComencarAEscalar(transform);
                Estat.Sortida(condicio);
                //Animacio Escalar desde terra
            }
        }
        public void C_EscAire(Estat.Condicio condicio)
        {
            if (Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform).Hitted() &&
                Preparacio.Preparat)
            {
                puntInicial = Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform);
                Estat.Sortida(condicio);
                //Aturar durant pocs segons
                //Animacio escalar saltant.
            }
        }

        public void C_EscalarCaient(Estat.Condicio condicio)
        {
            if (Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform).Hitted() &&
                Preparacio.Preparat)
            {
                puntInicial = Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform);
                Estat.Sortida(condicio);
                //Comprovar la velocitat vertical.
                //Aturar durant un rato, si es massa alta la velocitat.
                //Animacio escalar caient, amb esforç.
            }
        }

        public void C_SaltarEscalantReencangarse(Estat.Condicio condicio)
        {
            if (Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar(transform).Hitted())
            {
                puntInicial = Entorn.Buscar.Dret.OnComencarAEscalar(transform);
                Estat.Sortida(condicio);
                //Animacio que es reenganxi i es vegi la inercia dependent de la direccio del salt.
            }

        }

        public void C_DeixarInputEscalarQuanPla(Estat.Condicio condicio)
        {
            if(!Inputs.Escalar && Pla && enPosicio)
            {
                Estat.Sortida(condicio);
                //Animacio simple de ajupuid a dret
            }
        }

        public void C_CaurePerMassaInclinacio(Estat.Condicio condicio)
        {
            if (Inputs.GetHelperForward.CasiMenys1()) Estat.Sortida(condicio);
        }
        public void C_SenseResistencia(Estat.Condicio condicio)
        {
            if (Resistencia.Zero) Estat.Sortida(condicio);
        }
    }

}
