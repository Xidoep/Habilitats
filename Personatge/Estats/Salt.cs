using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Salt : EstatPersonatge
    {
        Vector3 moviment;

        [SerializeField] float forca;
        Vector3 ForcaLateral => Entorn.Buscar.Dret.EndevantAprop(transform).Hitted() ? Vector3.zero : (MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment) / 3f);
        //bool ObjecteDevant => XS_Physics.Ray(transform.position + transform.up, MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment), 1, Entorn.capaEntorn).Hitted();

        internal override void EnEntrar()
        {
            i.Preparacio.Preparar = 0.25f;

            transform.SetParent(null);

            //rb.velocity = Vector3.zero;


            i.Dinamic.Rigidbody.AddForce(((Vector3.up + ForcaLateral) * forca + (i.Dinamic.VelocitatSalt)) * 50, ForceMode.Impulse);

            i.Resistencia.Saltar();
            i.Animacio.Saltar();
        }

        internal override void EnUpdate()
        {
            i.Animacio.VelocitatVertical(i.Dinamic.VelocitatGravetat.y);

            moviment = MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment) * Time.deltaTime * 2;
            if (!Entorn.Buscar.Dret.EndevantAprop(transform).Hitted())
            {
                i.Dinamic.Rigidbody.AddForce((moviment * 20000) * i.Dinamic.MultiplicadorMovimentAeri(MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment)));
            }


            if (!i.Inputs.Saltar)
            {
                i.Dinamic.Rigidbody.Gravetat();
            }

            transform.Orientar(i.Inputs.Moviment,6);

           
            Debugar.DrawRay(
                transform.position + transform.up,
                MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment).normalized,
            XS_Physics.Ray(
                transform.position + transform.up,
                MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment), 
                1, 
                Entorn.capaEntorn).Hitted() ? Color.green : Color.red);
        }

        internal override void EnSortir()
        {
            //info.SetVelocitat = rb.velocity;
        }



        public void C_Saltar(Estat.Condicio condicio)
        {
            if (i.Inputs.Saltar &&
                i.Resistencia.UnaMica &&
                i.Preparacio.Preparat)
            {
                Estat.Sortida(condicio);
                //Aqui s'ha de comprovar si està en moviment o no. per posar una anim o una altre.
                //Animacio Saltar simple
            }

        }
    }

}
