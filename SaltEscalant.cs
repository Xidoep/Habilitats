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

        [SerializeField] float seguintParet = 1.2f;
        [SerializeField] float contraParet = 0.8f;
        [SerializeField] float ajupit = 0.7f;


        Vector3 DireccioSeguintParet => ((Inputs.GetHelperUp * Inputs.Moviment.y + Inputs.GetHelperRight * -Inputs.Moviment.x) * seguintParet + (Dinamic.VelocitatSalt)) * 50;
        Vector3 DireccioContraParet => ((Vector3.up - transform.forward) * contraParet + (Dinamic.VelocitatSalt)) * 50;
        Vector3 DireccioSaltAjupit => (((Vector3.up / 2f) + transform.forward) * ajupit + (Dinamic.VelocitatSalt)) * 50;

        RaycastHit paretHit;

        internal override void EnEntrar()
        {
            //if (info == null) info = GetComponent<Informacio>();
            //if (rb == null) rb = GetComponent<Rigidbody>();
            //if (camara == null) camara = info.Camara;
            if (rb == null) rb = GetComponent<Rigidbody>();

            Preparacio.Preparar = 0.25f;

            if (Inputs.GetHelperUp.Pla())  //En aquest cas .Pla() es fa servir per confirmar que la direcio de salt es cap AMUNT. (osigui, si estas escalant o ajupit)
            {
                if (Inputs.Moviment.y > 0f) SaltSeguintLaParet();
                else SaltContraLaParet();
            }
            else
            {
                SaltAjupit();
            }
            Resistencia.Saltar();

            Animacio.Saltar();
        }

        private void SaltSeguintLaParet()
        {
            direccioParet = true;
            rb.useGravity = false;
            rb.isKinematic = false;

            Debugar.Log("Salt seguint pret");
            rb.AddForce(DireccioSeguintParet, ForceMode.Impulse);
            Preparacio.Preparar = 0.5f;
            Inputs.SaltEscalantReenganxarse = true;
        }

        private void SaltContraLaParet()
        {
            direccioParet = false;
            rb.useGravity = true;
            rb.isKinematic = false;

            Debugar.Log("Salt contra pret");
            transform.SetParent(null);
            rb.AddForce(DireccioContraParet, ForceMode.Impulse);
            transform.forward = -transform.forward;
        }

        private void SaltAjupit()
        {
            direccioParet = false;
            rb.useGravity = true;
            rb.isKinematic = false;

            transform.SetParent(null);
            rb.AddForce(DireccioSaltAjupit, ForceMode.Impulse);
        }



        internal override void EnSortir()
        {
            rb.useGravity = true;
        }

        internal override void EnUpdate()
        {
            if (direccioParet)
            {
                SeguirLaParet();
            }
            else
            {
                moviment = LaMevaCamara.Transform.ACamaraRelatiu(Inputs.Moviment) * Time.deltaTime;
                rb.AddForce(moviment * 30);

                transform.Orientar(6);
            }

        }

        private void SeguirLaParet()
        {
            //Mirar la paret tot el ratu...
            //paretHit = Entorn.Buscar.Dret.Endevant(transform);
            //paretHit = Fisiques.Raig(transform.position + transform.up * 1, transform.forward, Entorn.capaEntorn, 1);
            /*if (paretHit.Impactat())
            {
                transform.forward = -paretHit.normal;
            }*/
        }
    }

}
