using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moviment3D
{
    public class Cap : MonoBehaviour
    {
        static Transform cap;
        public static Vector3 Posicio => cap.position;

        private void OnEnable()
        {
            cap = transform;
        }
    }
}

