using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{

    public static class Animacio
    {
        public static void Iniciar(Transform _transform)
        {
            animator = _transform.GetComponentInChildren<Animator>();
        }
        //REFERENCIES
        static Animator animator;

        //PRIVADES
        static string accioActual;
        static string tmp;

        public static void Dret()
        {
            Tigger("Dret");
        }
        public static void Saltar()
        {
            Tigger("Saltar");
        }
        public static void Caure()
        {
            Tigger("Caure");
        }
        public static void Escalar()
        {
            Tigger("Escalar");
        }
        public static void Relliscar()
        {
            Tigger("Relliscar");
        }

        public static void NoTerra(Transform transform)
        {
            if (!Entorn.Buscar.Terra.Hit(transform).Impactat() &&
               !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
               Preparacio.Preparat)
            {
                Caure();
                Float(Parametre.VelocitatVertical, Dinamic.VelocitatGravetat.y);
            }
            else Dret();
        }




        static void Tigger(string parametre)
        {
            //Debugar.Log($"Trigger ({parametre})");
            if (!animator) return;
            //if (IgualActual(parametre)) return;

            animator.SetTrigger(parametre);
            //accioActual = tmp;
        }
        public static void Bool(Parametre parametre, bool valor) 
        {
            if (GetBool(parametre) == valor) return;

            animator.SetBool(parametre.ToString(), valor);
        }
        
        public static void Float(Parametre parametre, float valor) => animator.SetFloat(parametre.ToString(), valor);
        public static float GetFloat(Parametre parametre) => animator.GetFloat(parametre.ToString());
        public static void Vector2(Parametre x, Parametre y, Vector2 valor)
        {
            animator.SetFloat(x.ToString(), valor.x);
            animator.SetFloat(y.ToString(), valor.y);
        }
        public static void EsperarFinalAnimacio(System.Action accio) => Corrutina.Iniciar(TempsAnimacioActual, accio);




        //PRIVADES
        static bool GetBool(Parametre parametre) => animator.GetBool(parametre.ToString());
        static float TempsAnimacioActual => animator.GetCurrentAnimatorStateInfo(0).length;
        static bool IgualActual(Parametre parametre) => IgualActual(parametre.ToString());
        static bool IgualActual(Clip clip) => IgualActual(clip.ToString());
        static bool IgualActual(string animacio)
        {
            tmp = animacio;
            if (accioActual != null)
                return accioActual.Equals(animacio);
            else return false;
        }
    }

}

