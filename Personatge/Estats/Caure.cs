using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Caure : EstatPlayer
    {
        public override string ToString() => "Caure";

        Rigidbody rb;
        Vector3 moviment;

        public float debug;

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
            moviment = MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment) * Time.deltaTime;

            rb.AddForce((moviment * 20000) * Dinamic.MultiplicadorMovimentAeri(MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment)));

            if (!i.Inputs.Saltar)
            {
                rb.Gravetat();
            }

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, new Vector3(0, transform.localEulerAngles.z, 0).ToQuaternion(), 10);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            transform.Orientar(1);
            

            Animacio.VelocitatVertical();
        }



        public void C_VelocitatVerticalNegativa(Estat.Condicio condicio)
        {
            if (Dinamic.VelocitatVerticalNegativa) Estat.Sortida(condicio);
        }
        public void C_NoEscAire(Estat.Condicio condicio)
        {
            if (i.Inputs.Deixar &&
                Preparacio.Preparat &&
                !Entorn.Buscar.Terra.Hit(transform).Hitted())
            {
                Estat.Sortida(condicio);
                //Simple animcio de escalar a aire.
            }
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
        public void C_SaltarEscalantCaure(Estat.Condicio condicio)
        {
            if (Preparacio.Preparat &&
                CoyoteTime.Temps(Preparacio.Preparat, 0.4f))
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
    }

}
