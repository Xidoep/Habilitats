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
        [SerializeField] InputActionReference visio;


        private void OnEnable()
        {
            MyCamera.CameraMain = camaraPrincipal;
        }

        public void Update()
    {
            if (!freeLook)
                return;

        freeLook.XS_AddOrientar(visio.GetVector2());

        if(!altura.IsNear(Entorn.Camera.DistanciaDesdeTerra(transform), 0.1f))
        {
            altura += (Entorn.Camera.DistanciaDesdeTerra(transform) - altura) * Time.deltaTime;
        }

        freeLook.XS_OrbitaSuperior(8 * distancia, 3 * distancia);
        freeLook.XS_OrbitaMitja(2 * distancia, 7 * distancia);
        freeLook.XS_OrbitaInferior((-altura) * distancia, 1 * distancia);
    }
}

}
