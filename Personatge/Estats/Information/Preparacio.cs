using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public static class Preparacio
    {
        static Coroutine coroutine;
        static bool preparat = false;
        public static bool Preparat => preparat;

        public static float Preparar
        {
            set
            {
                preparat = false;
                coroutine.StopCoroutine();
                coroutine = XS_Coroutine.StartCoroutine_Ending(value, PreparatTrue);
            }
        }
        static void PreparatTrue() => preparat = true;
    }
}

