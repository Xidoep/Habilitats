using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using XS_Utils;

namespace Moviment3D
{
    [SelectionBase]
    [DefaultExecutionOrder(-10)]
    public class Informacio : MonoBehaviour
    {
        [SerializeField] LayerMask capaEntorn;

        [SerializeField] Resist resistencia;
        [SerializeField] AnimPlayer animacio;
        [SerializeField] Inpt inputs;
        [SerializeField] Vid vida;
        [SerializeField] Preparaci preparacio;
        [SerializeField] Dinami dinamic;
        [SerializeField] CoyoteTim coyoteTime;

        public Resist Resistencia => resistencia;
        public AnimPlayer Animacio => animacio;
        public Vid Vida => vida;
        public Inpt Inputs => inputs;
        public Preparaci Preparacio => preparacio;
        public Dinami Dinamic => dinamic;
        public CoyoteTim CoyoteTime => coyoteTime;

        //public Info Info => info;
        //[SerializeField] LayerMask capaEntorn;

        /*[SerializeField] InputActionReference moviment;
        [SerializeField] InputActionReference saltar;
        [SerializeField] InputActionReference agafar;
        [SerializeField] InputActionReference deixar;*/

        /*[SerializeField] Rig rig;
        [SerializeField] Transform ikMaDreta;
        [SerializeField] Transform ikMaEsquerra;
        [SerializeField] Transform ikPeuDreta;
        [SerializeField] Transform ikMPeuEsquerra;*/

        /*[Header("Canals")]
        [SerializeField] Canal_Void canalMort;
        [SerializeField] Canal_Bool canalTrencat;
        */
        //[SerializeField] bool testing;
        public float debug;


        private void OnEnable()
        {
            foreach (var item in GetComponents<Estat>())
            {
                item.Informacio = this;
            }

            Entorn.Iniciar(capaEntorn);
            resistencia = resistencia.Iniciar();
            animacio = (AnimPlayer)animacio.Iniciar(transform);
            inputs = inputs.Iniciar(transform);
            vida = vida.Iniciar();
            preparacio = preparacio.Iniciar();
            dinamic = dinamic.Iniciar(transform);
            coyoteTime = coyoteTime.Iniciar();


            //GetComponent<PlayerInput>().enabled = false;
            //GetComponent<PlayerInput>().enabled = true;
            //Animacio.Iniciar(transform);
            //Inputs.Iniciar(moviment, saltar, agafar, deixar);
            //Entorn.Iniciar(capaEntorn);
            //Resistencia.Iniciar(canalResistencia, testing);
            //Vida.Iniciar(canalMort, canalTrencat);
            //gameObject.AddComponent<Environment.Effector_Collision>().LayerMask = capaEntorn;
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
            debug = Dinamic.MultiplicadorMovimentAeri(MyCamera.Transform.ACamaraRelatiu(inputs.Moviment));
            //IKs.Debug();

            if (Key.V.OnPress()) Vida.Restore();
            //if (Key.V.OnPress()) resist.Testing = !resist.Testing;
        }


        private void FixedUpdate()
        {
            //Dinamic.ActualitzarSmooth(transform);
        }
        //internal Animator Animator => animator;


    }
}




