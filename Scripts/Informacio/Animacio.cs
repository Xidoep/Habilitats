using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public static class Animacio
    {
        const string dret = "Dret";
        const string pla = "Pla";
        const string saltar = "Saltar";
        const string caure = "Caure";
        const string escalar = "Escalar";
        const string relliscar = "Relliscar";
        const string velocitatVertical = "VelocitatVertical";
        const string moviment = "Moviment";
        const string movimentX = "MovimentX";
        const string movimentY = "MovimentY";

        static bool aireFlanc;

        public static void Iniciar(Transform _transform)
        {
            animator = _transform.GetComponentInChildren<Animator>();
        }
        //REFERENCIES
        static Animator animator;


        public static void Dret()
        {
            Tigger(dret);
        }
        public static void Saltar()
        {
            Tigger(saltar);
        }
        public static void Caure()
        {
            Tigger(caure);
        }
        public static void Escalar()
        {
            Tigger(escalar);
        }
        public static void Relliscar()
        {
            Tigger(relliscar);
        }
        public static void NoTerra(Transform transform)
        {
            if (Aire(transform) && !aireFlanc)
            {
                Caure();
                Float(velocitatVertical, Dinamic.VelocitatGravetat.y);
                aireFlanc = true;
            }
            else if(!Aire(transform) && aireFlanc)
            {
                Dret();
                aireFlanc = false;
            }
        }
        public static void VelocitatVertical()
        {
            Float(velocitatVertical, Dinamic.VelocitatGravetat.y);
        }
        public static void Moviment(Vector2 valor)
        {
            Float(movimentX, valor.x);
            Float(movimentY, valor.y);
        }
        public static void MovimentY(float valor)
        {
            Float(movimentY, valor);
        }
        public static void EnMoviment(bool enMoviment)
        {
            if (animator.GetBool(moviment).Equals(enMoviment))
                return;

            Bool(moviment, enMoviment);
        }
        public static void Pla(bool _pla)
        {
            Bool(pla, _pla);
        }


        static bool Aire(Transform transform)
        {
            return !Entorn.Buscar.Terra.Hit(transform).Hitted() &&
             !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
               Preparacio.Preparat;
        }

        static void Tigger(string parametre)
        {
            if (!animator) 
                return;

            animator.SetTrigger(parametre);
        }

        static void Float(string parametre, float valor)
        {
            animator.SetFloat(parametre, valor);
        }

        static void Bool(string parametre, bool valor)
        {
            animator.SetBool(parametre, valor);
        }

    }

}

