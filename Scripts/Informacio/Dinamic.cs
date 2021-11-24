using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moviment3D
{
    public static class Dinamic
    {
        static List<Vector3> frames;
        static Vector3 posicioFameAnterior;
        static Vector3 velocitat;

        public static Vector3 Velocitat => velocitat;
        public static Vector3 VelocitatSalt => velocitat * 15;
        public static Vector3 VelocitatGravetat => velocitat * 4;
        public static bool VelocitatVerticalNegativa => velocitat.y < 0;

        static Vector3 tmp;

        public static void Actualitzar(Transform transform)
        {
            velocitat = transform.position - posicioFameAnterior;
            posicioFameAnterior = transform.position;
        }

        public static void ActualitzarSmooth(Transform transform)
        {
            if (frames == null) frames = new List<Vector3>();
            if (frames.Count == 0)
            {
                velocitat = transform.position - posicioFameAnterior;
                posicioFameAnterior = transform.position;
                frames.Add(transform.position);
                return;
            }

            tmp = transform.position - frames[0];
            for (int i = 1; i < frames.Count; i++)
            {
                tmp += frames[i - 1] - frames[i];
            }
            tmp /= frames.Count;
            velocitat = tmp;

            frames.Add(transform.position);
            if (frames.Count > 3) frames.RemoveAt(0);
        }
    }
}

