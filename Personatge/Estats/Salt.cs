using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Salt : EstatPlayer
    {
        Rigidbody rb;

        Vector3 moviment;

        [SerializeField] float forca;
        Vector3 ForcaLateral => Entorn.Buscar.Dret.EndevantAprop(transform).Hitted() ? Vector3.zero : (MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment) / 3f);
        //bool ObjecteDevant => XS_Physics.Ray(transform.position + transform.up, MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment), 1, Entorn.capaEntorn).Hitted();

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            Preparacio.Preparar = 0.25f;

            transform.SetParent(null);

            //rb.velocity = Vector3.zero;


            rb.AddForce(((Vector3.up + ForcaLateral) * forca + (Dinamic.VelocitatSalt)) * 50, ForceMode.Impulse);
            
            info.Resist.Saltar();
            Animacio.Saltar();
        }

        internal override void EnUpdate()
        {
            Animacio.VelocitatVertical();

            moviment = MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment) * Time.deltaTime * 2;
            if (!Entorn.Buscar.Dret.EndevantAprop(transform).Hitted())
            {
                rb.AddForce((moviment * 20000) * Dinamic.MultiplicadorMovimentAeri(MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment)));
            }


            if (!Inputs.Saltar)
            {
                rb.Gravetat();
            }

            transform.Orientar(6);

           
            Debugar.DrawRay(
                transform.position + transform.up,
                MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment).normalized,
            XS_Physics.Ray(
                transform.position + transform.up,
                MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment), 
                1, 
                Entorn.capaEntorn).Hitted() ? Color.green : Color.red);
        }

        internal override void EnSortir()
        {
            //info.SetVelocitat = rb.velocity;
        }



        public void C_Saltar(Estat.Condicio condicio)
        {
            if (Inputs.Saltar) Debugar.Log("Inputs.Saltar");
            if (info.Resist.UnaMica) Debugar.Log("Resistencia.UnaMica");
            if (Preparacio.Preparat) Debugar.Log("Preparacio.Preparat");
            if (Inputs.Saltar &&
                info.Resist.UnaMica &&
                Preparacio.Preparat)
            {
                Debugar.Log("Saltar!");
                Estat.Sortida(condicio);
                //Aqui s'ha de comprovar si està en moviment o no. per posar una anim o una altre.
                //Animacio Saltar simple
            }

        }
    }

}
