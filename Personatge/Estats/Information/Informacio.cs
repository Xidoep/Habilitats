<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using XS_Utils;

namespace Moviment3D
{
    [SelectionBase]
    [DefaultExecutionOrder(-1)]
    public class Informacio : MonoBehaviour
    {
        [SerializeField] Info info;
        [SerializeField] LayerMask capaEntorn;

        [SerializeField] InputActionReference moviment;
        [SerializeField] InputActionReference saltar;
        [SerializeField] InputActionReference agafar;
        [SerializeField] InputActionReference deixar;
        [SerializeField] Rig rig;

        [SerializeField] Transform ikMaDreta;
        [SerializeField] Transform ikMaEsquerra;
        [SerializeField] Transform ikPeuDreta;
        [SerializeField] Transform ikMPeuEsquerra;

        [Header("Canals")]
        [SerializeField] Canal_FloatFloat canalResistencia;
        [SerializeField] Canal_Void canalMort;
        [SerializeField] Canal_Bool canalTrencat;

        [SerializeField] bool testing;
        public float debug;

        public bool damaged;

        private void OnEnable()
        {
            info = info.Iniciar(transform);
            foreach (var item in GetComponents<EstatPlayer>())
            {
                item.Informacio = info;
            }
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<PlayerInput>().enabled = true;
            Animacio.Iniciar(transform);
            Inputs.Iniciar(moviment, saltar, agafar, deixar);
            Entorn.Iniciar(capaEntorn);
            Resistencia.Iniciar(canalResistencia, testing);
            Vida.Iniciar(canalMort, canalTrencat);
            gameObject.AddComponent<Environment.Effector_Collision>().LayerMask = capaEntorn;
            //IKs.Iniciar(helper, capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
            //IKs.Iniciar(transform, capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debugar.Log($"Colisio a {collision.relativeVelocity.magnitude}");
            if(collision.relativeVelocity.magnitude > 15)
            {
                Vida.Damage();
            }
        }

        private void Update()
        {
            debug = Dinamic.MultiplicadorMovimentAeri(MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment));
            //IKs.Debug();

            if (Key.V.OnPress()) Vida.Restore();
            //if (Key.V.OnPress()) resist.Testing = !resist.Testing;
        }


        private void FixedUpdate()
        {
            Dinamic.ActualitzarSmooth(transform);
        }
        //internal Animator Animator => animator;


        private void OnDisable()
        {
            
        }
    }
}




=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using XS_Utils;

namespace Moviment3D
{
    [SelectionBase]
    [DefaultExecutionOrder(-1)]
    public class Informacio : MonoBehaviour
    {
        [SerializeField] Info info;
        [SerializeField] LayerMask capaEntorn;

        [SerializeField] InputActionReference moviment;
        [SerializeField] InputActionReference saltar;
        [SerializeField] InputActionReference agafar;
        [SerializeField] InputActionReference deixar;
        [SerializeField] Rig rig;

        [SerializeField] Transform ikMaDreta;
        [SerializeField] Transform ikMaEsquerra;
        [SerializeField] Transform ikPeuDreta;
        [SerializeField] Transform ikMPeuEsquerra;

        [Header("Canals")]
        [SerializeField] Canal_FloatFloat canalResistencia;
        [SerializeField] Canal_Void canalMort;
        [SerializeField] Canal_Bool canalTrencat;

        [SerializeField] bool testing;
        public float debug;

        public bool damaged;

        private void OnEnable()
        {
            info = info.Iniciar(transform);
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<PlayerInput>().enabled = true;
            Animacio.Iniciar(transform);
            Inputs.Iniciar(moviment, saltar, agafar, deixar);
            Entorn.Iniciar(capaEntorn);
            Resistencia.Iniciar(canalResistencia, testing);
            Vida.Iniciar(canalMort, canalTrencat);
            gameObject.AddComponent<Environment.Effector_Collision>().LayerMask = capaEntorn;
            //IKs.Iniciar(helper, capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
            //IKs.Iniciar(transform, capaEntorn, rig, ikMaDreta, ikMaEsquerra, ikPeuDreta, ikMPeuEsquerra);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debugar.Log($"Colisio a {collision.relativeVelocity.magnitude}");
            if(collision.relativeVelocity.magnitude > 15)
            {
                Vida.Damage();
            }
        }

        private void Update()
        {
            debug = Dinamic.MultiplicadorMovimentAeri(MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment));
            //IKs.Debug();

            if (Key.V.OnPress()) Vida.Restore();
            //if (Key.V.OnPress()) resist.Testing = !resist.Testing;
        }


        private void FixedUpdate()
        {
            Dinamic.ActualitzarSmooth(transform);
        }
        //internal Animator Animator => animator;


        private void OnDisable()
        {
            
        }
    }
}




>>>>>>> parent of 5a24bc2 (Varis arreglos mes)
