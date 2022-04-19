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
        static Vector3 tmpCalculs;

        public static Vector3 Velocitat => velocitat;
        public static Vector3 VelocitatSalt => velocitat * 30;
        public static Vector3 VelocitatGravetat => velocitat * 4;
        public static bool VelocitatVerticalNegativa => velocitat.y < 0;
        public static Vector3 VelocitatHoritzontal => new Vector3(velocitat.x, 0, velocitat.z);

        //Retorna un numero entre 0 i 1 per multiplicar al moviment extra aeri, per evitar desplaçaments massa llargs pero permetent control.
        //Aquest valor te en compte la velocitat actual i si l'input va en la mateixa direccio que la velocitat.
        public static float MultiplicadorMovimentAeri(Vector3 moviment) => 
            Mathf.Max(
                1 - Mathf.Clamp01(Vector3.Dot(VelocitatHoritzontal.normalized, moviment) * 2), 
                1 - Mathf.Clamp01(VelocitatHoritzontal.magnitude * 35));



        /// <summary>
        /// Assigna la velocitat actual a partir de la diferencia de posico amb el frame anterior.
        /// </summary>
        public static void Actualitzar(Transform transform)
        {
            velocitat = transform.position - posicioFameAnterior;
            posicioFameAnterior = transform.position;
        }

        /// <summary>
        /// asigna la velocitat actual amb la interpolacio de la diferencia de posicions dels 3 frames anteriors.
        /// </summary>
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

            tmpCalculs = transform.position - frames[0];
            for (int i = 1; i < frames.Count; i++)
            {
                tmpCalculs += frames[i - 1] - frames[i];
            }
            tmpCalculs /= frames.Count;
            velocitat = tmpCalculs;

            frames.Add(transform.position);
            if (frames.Count > 3) frames.RemoveAt(0);
        }
        public static void Stop()
        {
            frames.Clear();
            tmpCalculs = Vector3.zero;
            velocitat = Vector3.zero;
        }
    }
}

