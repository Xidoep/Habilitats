using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moviment3D
{
    public static class Resistencia
    {
        static float maxim = 10;
        static float actual = 10;

        public static bool testing;

        public static float Actual
        {
            set => actual = Mathf.Clamp(value, 0, maxim);
            get => !testing ? actual : maxim;
        }
        public static bool Zero => actual <= 0;

        public static void Saltar() => Actual -= 3;
        public static void NoBuidarDelTot() => Actual = Mathf.Max(Actual, 0.1f);

        public static void Recuperar() => Actual += 3 * Time.deltaTime;
        public static void RescuperarLentament() => Actual += 0.5f * Time.deltaTime;
    }
}

