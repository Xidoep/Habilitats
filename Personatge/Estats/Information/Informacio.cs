using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using XS_Utils;

namespace Moviment3D
{
    [SelectionBase]
    [DefaultExecutionOrder(-1)]
    public class Informacio : MonoBehaviour
    {
        [SerializeField] LayerMask capaEntorn;

        [SerializeField] InputActionReference moviment;
        [SerializeField] InputActionReference saltar;
        [SerializeField] InputActionReference agafar;
        [SerializeField] InputActionReference deixar;
        [SerializeField] Rig rig;

        [SerializeField] Transform ikMaDreta;
        [SerializeField] Transform ikMaEsquerra;
        [SerializeField] Transform ikPeuDreta;
        [SerializeField] Transform ikMPeuEsquerra;

        [Header("Canals")]
        [SerializeField] Canal_FloatFloat resistencia;

        [SerializeField] bool testing;
        public float debug;

        private void OnEnable()
        {
            Animacio.Iniciar(transform);
            Inputs.Iniciar(moviment, saltar, agafar, deixar);
            Entorn.Iniciar(capaEntorn);
            Resistencia.testing = testing;
            Resistencia.Canal = resistencia;
            gameObject.AddComponent<Environment.Effector>().LayerMask = capaEntorn;
            //IKs.Iniciar(helper, capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
            //IKs.Iniciar(transform, capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debugar.Log($"Colisio a {collision.relativeVelocity.magnitude}");
        }

        private void Update()
        {
            debug = Dinamic.Velocitat.magnitude;
            //IKs.Debug();
        }


        private void FixedUpdate()
        {
            Dinamic.ActualitzarSmooth(transform);
        }
        //internal Animator Animator => animator;

