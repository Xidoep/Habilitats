using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using XS_Utils;
using Moviment3D;

namespace Moviment3D
{
    public class CamaraLliure : MonoBehaviour
    {
        public Camera camaraPrincipal;

        public CinemachineFreeLook freeLook;

        public float altura;

        public float distancia;
        public float extraDistancia;
        [SerializeField] InputActionReference visio;
        public float distanciaVelocitat;
        float iniciTornada;

        private void OnEnable()
        {
            MyCamera.Main = camaraPrincipal;
        }

        public void Update()
        {
            if (!freeLook)
                return;

            if (Dinamic.Velocitat.magnitude > 0)
            {
                distanciaVelocitat += Time.deltaTime * Dinamic.Velocitat.magnitude * 2;
                distanciaVelocitat = Mathf.Clamp01(distanciaVelocitat);
                iniciTornada = 0;
            }
            else 
            {
                iniciTornada += Time.deltaTime;
                distanciaVelocitat -= Time.deltaTime * distanciaVelocitat * (Mathf.Clamp01(iniciTornada));
                distanciaVelocitat = Mathf.Clamp01(distanciaVelocitat);
            }

            //freeLook.XS_AddOrientar(visio.GetVector2());

            if (!altura.IsNear(Entorn.Camera.DistanciaDesdeTerra(transform), 0.1f))
            {
                altura += (Entorn.Camera.DistanciaDesdeTerra(transform) - altura) * Time.deltaTime;
            }

            SetArcsCamara(distancia + extraDistancia);
        }



        void SetArcsCamara(float distancia)
        {
            freeLook.XS_OrbitaSuperior(8 * distancia, 3 * distancia);
            freeLook.XS_OrbitaMitja(2 * distancia, 7 * distancia);
            freeLook.XS_OrbitaInferior((-altura) * distancia, 1 * distancia);
        }

        public void SetExtraDistancia(float extraDistancia) => this.extraDistancia = extraDistancia;

        private void OnValidate()
        {
            if (freeLook.Follow == null | freeLook.LookAt == null)
                return;

            Transform personatge = FindObjectOfType<Informacio>().transform;
            freeLook.Follow = personatge;
            freeLook.LookAt = personatge;
        }
    }

    
}
