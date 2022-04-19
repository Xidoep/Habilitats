using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Moviment3D
{
    public static class CoyoteTime
    {
        public static bool activat;
        public static float temps;


        public static bool Temps(bool començarAContar, float temps)
        {
            if (!començarAContar)
            {
                activat = false;
                return false;
            }

            if (!activat)
            {
                activat = true;
                CoyoteTime.temps = temps;
                return false;
            }

            if (CoyoteTime.temps <= 0)
            {
                activat = false;
                return true;
            }

            CoyoteTime.temps -= Time.deltaTime;
            return false;
        }
        public static void Stop()
        {
            activat = false;
        }
    }
}