        //CONDICIONS
        public void C_Esc(Estat.Condicio condicio) 
        {
            if (Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar(transform).Hitted() &&
                Preparacio.Preparat)
            {

                //Animator.SetBool("Dret", true);
                Estat.Sortida(condicio);
            }
        }
        public void C_EscAire(Estat.Condicio condicio)
        {
            if (Inputs.Escalar &&
                Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform).Hitted() &&
                Preparacio.Preparat)
            {

                //Animator.SetBool("Dret", true);
                Estat.Sortida(condicio);
            }
        }
        public void C_NoEsc(Estat.Condicio condicio) 
        { 
            if (Inputs.Deixar && 
                Preparacio.Preparat &&
                Entorn.Buscar.Terra.Hit(transform).Hitted()) 
                
                Estat.Sortida(condicio); 
        }
        public void C_NoEscAire(Estat.Condicio condicio)
        {
            if (Inputs.Deixar &&
                Preparacio.Preparat &&
                !Entorn.Buscar.Terra.Hit(transform).Hitted()) 
                
                Estat.Sortida(condicio);
        }
        public void C_Mov(Estat.Condicio condicio) 
        { 
            if (Inputs.Moviment != Vector2.zero) Estat.Sortida(condicio); 
        }
        public void C_VelocitatVerticalNegativa(Estat.Condicio condicio) 
        { 
            if (Dinamic.VelocitatVerticalNegativa) Estat.Sortida(condicio); 
        }
        public void C_Terra(Estat.Condicio condicio) 
        { 
            if (Entorn.Buscar.Terra.Hit(transform).Hitted() && 
                Preparacio.Preparat && 
                !Entorn.Buscar.Terra.EsRelliscant(transform)) 
                
                Estat.Sortida(condicio); 
        }
        public void C_NoTerra(Estat.Condicio condicio) 
        { 
            if (!Entorn.Buscar.Terra.Hit(transform).Hitted() && 
                !Entorn.Buscar.Terra.HiHaEsglao(transform) && 
                CoyoteTime.Temps(!Entorn.Buscar.Terra.Hit(transform).Hitted(), 0.02f) && 
                Preparacio.Preparat)
            {
                CoyoteTime.Stop();
                Estat.Sortida(condicio);
            }
                
        }
        public void C_NoParetDevant(Estat.Condicio condicio)
        {
            if (!Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform).Hitted() &&
                Preparacio.Preparat)
            {
                Estat.Sortida(condicio);
            }
        }

        public void C_Salt(Estat.Condicio condicio) 
        { 
            if (Inputs.Saltar && 
                Resistencia.UnaMica &&
                Preparacio.Preparat)
            {
                //Animacio.Saltar();
                Estat.Sortida(condicio); 
            }
                
        }
        public void C_SaltarEscalant(Estat.Condicio condicio) 
        { 
            /*if (!Inputs.Saltar && 
                Inputs.SaltEscalantPreparat) 
                
                Estat.Sortida(condicio);
            */
            if (Inputs.Saltar)
            {
                Estat.Sortida(condicio);
            }
        }
        public void C_SaltarEscalantReencangarse(Estat.Condicio condicio) 
        {
            if (Inputs.Escalar && 
                Entorn.Buscar.Dret.OnComencarAEscalar(transform).Hitted()) 
                
                Estat.Sortida(condicio); 
        }
        public void C_SaltarEscalantCaure(Estat.Condicio condicio) 
        {
            if (Preparacio.Preparat && 
                CoyoteTime.Temps(Preparacio.Preparat, 0.4f))
            {
                CoyoteTime.Stop();
                Estat.Sortida(condicio); 
            }
                
        }
        public void C_TornarAAgafarSiPot(Estat.Condicio condicio) 
        { 
            if (Preparacio.Preparat && 
                Entorn.Buscar.Dret.OnComencarAEscalar(transform).Hitted()) 
                
                Estat.Sortida(condicio); 
        }
        public void C_TrobarCantonadaSuperior(Estat.Condicio condicio) 
        { 
            if (Preparacio.Preparat && 
                Entorn.Escalant.Buscar.CantonadaSuperior(transform).Hitted() &&
                Inputs.SaltEscalantReenganxarse)
                
                Estat.Sortida(condicio); 
        }
        public void C_CaurePerMassaInclinacio(Estat.Condicio condicio) 
        {
            if(Inputs.GetHelperForward.CasiMenys1()) Estat.Sortida(condicio); 
        }
        public void C_SenseResistencia(Estat.Condicio condicio) 
        { 
            if (Resistencia.Zero) Estat.Sortida(condicio); 
        }
        public void C_Relliscar(Estat.Condicio condicio) 
        { 
            if (Entorn.Buscar.Terra.EsRelliscant(transform) && 
                !Entorn.Buscar.Terra.HiHaEsglao(transform) && 
                CoyoteTime.Temps(Entorn.Buscar.Terra.EsRelliscant(transform), 0.05f))
            {
                CoyoteTime.Stop();
                Estat.Sortida(condicio); 
            }
                
        }
        public void C_RelliscarCoyoteTime(Estat.Condicio condicio) 
        { 
            if (Entorn.Buscar.Terra.EsRelliscant(transform) && 
                !Entorn.Buscar.Terra.HiHaEsglao(transform) && 
                CoyoteTime.Temps(Entorn.Buscar.Terra.EsRelliscant(transform), 0.5f))
            {
                CoyoteTime.Stop();
                Estat.Sortida(condicio); 
            }
                
        }
        public void C_NoRelliscar(Estat.Condicio condicio)
        { 
            if (!Entorn.Buscar.Terra.EsRelliscant(transform) && 
                CoyoteTime.Temps(!Entorn.Buscar.Terra.EsRelliscant(transform), 0.25f))
            {
                CoyoteTime.Stop();
                Estat.Sortida(condicio); 
            }
                
        }
        public void C_Esglao(Estat.Condicio condicio) 
        { 
            if (Entorn.Buscar.Terra.HiHaEsglao(transform) && 
                !Inputs.Saltar) 
                
                Estat.Sortida(condicio); 
        }


        private void OnDisable()
        {
            
        }
    }
}




