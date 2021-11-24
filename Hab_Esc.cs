using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Hab_Esc : EstatPlayer
    {
        Transform helper;
        Rigidbody rb;

        Vector3 posicioInicial;
        Quaternion rotacioInicial;

        float temps;
        bool enPosicio;

        [SerializeField] int velocitat;
        int multiplicadorVelocitat = 1;
        [SerializeField] bool reenganxat = false;
        [SerializeField] AnimationCurve velocitatMovimentAjupit;
        [SerializeField] LayerMask capaEntorn;
        [SerializeField] Rig rig;
        [SerializeField] Transform ikMaDreta;
        [SerializeField] Transform ikMaEsquerra;
        [SerializeField] Transform ikPeuDreta;
        [SerializeField] Transform ikMPeuEsquerra;

        Rigidbody otherRigidbody;
        ConfigurableJoint joint;

        bool inputSaltarFlanc;

        bool Pla => helper.forward.Pla();
        bool pla;

        public override string ToString() => "Escalar!";

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            PrepararRigidBody(true);
            if (!reenganxat) CrearHelper(Entorn.Buscar.Dret.OnComencarAEscalar(transform));
            //else 
            Inputs.SaltEscalantPreparat = false;
            Inputs.SaltEscalantReenganxarse = false;
            temps = 0;
            multiplicadorVelocitat = 2;
            inputSaltarFlanc = false;
            enPosicio = false;
            Preparacio.Preparar = 0.25f;
            reenganxat = false;

            pla = Pla;
            Animacio.Pla(Pla);
            Animacio.Escalar();
            Animacio.Moviment(Vector2.zero);
            Animacio.MovimentY(0);
            Animacio.SaltPreparat(false);

            IKs.Iniciar(helper, capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
            if (!Pla) IKs.Capturar(Vector2.zero);
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
            Animacio.MovimentY(0);
            IKs.Apagar();
        }

        internal override void EnUpdate()
        {
            if (!Preparacio.Preparat && Inputs.Escalar) Preparacio.Preparar = 0.25f;

            if (!enPosicio) Desplacar();
            else Quiet();

            Animacio.Pla(Pla);

            IKs.Debug();
        }

        void Desplacar()
        {
            temps += Time.deltaTime * velocitat * (1 - Mathf.Clamp(Vector3.Dot(helper.forward, Vector3.down),0,.5f)) * multiplicadorVelocitat ;
            transform.localPosition = Vector3.Lerp(posicioInicial, helper.localPosition, temps);

            if (Pla)
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
                transform.rotation = Quaternion.Slerp(rotacioInicial, Quaternion.LookRotation(-helper.forward), temps);
                Resistencia.Actual -= 1 * Time.deltaTime;
            }

            if (temps > 1)
            {
                enPosicio = true;
                Inputs.SetHelperVectors = helper;

                Animacio.EnMoviment(false);
                Animacio.Moviment(Vector2.zero);
                //Animacio.MovimentY(0);
                IKs.Actualitzar(1);
            }
            else 
            {
                Animacio.EnMoviment(true);
                if (Pla)
                {
                    Animacio.MovimentY(velocitatMovimentAjupit.Evaluate(temps));
                }
                else
                {
                    IKs.Actualitzar(temps);
                }
            } 

        }
        void Quiet()
        {
            if (Inputs.Saltar)
            {
                if (!inputSaltarFlanc)
                {
                    inputSaltarFlanc = true;
                    Inputs.SaltEscalantPreparat = true;
                    Animacio.SaltPreparat(true);
                }

                if (Pla) 
                {
                    transform.Orientar(10);
                }
                else
                {
                    Animacio.Moviment(Inputs.Moviment.normalized);
                }

                if (Inputs.Deixar)
                {
                    inputSaltarFlanc = false;
                    Inputs.SaltEscalantPreparat = false;
                    Animacio.SaltPreparat(false);
                }
            }

            if (inputSaltarFlanc)
                return;

            if(!Inputs.Saltar)
            {
                if (!Pla)
                {
                    helper.rotation = Quaternion.Euler(helper.eulerAngles.x, helper.eulerAngles.y, 0);
                    transform.rotation = Quaternion.LookRotation(-helper.forward);
                    Resistencia.Actual -= 0.1f * Time.deltaTime;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                    IKs.Apagar();
                }
            }

            if (Inputs.Moviment != Vector2.zero && Entorn.Escalant.Moviment(helper, Pla, Inputs.Moviment).Hitted())
            {
                PosicionarHelper(Entorn.Escalant.Moviment(helper, Pla, Inputs.Moviment));

                Animacio.EnMoviment(true);
                if (Pla)
                {
                    transform.forward = MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment);
                    Animacio.MovimentY(1);
                }
                else
                {
                    Animacio.Moviment(Inputs.Moviment);
                    IKs.Capturar(Inputs.Moviment * 0.15f);
                }


                temps = 0;
                multiplicadorVelocitat = 1;
                enPosicio = false;


            }

            if (!Inputs.Saltar && inputSaltarFlanc)
            {
                inputSaltarFlanc = false;
            }

            

        }

        void CrearHelper(RaycastHit hit)
        {
            helper = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            helper.localScale = Vector3.one * 0.3f;

            otherRigidbody = hit.collider.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                joint = gameObject.AddComponent<ConfigurableJoint>();
                joint.connectedBody = otherRigidbody;
            }

            PosicionarHelper(hit, -0.4f);
        }
        void PosicionarHelper(RaycastHit hit, float offestVertical = 0)
        {
            helper.SetParent(hit.collider.transform);
            transform.SetParent(hit.collider.transform);

            posicioInicial = transform.localPosition;
            rotacioInicial = transform.rotation;

            helper.forward = hit.normal;
            helper.position = hit.point + (!Pla ? (hit.normal * 0.4f + helper.up * offestVertical) : Vector3.zero);
            Entorn.HitNormal(ref hit, helper);
            helper.forward = hit.normal;
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
                reenganxat = true;
                Resistencia.NoBuidarDelTot();
                CrearHelper(hit);
                Estat.Sortida(condicio);
            });
        }

        
    }

}
