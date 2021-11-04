﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public class Hab_Esc : EstatPlayer
    {
        Transform helper;
        Rigidbody rb;

        Vector3 posicioInicial;
        Quaternion rotacioInicial;

        float temps;
        bool enPosicio;

        [SerializeField] int velocitat;
        [SerializeField] bool reenganxat = false;

        Rigidbody otherRigidbody;
        ConfigurableJoint joint;





        public override string ToString() => "Escalar!";

        internal override void EnEntrar()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();

            PrepararRigidBody(true);
            if (!reenganxat) CrearHelper(Entorn.Buscar.Dret.OnComencarAEscalar(transform));
            //else 
            Inputs.SaltEscalantPreparat = false;
            Inputs.SaltEscalantReenganxarse = false;
            temps = 0;
            enPosicio = false;
            Preparacio.Preparar = 0.25f;
            reenganxat = false;

            Animacio.Escalar();
            Animacio.Vector2(Parametre.MovimentX, Parametre.MovimentY, Vector2.zero);

            IKs.Capturar(Vector2.zero);
        }

        internal override void EnSortir()
        {
            PrepararRigidBody(false);
            DestruirHelper();
            enPosicio = false;
            Preparacio.Preparar = 0.25f;

            transform.SetParent(null);

            if (joint != null) Destroy(joint);

            Animacio.Vector2(Parametre.MovimentX, Parametre.MovimentY, Vector2.zero);
            IKs.Apagar();
        }

        internal override void EnUpdate()
        {
            if (!Preparacio.Preparat && Inputs.Escalar) Preparacio.Preparar = 0.25f;

            if (!enPosicio) Desplacar();
            else Quiet();
        }

        void Desplacar()
        {
            temps += Time.deltaTime * velocitat * (1 - Mathf.Clamp01(Vector3.Dot(helper.forward, Vector3.down)));
            transform.localPosition = Vector3.Lerp(posicioInicial, helper.localPosition, temps);
            if (!helper.forward.Pla()) transform.rotation = Quaternion.Slerp(rotacioInicial, Quaternion.LookRotation(-helper.forward), temps);

            if (temps > 1)
            {
                enPosicio = true;
                Inputs.SetHelperVectors = helper;

                Animacio.Bool(Parametre.Moviment, true);
                Animacio.Vector2(Parametre.MovimentX, Parametre.MovimentY, Vector2.zero);

            }
            else IKs.Actualitzar(temps);

            if (!helper.forward.Pla()) Resistencia.Actual -= 1 * Time.deltaTime;
        }
        void Quiet()
        {
            if (Inputs.Saltar)
            {
                if (helper.forward.Pla()) transform.Orientar(10);
                Inputs.SaltEscalantPreparat = true;
                return;
            }
            else
            {
                if (!helper.forward.Pla())
                {
                    helper.rotation = Quaternion.Euler(helper.eulerAngles.x, helper.eulerAngles.y, 0);
                    transform.rotation = Quaternion.LookRotation(-helper.forward);
                    Resistencia.Actual -= 0.1f * Time.deltaTime;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                }
            }

            if (Inputs.Moviment != Vector2.zero && Entorn.Escalant.Moviment(helper, helper.forward.Pla(), Inputs.Moviment).Impactat())
            {
                

                PosicionarHelper(Entorn.Escalant.Moviment(helper, helper.forward.Pla(), Inputs.Moviment));
                if (helper.forward.Pla()) transform.forward = LaMevaCamara.Transform.ACamaraRelatiu(Inputs.Moviment);
                else IKs.Capturar(Inputs.Moviment * 0.5f);
                temps = 0;
                enPosicio = false;

                Animacio.Bool(Parametre.Moviment, true);
                Animacio.Vector2(Parametre.MovimentX, Parametre.MovimentY, Inputs.Moviment);


            }

        }

        void CrearHelper(RaycastHit hit)
        {
            helper = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            helper.localScale = Vector3.one * 0.3f;

            otherRigidbody = hit.collider.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                joint = gameObject.AddComponent<ConfigurableJoint>();
                joint.connectedBody = otherRigidbody;
            }

            PosicionarHelper(hit);
        }
        void PosicionarHelper(RaycastHit hit)
        {
            helper.SetParent(hit.collider.transform);
            transform.SetParent(hit.collider.transform);

            posicioInicial = transform.localPosition;
            rotacioInicial = transform.rotation;

            helper.forward = hit.normal;
            helper.position = hit.point + (!helper.forward.Pla() ? hit.normal * 0.4f : Vector3.zero);
            Entorn.HitNormal(ref hit, helper);
            helper.forward = hit.normal;
        }
        void PrepararRigidBody(bool esc)
        {

            rb.useGravity = !esc;
            rb.isKinematic = esc;
            if (esc) rb.velocity = Vector3.zero;
        }

        void DestruirHelper()
        {
            Inputs.SetHelperVectors = helper;
            Destroy(helper.gameObject);
        }
        public void C_TrobarCantonadaSuperior(Estat.Condicio condicio) 
        {
            /* if (info.Preparat && entorn.BuscarCantonadaSuperior(transform).Impactat() && info.Preparat)
             {
                 reenganxat = true;
                 info.Resistencia = Mathf.Max(info.Resistencia, 0.1f);
                 CrearHelper(entorn.BuscarCantonadaSuperior(transform));
                 Estat.Sortida(condicio);
             }*/
            if (Preparacio.Preparat && Entorn.Escalant.Buscar.CantonadaSuperior(transform).Impactat())
            {
                reenganxat = true;
                Resistencia.NoBuidarDelTot();
                CrearHelper(Entorn.Escalant.Buscar.CantonadaSuperior(transform));
                Estat.Sortida(condicio);
            }
        }
    }

}
