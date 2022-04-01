using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class SaltEscalant : Estat
    {
        //Informacio info;
        //Rigidbody rb;
        //Transform camara;

        Vector3 moviment;

        bool direccioParet;

        [SerializeField] float seguintParet = 0.9f;
        [SerializeField] float contraParet = 0.8f;
        [SerializeField] float ajupit = 0.7f;


        Vector3 DireccioSeguintParet => ((i.Inputs.GetHelperUp * (i.Inputs.Moviment.y + 0.1f) + i.Inputs.GetHelperRight * -i.Inputs.Moviment.x).normalized * seguintParet + (i.Dinamic.VelocitatSalt)) * 50;
        Vector3 DireccioContraParet => ((Vector3.up - transform.forward) * contraParet + (i.Dinamic.VelocitatSalt)) * 50;
        Vector3 DireccioSaltAjupit => (((Vector3.up / 2f) + transform.forward) * ajupit + (i.Dinamic.VelocitatSalt)) * 50;

        RaycastHit paretHit;
        RigidbodyConstraints currentConstraints;

        internal override void EnEntrar()
        {
            currentConstraints = i.Dinamic.Rigidbody.constraints;

            if (i.Inputs.GetHelperUp.Pla())  //En aquest cas .Pla() es fa servir per confirmar que la direcio de salt es cap AMUNT. (osigui, si estas escalant o ajupit)
            {
                if (i.Inputs.Moviment.y > -0.1f || Mathf.Abs(i.Inputs.Moviment.x) > 0.45f) SaltSeguintLaParet();
                else SaltContraLaParet();
            }
            else
            {
                SaltAjupit();
            }

            i.Resistencia.SaltarFort();
        }



        private void SaltSeguintLaParet()
        {
            i.Preparacio.Preparar = 0.1f;

            direccioParet = true;
            i.Dinamic.Rigidbody.useGravity = false;

            Debugar.Log("Salt seguint pret");

            i.Dinamic.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            i.Dinamic.Rigidbody.AddForce(DireccioSeguintParet, ForceMode.Impulse);
            i.Inputs.SaltEscalantReenganxarse = true;

            i.Animacio.Moviment(i.Inputs.Moviment);
            i.Animacio.SaltEscalant();
        }

        private void SaltContraLaParet()
        {
            i.Preparacio.Preparar = 0.25f;

            direccioParet = false;

            Debugar.Log("Salt contra pret");
            transform.SetParent(null);
            i.Dinamic.Rigidbody.AddForce(DireccioContraParet, ForceMode.Impulse);
            transform.forward = -transform.forward;

            i.Animacio.Saltar();
        }

        private void SaltAjupit()
        {
            i.Inputs.DisableEscalar();
            i.Preparacio.Preparar = 0.25f;
            
            direccioParet = false;

            transform.SetParent(null);
            i.Dinamic.Rigidbody.AddForce(DireccioSaltAjupit, ForceMode.Impulse);

            i.Animacio.SaltAjupid();
        }



        internal override void EnSortir()
        {
            i.Dinamic.Rigidbody.useGravity = true;
            i.Dinamic.Rigidbody.constraints = currentConstraints;
        }

        internal override void EnUpdate()
        {
            if (direccioParet)
            {
                SeguirLaParet();
            }
            else
            {
                moviment = MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment) * Time.deltaTime;
                i.Dinamic.Rigidbody.AddForce(moviment * 30);

                transform.Orientar(i.Inputs.Moviment, 6);
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
            if (i.Inputs.Saltar)
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
            if (i.Preparacio.Preparat &&
                Entorn.Escalant.Buscar.CantonadaSuperior(transform).Hitted() &&
                i.Inputs.SaltEscalantReenganxarse)

                Estat.Sortida(condicio);
        }
    }

}
