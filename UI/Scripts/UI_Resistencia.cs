using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XS_Utils;
using Esdeveniment;

namespace Moviment3D
{
    [DefaultExecutionOrder(1)]
    public class UI_Resistencia : MonoBehaviour
    {
        const string PLE = "_Ple";
        const string GASTANT = "_Gastant";

        [SerializeField] Canal_FloatFloatFloat resistencia;

        [SerializeField] Material material;
        [SerializeField] RectTransform rect;
        [SerializeField] Image image;
        [SerializeField] Vector2 offset;

        Vector3[] positions;
        int index;

        Vector3 finalPosition;

        float anterior = 0;

        private void OnEnable()
        {
            material = image.material;
            resistencia.Registrar(Pintar);
        }

        private void OnDisable()
        {
            resistencia.Desregistrar(Pintar);
        }

        private void Start()
        {
            positions = new Vector3[] {
                Cap.Posicio.ToCanvas(),
                Cap.Posicio.ToCanvas(),
                Cap.Posicio.ToCanvas(),
                Cap.Posicio.ToCanvas(),
                Cap.Posicio.ToCanvas()
            };
        }

        void LateUpdate()
        {
            index++;
            if (index >= 5) index = 0;

            positions[index] = Cap.Posicio.ToCanvas();

            finalPosition = Vector3.zero;
            for (int i = 0; i < positions.Length; i++)
            {
                finalPosition += positions[i];
            }
            finalPosition /= 5;
            

            rect.anchoredPosition3D = finalPosition + (Vector3.right * offset.x) + (Vector3.up * offset.y);

        }

        void Pintar(float actual, float maxim, float gastant)
        {
            material.SetFloat(GASTANT, gastant);
            material.SetFloat(PLE, actual / maxim);
            anterior = actual / maxim;
            //image.fillAmount = actual / maxim;
        }
    }
}

