using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    public static class Preparacio
    {
        static bool preparat = false;
        public static bool Preparat => preparat;
        public static float Preparar
        {
            set
            {
                preparat = false;
                XS_Coroutine.Iniciar(value, () => { preparat = true; });
            }
        }

    }
}

