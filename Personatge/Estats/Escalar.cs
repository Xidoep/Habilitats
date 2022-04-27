using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Escalar : EstatPersonatge
    {
        [SerializeField] int velocitat;
        float multiplicador = 1;

        [SerializeField] AnimationCurve velocitatMovimentAjupit;
        [SerializeField] Rig rig;
        [SerializeField] Transform ikMaDreta;
        [SerializeField] Transform ikMaEsquerra;
        [SerializeField] Transform ikPeuDreta;
        [SerializeField] Transform ikMPeuEsquerra;

        Transform helper;
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

        //bool reenganxarCantondaSuperior = false;

        bool Pla => helper.forward.Pla();
        float SumarTemps => Time.deltaTime * velocitat * (1 - Mathf.Clamp(Inclinacio, 0, .5f) * 0.5f) * multiplicador;

        float Inclinacio => Vector3.Dot(helper.forward, Vector3.down);

        RaycastHit puntInicial;
        Collider[] colliders;
        int overlaps;

        internal override void EnEntrar()
        {
            if (colliders == null) colliders = new Collider[10];
            PrepararRigidBody(true);
            CrearHelper(puntInicial);
            PrepararVariables();
            PerpararAnimacio();

            pla = true;

            IKs.Iniciar(helper, Entorn.capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
            if (!pla) IKs.Capturar(Vector2.zero);
            
            ComprovarPla();

            i.Dinamic.Stop();
        }

       

        internal override void EnSortir()
        {
            PrepararRigidBody(false);
            DestruirHelper();
            enPosicio = false;
            i.Preparacio.Preparar = 0.25f;

            transform.SetParent(null);

            if (joint != null) Destroy(joint);

            i.Animacio.Moviment(Vector2.zero);
            IKs.Apagar();
        }



        internal override void EnUpdate()
        {


            Entorn.Escalant.Buscar.CantonadaPlanaAmunt(helper);

            if (!i.Preparacio.Preparat && i.Inputs.Escalar) i.Preparacio.Preparar = 0.25f;

            if (!enPosicio) Desplacar();
            else Quiet();

            i.Animacio.Pla(pla);
            IKs.Debug();
        }


        
        //*******************************//
        //           GENERALS            //
        //*******************************//
        void Desplacar()
        {
            Debugar.Log(Inclinacio);
            temps += SumarTemps;
            
            
            transform.position = Vector3.Lerp(posicioInicial, helper.position - (pla ? Vector3.zero : helper.up * 0.75f), temps);
            //transform.localPosition = Vector3.Lerp(posicioInicial, helper.localPosition - (pla ? Vector3.zero : helper.up * 0.25f), temps);


            ComprovarPla();

            i.Animacio.EnMoviment(true);
            if (!pla)
            {
                if (plaSmooth < 1) plaSmooth = temps;
                //Resistencia.Gastar();
                i.Resistencia.Gastar(Inclinacio);
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

            //Correct position

            if (temps > 1)
            {
                enPosicio = true;
                i.Inputs.SetHelperVectors = helper;

                i.Animacio.EnMoviment(false);

               if (!pla) IKs.Actualitzar(1);
            }
        }
        
       


        void Quiet()
        {


            ComprovarPla();

            //moviment per evitar overlapings
            /*if (!pla)
            {
                overlaps = XS_Physics.Capsule(ref colliders, transform.position + transform.up * 0.5f, transform.position + transform.up * 0.9f, 0.15f, Entorn.capaEntorn);
                if (overlaps > 0)
                {
                    //Snaps the direction oposite of the colliders and snaps them to the Helper's vectors.
                    Vector3 _forward = helper.up;
                    _forward.z = 0f;
                    _forward.Normalize();
                    Vector3 _direction = ((transform.position + helper.up * 0.6f) - colliders[0].ClosestPoint((transform.position + transform.up * 0.6f))).normalized;
                    Vector3 directionAsHelperUp = (_forward * _direction.y - helper.right * _direction.x).normalized;

                    SeleccionarPuntDeMoviment(directionAsHelperUp * 0.1f);

                    Debugar.DrawRay((transform.position + transform.up * 0.6f), _direction, Color.blue);
                    Debugar.DrawRay((transform.position + transform.up * 0.6f), directionAsHelperUp, Color.yellow);

                    //Debugar.DrawLine(transform.position, colliders[0].transform.position, Color.yellow);
                    Debugar.DrawLine((transform.position + transform.up * 0.6f), colliders[0].ClosestPoint((transform.position + transform.up * 0.6f)), Color.yellow);
                    Debugar.Log("hola");
                }
            }*/

            if (!pla) i.Resistencia.GastarLentament();
            else i.Resistencia.RescuperarLentament();

            ComprovarMoviment();

            if (!i.Inputs.Saltar && inputSaltarFlanc) inputSaltarFlanc = false;

            if (!pla)
                if (plaSmooth < 1) plaSmooth += Time.deltaTime * 0.5f;
            else if (plaSmooth > 0) plaSmooth -= Time.deltaTime * 0.5f;
        }



        //************************//
        //         EINES          //
        //************************//

        //HELPER
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

            PosicionarHelper(hit);
            rotacioFinal = transform.rotation;
        }
        void PosicionarHelper(RaycastHit hit, float offestVertical = -.0f)
        {
            helper.SetParent(hit.collider.transform);
            transform.SetParent(hit.collider.transform);

            //posicioInicial = transform.localPosition;
            posicioInicial = transform.position;
            forwardInicial = transform.forward;
            rotacioInicial = transform.rotation;

            if(!helper.forward.PropDe1()) 
                if(helper.forward.Pla()) Entorn.HitNormal(ref hit, helper);
            
            helper.forward = hit.normal;
            helper.position = hit.point + (!Pla ? (hit.normal * 0.4f + helper.up * offestVertical) : Vector3.zero);
        }
        void DestruirHelper()
        {
            i.Inputs.SetHelperVectors = helper;
            Destroy(helper.gameObject);
        }


        //ORIENTACIO
        void OrientacioVertical()
        {
            helper.rotation = Quaternion.Euler(helper.eulerAngles.x, helper.eulerAngles.y, 0);
            transform.rotation = Quaternion.LookRotation(-helper.forward);
        }
        void OrientacioPla() => transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        //PLA
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
        void Quiet_PrepararSalt()
        {
            if (!inputSaltarFlanc)
            {
                inputSaltarFlanc = true;
                i.Inputs.SaltEscalantPreparat = true;
                i.Animacio.SaltPreparat(true);
            }

            if (pla) transform.Orientar(i.Inputs.Moviment,10);
            else i.Animacio.Moviment(i.Inputs.Moviment.normalized);

            if (i.Inputs.Deixar)
            {
                inputSaltarFlanc = false;
                i.Inputs.SaltEscalantPreparat = false;
                i.Animacio.SaltPreparat(false);
            }
        }



        //MOVIMENT
        void ComprovarMoviment()
        {
            if (i.Inputs.Moviment != Vector2.zero && Entorn.Escalant.Moviment(helper, pla, i.Inputs.Moviment, out multiplicador).Hitted())
                SeleccionarPuntDeMoviment(i.Inputs.Moviment);
        }
        void SeleccionarPuntDeMoviment(Vector2 moviment)
        {
            PosicionarHelper(Entorn.Escalant.Moviment(helper, pla, moviment, out multiplicador));

            i.Animacio.EnMoviment(true);

            if (!pla)
                IKs.Capturar(moviment * 0.5f);

            rotacioFinal = MyCamera.Transform.ACamaraRelatiu(moviment).ToQuaternion();

            temps = 0;
            multiplicador = 1;
            enPosicio = false;
        }


        //PREPARACIONS INICIALS/FINALS
        private void PrepararVariables()
        {
            i.Inputs.SaltEscalantPreparat = false;
            i.Inputs.SaltEscalantReenganxarse = false;
            temps = 0;
            multiplicador = 2;
            inputSaltarFlanc = false;
            enPosicio = false;
            i.Preparacio.Preparar = 0.25f;
        }
        private void PerpararAnimacio()
        {
            i.Animacio.Pla(pla);
            /*if (!reenganxarCantondaSuperior)
                i.Animacio.Escalar();
            else i.Animacio.ReenganxarCantondaSuperior();
            */
            i.Animacio.Moviment(Vector2.zero);
            i.Animacio.SaltPreparat(false);
        }
        void PrepararRigidBody(bool esc)
        {

            i.Dinamic.Rigidbody.useGravity = !esc;
            i.Dinamic.Rigidbody.isKinematic = esc;
            if (esc) i.Dinamic.Rigidbody.velocity = Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position + transform.up * 0.5f, 0.15f);
            Gizmos.DrawCube(transform.position + transform.up * 0.7f, new Vector3(0.2f, 0.9f, 0.15f));
            Gizmos.DrawSphere(transform.position + transform.up * 0.9f, 0.15f);
            Gizmos.color = new Color(1, 1, 0, 0.5f);
            Gizmos.DrawCube(helper.localPosition + helper.up, Vector3.one * 0.1f);
        }


        //**************************************//
        //             CONDICIONS               //
        //**************************************//
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

            if (i.Preparacio.Preparat) Entorn.Escalant.Buscar.CantonadaSuperior(transform, (RaycastHit hit) => 
            {
                //reenganxarCantondaSuperior = hit.normal.PropDe1();
                multiplicador = 2;
                i.Resistencia.NoBuidarDelTot();
                CrearHelper(hit);
                Estat.Sortida(condicio);

                i.Animacio.ReenganxarCantondaSuperior();
            });
        }

        public void C_Esc(Estat.Condicio condicio)
        {
            if (i.Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar(transform, out multiplicador).Hitted() &&
                i.Preparacio.Preparat)
            {
                multiplicador = 2;
                puntInicial = Entorn.Buscar.Dret.OnComencarAEscalar(transform, out multiplicador);
                Estat.Sortida(condicio);

                i.Animacio.Escalar();
                //Animacio Escalar desde terra
            }
        }
        public void C_EscAire(Estat.Condicio condicio)
        {
            if (i.Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform, out multiplicador).Hitted() &&
                i.Preparacio.Preparat)
            {
                multiplicador = 2;
                puntInicial = Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform, out multiplicador);
                Estat.Sortida(condicio);

                i.Animacio.Escalar();
                //Aturar durant pocs segons
                //Animacio escalar saltant.
            }
        }

        public void C_EscalarCaient(Estat.Condicio condicio)
        {
            if (i.Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform, out multiplicador).Hitted() &&
                i.Preparacio.Preparat)
            {
                multiplicador = 2;
                puntInicial = Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform, out multiplicador);
                Estat.Sortida(condicio);

                i.Animacio.Escalar();
                //Comprovar la velocitat vertical.
                //Aturar durant un rato, si es massa alta la velocitat.
                //Animacio escalar caient, amb esforç.
            }
        }

        public void C_SaltarEscalantReencangarse(Estat.Condicio condicio)
        {
            if (i.Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar(transform, out multiplicador).Hitted() &&
                i.Preparacio.Preparat)
            {
                multiplicador = 2;
                puntInicial = Entorn.Buscar.Dret.OnComencarAEscalar(transform, out multiplicador);
                Estat.Sortida(condicio);

                i.Animacio.Escalar();
                //Animacio que es reenganxi i es vegi la inercia dependent de la direccio del salt.
            }

        }

        public void C_DeixarInputEscalarQuanPla(Estat.Condicio condicio)
        {
            if(!i.Inputs.Escalar && Pla && enPosicio)
            {
                Estat.Sortida(condicio);
                //Animacio simple de ajupuid a dret
            }
        }

        public void C_CaurePerMassaInclinacio(Estat.Condicio condicio)
        {
            if (i.Inputs.GetHelperForward.CasiMenys1()) Estat.Sortida(condicio);
        }
        public void C_SenseResistencia(Estat.Condicio condicio)
        {
            if (i.Resistencia.Zero) Estat.Sortida(condicio);
        }
    }

}
