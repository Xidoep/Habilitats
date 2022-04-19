using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class SaltEscalant : EstatPlayer
    {
        Rigidbody rb;
        //Informacio info;
        //Rigidbody rb;
        //Transform camara;

        Vector3 moviment;

        bool direccioParet;

        [SerializeField] float seguintParet = 0.9f;
        [SerializeField] float contraParet = 0.8f;
        [SerializeField] float ajupit = 0.7f;


        Vector3 DireccioSeguintParet => ((Inputs.GetHelperUp * (Inputs.Moviment.y + 0.1f) + Inputs.GetHelperRight * -Inputs.Moviment.x).normalized * seguintParet + (Dinamic.VelocitatSalt)) * 50;
        Vector3 DireccioContraParet => ((Vector3.up - transform.forward) * contraParet + (Dinamic.VelocitatSalt)) * 50;
        Vector3 DireccioSaltAjupit => (((Vector3.up / 2f) + transform.forward) * ajupit + (Dinamic.VelocitatSalt)) * 50;

        RaycastHit paretHit;
        RigidbodyConstraints currentConstraints;

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            
            currentConstraints = rb.constraints;

            if (Inputs.GetHelperUp.Pla())  //En aquest cas .Pla() es fa servir per confirmar que la direcio de salt es cap AMUNT. (osigui, si estas escalant o ajupit)
            {
                if (Inputs.Moviment.y > -0.1f || Mathf.Abs(Inputs.Moviment.x) > 0.45f) SaltSeguintLaParet();
                else SaltContraLaParet();
            }
            else
            {
                SaltAjupit();
            }

            Resistencia.SaltarFort();
        }



        private void SaltSeguintLaParet()
        {
            Preparacio.Preparar = 0.1f;

            direccioParet = true;
            rb.useGravity = false;

            Debugar.Log("Salt seguint pret");

            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.AddForce(DireccioSeguintParet, ForceMode.Impulse);
            Inputs.SaltEscalantReenganxarse = true;

            Animacio.Moviment(Inputs.Moviment);
            Animacio.SaltEscalant();
        }

        private void SaltContraLaParet()
        {
            Preparacio.Preparar = 0.25f;

            direccioParet = false;

            Debugar.Log("Salt contra pret");
            transform.SetParent(null);
            rb.AddForce(DireccioContraParet, ForceMode.Impulse);
            transform.forward = -transform.forward;

            Animacio.Saltar();
        }

        private void SaltAjupit()
        {
            Inputs.DisableEscalar();
            Preparacio.Preparar = 0.25f;
            
            direccioParet = false;

            transform.SetParent(null);
            rb.AddForce(DireccioSaltAjupit, ForceMode.Impulse);

            Animacio.SaltAjupid();
        }



        internal override void EnSortir()
        {
            rb.useGravity = true;
            rb.constraints = currentConstraints;
        }

        internal override void EnUpdate()
        {
            if (direccioParet)
            {
                SeguirLaParet();
            }
            else
            {
                moviment = MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment) * Time.deltaTime;
                rb.AddForce(moviment * 30);

                transform.Orientar(6);
            }

        }

        private void SeguirLaParet()
        {
            //Mirar la paret tot el ratu...
            //paretHit = Entorn.Buscar.Dret.Endevant(transform);
            paretHit = XS_Physics.Ray(transform.position + transform.up * 1, transform.forward, Entorn.capaEntorn, 1);
            if (paretHit.Hitted())
            {
                transform.forward = -paretHit.normal;
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
                //Aquí decidir quin tipus de salt es fa segons la condicio.
                //Animacio de salt consequent
            }
        }

        //Aixo es podria utilitzar igual que quan arribes al top.
        //Si deixes apretat el salt, continua saltant. si no, "s'enganxa" a la cantonada.
        public void C_TrobarCantonadaSuperior(Estat.Condicio condicio)
        {
            if (Preparacio.Preparat &&
                Entorn.Escalant.Buscar.CantonadaSuperior(transform).Hitted() &&
                Inputs.SaltEscalantReenganxarse)

                Estat.Sortida(condicio);
        }
    }

}
