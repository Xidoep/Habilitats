using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Caure : EstatPersonatge
    {
        public override string ToString() => "Caure";

        Vector3 moviment;

        float timer = 0;

        internal override void EnEntrar()
        {
            transform.SetParent(null);

            i.Animacio.Caure();
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            i.Dinamic.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            timer = 1;
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
            moviment = MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment) * Time.deltaTime * Mathf.Clamp01(timer);

            i.Dinamic.Rigidbody.AddForce((moviment * 20000) * i.Dinamic.MultiplicadorMovimentAeri(MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment)));

            if (!i.Inputs.Saltar)
            {
                i.Dinamic.Rigidbody.Gravetat();
            }

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, new Vector3(0, transform.localEulerAngles.z, 0).ToQuaternion(), 10);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            transform.Orientar(i.Inputs.Moviment,6);


            i.Animacio.VelocitatVertical(i.Dinamic.VelocitatGravetat.y);

            timer -= Time.deltaTime;
        }



        public void C_VelocitatVerticalNegativa(Estat.Condicio condicio)
        {
            if (i.Dinamic.VelocitatVerticalNegativa) Estat.Sortida(condicio);
        }
        public void C_NoEscAire(Estat.Condicio condicio)
        {
            if (i.Inputs.Deixar &&
                i.Preparacio.Preparat &&
                !Entorn.Buscar.Terra.Hit(transform).Hitted())
            {
                Estat.Sortida(condicio);
                //Simple animcio de escalar a aire.
            }
        }
        public void C_NoTerra(Estat.Condicio condicio)
        {
            if (Entorn.Buscar.Terra.Hit(transform).Hitted())
            {
                if (!Entorn.Buscar.Terra.Hit(transform).normal.Pla() &&
                i.CoyoteTime.Temps(!Entorn.Buscar.Terra.Hit(transform).Hitted(), 0.02f) &&
                i.Preparacio.Preparat)
                {
                    i.CoyoteTime.Stop();
                    Estat.Sortida(condicio);
                }
            }
            else
            {
                if (!Entorn.Buscar.Terra.HiHaEsglao(transform) &&
                i.CoyoteTime.Temps(!Entorn.Buscar.Terra.Hit(transform).Hitted(), 0.02f) &&
                i.Preparacio.Preparat)
                {
                    i.CoyoteTime.Stop();
                    Estat.Sortida(condicio);
                }
            }
            /*if ((!Entorn.Buscar.Terra.Hit(transform).Hitted() || !Entorn.Buscar.Terra.Hit(transform).normal.Pla()) &&
                !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
                i.CoyoteTime.Temps(!Entorn.Buscar.Terra.Hit(transform).Hitted(), 0.02f) &&
                i.Preparacio.Preparat)
            {
                i.CoyoteTime.Stop();
                Estat.Sortida(condicio);
            }*/

        }
        public void C_SaltarEscalantCaure(Estat.Condicio condicio)
        {
            if (i.Preparacio.Preparat &&
                i.CoyoteTime.Temps(i.Preparacio.Preparat, 0.4f))
            {
                i.CoyoteTime.Stop();
                Estat.Sortida(condicio);
            }

        }
        public void C_NoParetDevant(Estat.Condicio condicio)
        {
            if (!Entorn.Buscar.Dret.OnComencarAEscalar_Aire(transform, out float innecessari).Hitted() &&
                i.Preparacio.Preparat)
            {
                Estat.Sortida(condicio);
            }
        }
    }

}
