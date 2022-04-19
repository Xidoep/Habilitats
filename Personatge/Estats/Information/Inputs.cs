using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using XS_Utils;
using System;

namespace Moviment3D
{
    public static class Inputs
    {
        public static void Iniciar(
            InputActionReference _moviment,
            InputActionReference _saltar,
            InputActionReference _agafar,
            InputActionReference _deixar
            )
        {
            moviment = _moviment;
            saltar = _saltar;
            agafar = _agafar;
            deixar = _deixar;

            agafar.OnPerformedAdd(AgafarPerformed);
        }

        



        //PUBLIQUES
        static InputActionReference moviment;
        public static InputActionReference saltar;
        static InputActionReference agafar;
        static InputActionReference deixar;

        //PRIVADES
        static EscalarVectors vectors;
        /*Vector3 posicioFameAnterior;
        [SerializeField] Vector3 velocitat;*/

        //internal Rigidbody rb;
        static bool saltEscalantPreparat;
        static bool saltEscalantReenganxarse;

        static bool agafarPerformed = false;
        //PROPIETATS

        public static bool SaltEscalantPreparat
        {
            get => saltEscalantPreparat;
            set => saltEscalantPreparat = value;
        }
        public static bool SaltEscalantReenganxarse
        {
            get => saltEscalantReenganxarse;
            set => saltEscalantReenganxarse = value;
        }

        //public static bool Escalar => agafar.GetBool();
        public static bool Escalar => agafarPerformed;
        public static void DisableEscalar() => agafarPerformed = false;
        public static Vector2 Moviment => moviment.GetVector2();

        public static bool MovimentZero => moviment.GetVector2() == Vector2.zero;
        public static bool Saltar => saltar.GetBool();
        public static bool Deixar => deixar.GetBool();

        //internal Vector3 Velocitat => velocitat;
        //internal Vector3 SetVelocitat { set => velocitat = value; }
        public static Transform SetHelperVectors
        {
            set
            {
                vectors.up = value.up;
                vectors.right = value.right;
                vectors.forward = value.forward;
            }
        }
        public static Vector3 GetHelperUp => vectors.up;
        public static Vector3 GetHelperRight => vectors.right;
        public static Vector3 GetHelperForward => vectors.forward;


        public static bool AreActived => moviment != null;

        private static void AgafarPerformed(InputAction.CallbackContext obj)
        {
            agafarPerformed = agafar.GetBool();
        }
    }

}

internal struct EscalarVectors
{
    public Vector3 up;
    public Vector3 right;
    public Vector3 forward;
}

