using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    //SHAREABLE
    public static class Funcions
    {
        public static void Orientar(this Transform transform, int velocitat)
        {
            if (Inputs.Moviment != Vector2.zero)
                transform.MirarObjectiu(MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment).ToQuaternion(Vector3.up), velocitat);
            else transform.MirarObjectiu(transform.forward.ToQuaternion(Vector3.up), velocitat);
        }
        public static void Gravetat(this Rigidbody rigidbody)
        {
            if (rigidbody.velocity.y > -20)
                rigidbody.AddForce(Vector3.down * 300);
        }
    }

    public static class ExtensionsInformacio
    {
        public static bool Pla(this Vector3 vector3) => Vector3.Dot(vector3, Vector3.up) > 0.65f;
        public static bool PropDe1(this Vector3 vector3) => Vector3.Dot(vector3, Vector3.up) > 0.8f;
        //public static bool Relliscar(this Vector3 vector3) => Vector3.Dot(vector3, Vector3.up) < 0.75f;
        public static bool Relliscar(this Vector3 vector3) => Vector3.Dot(vector3, Vector3.up) == Mathf.Clamp(Vector3.Dot(vector3, Vector3.up), 0.35f, 0.75f);
        public static bool Positiu(this Vector3 normal) => Vector3.Dot(normal, Vector3.up) > 0;
        public static bool CasiMenys1(this Vector3 vector3) => Vector3.Dot(vector3, Vector3.down) > 0.9f;
    }
}

