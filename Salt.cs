using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Salt : EstatPlayer
    {
        const string SALT = "Salt";
        const string MOVIMENY_Y = "MovimentY";
        const string VELOCITAT_VERTICAL = "VelocitatVertical";
        public override string ToString() => "Salt";

        //Informacio info;
        //Rigidbody rb;
        //Transform camara;

        Vector3 moviment;

        [SerializeField] float forca;

        internal override void EnEntrar()
        {
            //if (info == null) info = GetComponent<Informacio>();
            //if (rb == null) rb = GetComponent<Rigidbody>();
            //if (camara == null) camara = info.Camara;

            Preparacio.Preparar = 0.25f;

            transform.SetParent(null);

            Resistencia.Saltar();
            rb.velocity = Vector3.zero;
            //rb.velocity = info.Velocitat;
            rb.AddForce(((Vector3.up + (LaMevaCamara.Transform.ACamaraRelatiu(Inputs.Moviment) / 3f)) * forca + (Dinamic.VelocitatSalt)) * 50, ForceMode.Impulse);

            Animacio.Tigger(Parametre.Saltar);
        }

        internal override void EnSortir()
        {
            //info.SetVelocitat = rb.velocity;
            
        }

        internal override void EnUpdate()
        {
            moviment = LaMevaCamara.Transform.ACamaraRelatiu(Inputs.Moviment) * Time.deltaTime;
            rb.AddForce(moviment * 400);

            if (!Inputs.Saltar)
            {
                rb.Gravetat();
            }

            transform.Orientar(6);

            Animacio.Float(Parametre.VelocitatVertical, Dinamic.VelocitatGravetat.y);
            //Animator.SetFloat(VELOCITAT_VERTICAL, rb.velocity.y * 4);
        }
    }

}
