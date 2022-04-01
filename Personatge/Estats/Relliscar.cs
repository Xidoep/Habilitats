using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Relliscar : Estat
    {
        [SerializeField] int velocitat;
        Transform helper;

        float inersia;

        //float Inersia { get => inersia; set => inersia = Mathf.Clamp01(value); }
        float Inersia { get => inersia; set => inersia = Mathf.Clamp(value, 0, 2); }

        Vector3 Direccio => -helper.right * (Vector3.Dot(MyCamera.Transform.ACamaraRelatiu(i.Inputs.Moviment.ToVector3_Vertical()), -helper.right) * 1f);

        internal override void EnEntrar()
        {
            Inersia = 0;
            CrearHelper(Entorn.Buscar.Terra.Hit(transform));

            i.Preparacio.Preparar = 0.5f;
            i.Animacio.Relliscar();
           
        }

        internal override void EnSortir()
        {
            //info.SetVelocitat = rb.velocity;
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            
        }

        internal override void EnUpdate()
        {
            Inersia += Time.deltaTime * 5f;

            if (Entorn.Buscar.Terra.Hit(transform).Hitted()) OrientarHelper(Entorn.Buscar.Terra.Hit(transform).normal);

            //rb.MovePosition(rb.transform.position + -helper.up * velocitat * Inersia * Time.deltaTime);
            //transform.position += (-helper.up + Direccio) * Time.deltaTime * velocitat * Inersia;
            transform.position += (Entorn.Buscar.Terra.InclinacioForward(transform) + Direccio) * Time.deltaTime * velocitat * Inersia;
            transform.RotateToQuaternionSmooth((-helper.up).ToQuaternion(Vector3.up), 10);
            i.Dinamic.Actualitzar(transform);
        }

        void CrearHelper(RaycastHit hit)
        {
            helper = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            OrientarHelper(hit.normal);
        }
        void OrientarHelper(Vector3 normal)
        {
            helper.forward = normal;
        }



        public void C_Relliscar(Estat.Condicio condicio)
        {
            if (Entorn.Buscar.Terra.EsRelliscant(transform) &&
                !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
                i.CoyoteTime.Temps(Entorn.Buscar.Terra.EsRelliscant(transform), 0.05f))
            {
                i.CoyoteTime.Stop();
                Estat.Sortida(condicio);
            }

        }
        public void C_RelliscarCoyoteTime(Estat.Condicio condicio)
        {
            if (Entorn.Buscar.Terra.EsRelliscant(transform) &&
                !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
                i.CoyoteTime.Temps(Entorn.Buscar.Terra.EsRelliscant(transform), 0.5f))
            {
                //FALTA ANIM PER MOSTRAR QUE S'ESTA APUNT DE CAURE
                i.CoyoteTime.Stop();
                Estat.Sortida(condicio);
            }

        }
    }
}

