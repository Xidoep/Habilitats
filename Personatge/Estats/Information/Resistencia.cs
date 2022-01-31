using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moviment3D
{
    public static class Resistencia
    {
        static Canal_FloatFloat canal;
        public static Canal_FloatFloat Canal { set { canal = value; } }

        static float maxim = 10;
        static float actual = 10;

        public static bool testing;

        static float Actual
        {
            set 
            {
                actual = Mathf.Clamp(value, 0, maxim);
                canal?.Invocar(actual, maxim);
            } 
            get => !testing ? actual : maxim;
        }
        public static bool Zero => actual <= 0;
        public static bool UnaMica => actual > 0.1f;

        public static float Percentatge => actual / maxim;

        public static void SaltarFort() 
        {
            Actual -= 2;
            Actual = Mathf.Max(Actual, 0.1f);                                                                                                                                                                                                           
        }
        public static void Saltar()
        {
            Actual -= 1;
        }
        public static void NoBuidarDelTot() => Actual = Mathf.Max(Actual, 0.1f);

        public static void Gastar() => Actual -= 0.75f * Time.deltaTime;
        public static void GastarLentament() => Actual -= 0.05f * Time.deltaTime;

        public static void Recuperar() => Actual += 3 * Time.deltaTime;
        public static void RescuperarLentament() => Actual += 0.5f * Time.deltaTime;
    }
}

