using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moviment3D
{
    public static class Vida
    {
        public static void Iniciar(Canal_Void _canalMort, Canal_Bool _canalTrencat)
        {
            canalMort = _canalMort;
            canalTrencat = _canalTrencat;
        }

        static Canal_Void canalMort;
        static Canal_Bool canalTrencat;
        static bool trencat;

        public static void Damage()
        {
            if (!trencat)
            {
                trencat = true;
                canalTrencat.Invocar(trencat);
                return;
            }

            trencat = false;
            canalMort.Invocar();
        }

        public static void Restore()
        {
            trencat = false;
            canalTrencat.Invocar(trencat);
        }
    }
}

