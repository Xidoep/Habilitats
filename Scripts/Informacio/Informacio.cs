using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using XS_Utils;

namespace Moviment3D
{
    [SelectionBase]
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

        [SerializeField] bool testing;

        [SerializeField] List<Vector3> frames;
        [SerializeField] Vector3 velocitat;

        private void OnEnable()
        {
            Inputs.Iniciar(moviment, saltar, agafar, deixar);
            Entorn.Iniciar(capaEntorn);
            Animacio.Iniciar(transform);
            Resistencia.testing = testing;
            IKs.Iniciar(transform, capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
        }

        private void Update()
        {
            IKs.Debug();
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
                Entorn.Buscar.Dret.OnComencarAEscalar(transform).Impactat() &&
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
                Entorn.Buscar.Terra.Hit(transform).Impactat()) 
                
                Estat.Sortida(condicio); 
        }
        public void C_NoEscAire(Estat.Condicio condicio)
        {
            if (Inputs.Deixar &&
                Preparacio.Preparat &&
                !Entorn.Buscar.Terra.Hit(transform).Impactat()) 
                
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
            if (Entorn.Buscar.Terra.Hit(transform).Impactat() && 
                Preparacio.Preparat && 
                !Entorn.Buscar.Terra.EsRelliscant(transform)) 
                
                Estat.Sortida(condicio); 
        }
        public void C_NoTerra(Estat.Condicio condicio) 
        { 
            if (!Entorn.Buscar.Terra.Hit(transform).Impactat() && 
                !Entorn.Buscar.Terra.HiHaEsglao(transform) && 
                CoyoteTime.Temps(!Entorn.Buscar.Terra.Hit(transform).Impactat(), 0.02f) && 
                Preparacio.Preparat) 
                
                Estat.Sortida(condicio);
        }
        public void C_Salt(Estat.Condicio condicio) 
        { 
            if (Inputs.Saltar && 
                Resistencia.Actual > 0.1f && 
                Preparacio.Preparat) 
                
                Estat.Sortida(condicio); 
        }
        public void C_SaltarEscalant(Estat.Condicio condicio) 
        { 
            if (!Inputs.Saltar && 
                Inputs.SaltEscalantPreparat) 
                
                Estat.Sortida(condicio);
        }
        public void C_SaltarEscalantReencangarse(Estat.Condicio condicio) 
        {
            if (Inputs.Escalar && 
                Entorn.Buscar.Dret.OnComencarAEscalar(transform).Impactat()) 
                
                Estat.Sortida(condicio); 
        }
        public void C_SaltarEscalantCaure(Estat.Condicio condicio) 
        {
            if (Preparacio.Preparat && 
                CoyoteTime.Temps(Preparacio.Preparat, 0.5f)) 
                
                Estat.Sortida(condicio); 
        }
        public void C_TornarAAgafarSiPot(Estat.Condicio condicio) 
        { 
            if (Preparacio.Preparat && 
                Entorn.Buscar.Dret.OnComencarAEscalar(transform).Impactat()) 
                
                Estat.Sortida(condicio); 
        }
        public void C_TrobarCantonadaSuperior(Estat.Condicio condicio) 
        { 
            if (Preparacio.Preparat && 
                Entorn.Escalant.Buscar.CantonadaSuperior(transform).Impactat() &&
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
                
                Estat.Sortida(condicio); 
        }
        public void C_RelliscarCoyoteTime(Estat.Condicio condicio) 
        { 
            if (Entorn.Buscar.Terra.EsRelliscant(transform) && 
                !Entorn.Buscar.Terra.HiHaEsglao(transform) && 
                CoyoteTime.Temps(Entorn.Buscar.Terra.EsRelliscant(transform), 0.5f)) 
                
                Estat.Sortida(condicio); 
        }
        public void C_NoRelliscar(Estat.Condicio condicio)
        { 
            if (!Entorn.Buscar.Terra.EsRelliscant(transform) && 
                CoyoteTime.Temps(!Entorn.Buscar.Terra.EsRelliscant(transform), 0.25f)) 
                
                Estat.Sortida(condicio); 
        }
        public void C_Esglao(Estat.Condicio condicio) 
        { 
            if (Entorn.Buscar.Terra.HiHaEsglao(transform) && 
                !Inputs.Saltar) 
                
                Estat.Sortida(condicio); 
        }

    }
}




