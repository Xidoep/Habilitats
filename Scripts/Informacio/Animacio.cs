using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public static class Animacio
    {
        const string dret = "Dret";
        const string dreta = "Dreta";
        const string pla = "Pla";
        const string saltar = "Salt";
        const string caure = "Caure";
        const string escalar = "Escalar";
        const string relliscar = "Relliscar";
        const string velocitatVertical = "VelocitatVertical";
        const string moviment = "Moviment";
        const string movimentX = "MovimentX";
        const string movimentY = "MovimentY";
        const string saltPreparat = "SaltPreparat";
        const string saltSjupit = "SaltAjupit";
        const string saltEscalant = "SaltEscalant";
        const string reenganxarCantondaSuperior = "ReenganxarCantondaSuperior";

        static int dretHash;

        static bool aireFlanc;

        public static void Iniciar(Transform _transform)
        {
            animator = _transform.GetComponentInChildren<Animator>();
            dretHash = Animator.StringToHash(dret);
        }
        //REFERENCIES
        static Animator animator;


        public static void Dret()
        {
            Trigger(dretHash);
        }
        public static void Dreta(bool _dreta)
        {
            Bool(dreta, _dreta);
        }
        public static void Saltar()
        {
            Trigger(saltar);
        }
        public static void Caure()
        {
            Trigger(caure);
        }
        public static void Escalar()
        {
            Trigger(escalar);
        }
        public static void Relliscar()
        {
            Trigger(relliscar);
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
        public static void SaltPreparat(bool _saltPreparat)
        {
            Bool(saltPreparat, _saltPreparat);
        }
        public static void SaltAjupid()
        {
            Trigger(saltSjupit);
        }
        public static void SaltEscalant()
        {
            Trigger(saltEscalant);
        }
        public static void ReenganxarCantondaSuperior()
        {
            Trigger(reenganxarCantondaSuperior);
        }


        static bool Aire(Transform transform)
        {
            return !Entorn.Buscar.Terra.Hit(transform).Hitted() &&
             !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
               Preparacio.Preparat;
        }

        static void Trigger(string parametre)
        {
            if (!animator) 
                return;

            animator.SetTrigger(parametre);
        }

        static void Trigger(int parametre)
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

