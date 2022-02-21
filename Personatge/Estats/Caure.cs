using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Caure : Estat
    {
        Rigidbody rb;
        Vector3 moviment;

        public override string ToString() => "Caure";

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            transform.SetParent(null);
            
            Animacio.Caure();
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        internal override void EnSortir()
        {
            /*GameObject final = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            final.transform.position = transform.position;
            final.transform.localScale = Vector3.one * 0.25f;
            Destroy(final.GetComponent<SphereCollider>());*/
           
        }

        internal override void EnUpdate()
        {
            moviment = MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment) * Time.deltaTime;
            rb.AddForce(moviment * 4000);

            if (!Inputs.Saltar)
            {
                rb.Gravetat();
            }

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, new Vector3(0, transform.localEulerAngles.z, 0).ToQuaternion(), 10);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            transform.Orientar(1);
            

            Animacio.VelocitatVertical();
        }
    }

}
