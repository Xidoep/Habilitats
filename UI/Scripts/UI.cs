using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    [CreateAssetMenu(menuName = "Xido Studio/Moviment3D/UI")]
    public class UI : ScriptableObject
    {
        [System.Serializable]
        public class Emergent
        {
            [SerializeField] GameObject prefab;

            bool esperant;
            bool mostrat;
            Vector3 posicio;
            GameObject instanciat;

            Coroutine proces;

            public void Mostrar(Vector3 posicio, float temps)
            {
                if (mostrat)
                    return;

                if (!this.posicio.Equals(posicio)) this.posicio = posicio;

                if (esperant)
                    return;

                esperant = true;
                proces = XS_Coroutine.StartCoroutine(temps, Mostrar);
            }

            void Mostrar()
            {
                mostrat = true;
                esperant = false;
                instanciat = GameObject.Instantiate(prefab, posicio, Quaternion.identity);
            }

            public void Amagar()
            {
                if (!mostrat)
                {
                    if (esperant) 
                    {
                        if(proces != null)
                            proces.StopCoroutine();
                    } 
                    return;
                }

                mostrat = false;
                esperant = false;
                GameObject.Destroy(instanciat);
            }
        }

        public Emergent forat;
        public Emergent paret;



    }
   
   
}

