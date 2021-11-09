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

        Rigidbody rb;
        //Informacio info;
        //Rigidbody rb;
        //Transform camara;

        Vector3 moviment;

        [SerializeField] float forca;

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            Preparacio.Preparar = 0.25f;

            transform.SetParent(null);

            rb.velocity = Vector3.zero;
            rb.AddForce(((Vector3.up + (LaMevaCamara.Transform.ACamaraRelatiu(Inputs.Moviment) / 3f)) * forca + (Dinamic.VelocitatSalt)) * 50, ForceMode.Impulse);

            Resistencia.Saltar();
            Animacio.Saltar();
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
        }
    }

}
