using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Caure : EstatPlayer
    {
        Rigidbody rb;
        Vector3 moviment;

        public override string ToString() => "Caure";

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            transform.SetParent(null);
            Animacio.Caure();
        }

        internal override void EnSortir()
        {
            GameObject final = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            final.transform.position = transform.position;
            final.transform.localScale = Vector3.one * 0.25f;
            Destroy(final.GetComponent<SphereCollider>());
        }

        internal override void EnUpdate()
        {
            moviment = LaMevaCamara.Transform.ACamaraRelatiu(Inputs.Moviment) * Time.deltaTime;
            rb.AddForce(moviment * 200);

            Resistencia.RescuperarLentament();

            if (!Inputs.Saltar)
            {
                rb.Gravetat();
            }

            transform.Orientar(4);


            Animacio.Float(Parametre.VelocitatVertical, Dinamic.VelocitatGravetat.y);
        }
    }

}
