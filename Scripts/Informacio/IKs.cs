using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using XS_Utils;

namespace Moviment3D
{
    public class IK
    {
        public IK(Transform ik, float offsetParet, bool ma, bool dreta)
        {
            this.ik = ik;
            this.offsetParet = offsetParet;
            this.ma = ma;
            this.dreta = dreta;
        }
        public RaycastHit hit;
        public Transform ik;
        public Vector3 point;
        public Vector3 normal;

        public float offsetParet;
        public bool ma;
        bool dreta;
        
        public void Actualitzar(float temps, bool forçat)
        {
            if (!Vector3.Distance(point, hit.point).IsNear(0, 0.1f))
            {
                point = Vector3.Lerp(point, hit.point, !forçat ? temps : 1);
                normal = Vector3.Lerp(normal, hit.normal, !forçat ? temps : 1);
                ik.position = point + hit.normal * offsetParet;
                ik.right = normal;
                if (ma)
                {
                    ik.localRotation = Quaternion.Euler(ik.localEulerAngles + (Vector3.back * 90) + (Vector3.right * (dreta ? 35 : -35)));
                }
            }
        }
    }
    public static class IKs
    {
        public static void Iniciar(Transform _transform, LayerMask _capaEntorn, Rig _rig, Transform _ikMaDreta, Transform _ikMaEsquerra, Transform _ikPeuDret, Transform _ikPeuEsquerra)
        {
            transform = _transform;
            capaEntorn = _capaEntorn;
            rig = _rig;
            //ikMaDreta = _ikMaDreta;
            maD = new IK(_ikMaDreta, 0.05f, true, true);
            maE = new IK(_ikMaEsquerra, 0.05f, true, false);
            peuD = new IK(_ikPeuDret, 0.17f, false, true);
            peuE = new IK(_ikPeuEsquerra, 0.17f, false, false);
        }
        static Transform transform;
        static LayerMask capaEntorn;
        static Rig rig;
        static IK maD;
        static IK maE;
        static IK peuD;
        static IK peuE;


        static float vMulti = 1f;
        static RaycastHit tmpDreta;
        static RaycastHit tmpEsquerra;
        static bool forçat;
        static bool entrar;
        public static void Debug()
        {
            if (maD.point != Vector3.zero) Debugar.DrawRay(maD.point, maD.normal);
            if (maE.point != Vector3.zero) Debugar.DrawRay(maE.point, maE.normal);
            if (peuD.point != Vector3.zero) Debugar.DrawRay(peuD.point, peuD.normal);
            if (peuE.point != Vector3.zero) Debugar.DrawRay(peuE.point, peuE.normal);
            /*RaigMaDreta(Inputs.Moviment * 0.5f);
            RaigMaEsquerra(Inputs.Moviment * 0.5f);
            RaigPeuEsquerra(Inputs.Moviment * 0.5f);
            RaigPeuDret(Inputs.Moviment * 0.5f);*/

            RaigMaDreta(Vector2.zero);
            //RaigMaEsquerra(Vector2.zero);
            //RaigPeuDret(Vector2.zero);
            //RaigPeuEsquerra(Vector2.zero);

        }
        public static void Actualitzar(float temps)
        {
            if (entrar)
                rig.weight = temps;

            maD.Actualitzar(temps, forçat);
            maE.Actualitzar(temps, forçat);
            peuD.Actualitzar(temps, forçat);
            peuE.Actualitzar(temps, forçat);
        }

        static RaycastHit Raig(Vector2 moviment, Vector3 vertical, Vector3 horitzontal, Vector3 offsetVertical)
        {
            if(XS_Physics.RaySphereDebug(RaigOrigen(moviment, vertical, horitzontal, offsetVertical), RaigDireccio(vertical, horitzontal), 2f, capaEntorn, 0.1f).Hitted())
                return XS_Physics.RaySphereDebug(RaigOrigen(moviment, vertical, horitzontal, offsetVertical), RaigDireccio(vertical, horitzontal), 2.5f, capaEntorn, 0.1f);
            else return XS_Physics.RaySphereDebug(RaigOrigen(moviment, vertical, horitzontal, offsetVertical), RaigDireccio(vertical * 1.5f, horitzontal * 1.5f), 3f, capaEntorn, 0.4f);
        }
        static Vector3 oInici(Transform transform, Vector3 direccio, Vector2 origen, Vector2 moviment) => Origen(origen) + Moviment(moviment * 0.5f) - transform.forward * 0.50f;
        static Vector3 oFinal(Transform transform, Vector3 direccio, Vector2 origen, Vector2 moviment) => Origen(origen) + Moviment(moviment * 0.5f) + transform.forward * 0.50f;
        static Vector3 dInici(Transform transform, Vector3 direccio, Vector2 origen, Vector2 moviment) => ( direccio * 0.50f) + transform.forward * 0.25f;
        static Vector3 dFinal(Transform transform, Vector3 direccio, Vector2 origen, Vector2 moviment) => (-direccio * 1.00f) + transform.forward * 0.25f;
        /*
        static RaycastHit RaigCorva(Transform transform, Vector3 direccio, Vector3 direccioRI, Vector2 origen, Vector2 moviment)
        {
            int max = 10;
            int actual = 0;<
            bool impactat = false;
            RaycastHit hit = new RaycastHit();
            while(actual < max && !impactat)
            {
                hit = XS_Physics.RayDebug(
                    Vector3.Lerp(oInici(transform, (direccio), origen, moviment), oFinal(transform, (direccio), origen, moviment), actual / (float)max),
                    Vector3.Lerp(dInici(transform, (direccio), origen, moviment), dFinal(transform, (direccio), origen, moviment), actual / (float)max), 
                    0.5f + (actual / (float)max) * 1.5f, 
                    capaEntorn);
                impactat = hit.Hitted();
                if (!impactat)
                    actual++;
            }
            if (impactat)
            {
                return hit;
            }
            else
            {
                max = 10;
                actual = 0;
                impactat = false;
                hit = new RaycastHit();
                while (actual < max && !impactat)
                {
                    //(transform.right + (transform.up * 0.50f)).normalized
                    hit = XS_Physics.RayDebug(
                        Vector3.Lerp(oInici(transform, transform.up, origen, moviment), oFinal(transform, transform.up, origen, moviment), actual / (float)max),
                        Vector3.Lerp(dInici(transform, transform.up, origen, moviment), dFinal(transform, transform.up, origen, moviment), actual / (float)max),
                        0.5f + (actual / (float)max) * 1.5f,
                        capaEntorn);
                    impactat = hit.Hitted();
                    if (!impactat)
                        actual++;
                }
                if (impactat)
                {
                    return hit;
                }
                else
                {
                    max = 10;
                    actual = 0;
                    impactat = false;
                    hit = new RaycastHit();
                    while (actual < max && !impactat)
                    {
                        hit = XS_Physics.RayDebug(
                            Vector3.Lerp(oInici(transform, direccioRI, origen, moviment), oFinal(transform, direccioRI, origen, moviment), actual / (float)max),
                            Vector3.Lerp(dInici(transform, direccioRI, origen, moviment), dFinal(transform, direccioRI, origen, moviment), actual / (float)max),
                            0.5f + (actual / (float)max) * 1.5f,
                            capaEntorn);
                        impactat = hit.Hitted();
                        if (!impactat)
                            actual++;
                    }
                    if (impactat)
                    {
                        return hit;
                    }
                    else
                    {
                        //Debugar.Log("hola?");
                        return new RaycastHit();
                    }
                }
               
            }


            return new RaycastHit();
        }
        */
        static RaycastHit RaigCorva(Transform transform, Vector3 direccio, Vector3 direccioRI, Vector2 origen, Vector2 moviment)
        {
            int max = 10;
            int actual = 0;
            bool impactat = false;
            RaycastHit hit = new RaycastHit();
            while (actual < max && !impactat)
            {
                hit = XS_Physics.RayDebug(
                    Vector3.Lerp(oInici(transform, (direccio), origen, moviment), oFinal(transform, (direccio), origen, moviment), actual / (float)max),
                    Vector3.Lerp(dInici(transform, (direccio), origen, moviment), dFinal(transform, (direccio), origen, moviment), actual / (float)max),
                    0.5f + (actual / (float)max) * 1.5f,
                    capaEntorn);
                impactat = hit.Hitted();
                if (!impactat)
                    actual++;
            }
            if (impactat)
            {
                return hit;
            }
            /*else
            {
                max = 10;
                actual = 0;
                impactat = false;
                hit = new RaycastHit();
                while (actual < max && !impactat)
                {
                    hit = XS_Physics.RayDebug(
                        Vector3.Lerp(oInici(transform, transform.up, origen, moviment), oFinal(transform, transform.up, origen, moviment), actual / (float)max),
                        Vector3.Lerp(dInici(transform, transform.up, origen, moviment), dFinal(transform, transform.up, origen, moviment), actual / (float)max),
                        0.5f + (actual / (float)max) * 1.5f,
                        capaEntorn);
                    impactat = hit.Hitted();
                    if (!impactat)
                        actual++;
                }
                if (impactat)
                {
                    return hit;
                }
                else
                {
                    max = 10;
                    actual = 0;
                    impactat = false;
                    hit = new RaycastHit();
                    while (actual < max && !impactat)
                    {
                        hit = XS_Physics.RayDebug(
                            Vector3.Lerp(oInici(transform, direccioRI, origen, moviment), oFinal(transform, direccioRI, origen, moviment), actual / (float)max),
                            Vector3.Lerp(dInici(transform, direccioRI, origen, moviment), dFinal(transform, direccioRI, origen, moviment), actual / (float)max),
                            0.5f + (actual / (float)max) * 1.5f,
                            capaEntorn);
                        impactat = hit.Hitted();
                        if (!impactat)
                            actual++;
                    }
                    if (impactat)
                    {
                        return hit;
                    }
                    else
                    {
                        return new RaycastHit();
                    }
                }

            }*/


            return new RaycastHit();
        }

        static Vector3 Origen(Vector2 origen) => transform.position + (transform.right * origen.x) + (transform.up * origen.y);
        static Vector3 Moviment(Vector2 moviment) => (transform.right * (moviment.x * 1.5f)) + (transform.up * (moviment.y * 1.7f));
        static Vector3 RaigOrigen(Vector2 moviment, Vector3 vertical, Vector3 horitzontal, Vector3 offsetVertical) => transform.position + (transform.up * (moviment.y * 1.9f) + transform.right * (moviment.x * 1.9f)) - transform.forward * 0.5f + vertical + offsetVertical + horitzontal;
        static Vector3 RaigDireccio(Vector3 vertical, Vector3 horitzontal) => (transform.forward * 2 - vertical - horitzontal).normalized * 2;

        public static void Capturar(Vector2 offset)
        {
            if(offset != Vector2.zero)
            {
                tmpDreta = RaigMaDreta(offset);
                tmpEsquerra = RaigMaEsquerra(offset);
                if (Vector3.Distance(maD.point, tmpDreta.point) > Vector3.Distance(maE.point, tmpEsquerra.point))
                {
                    maD.hit = tmpDreta;
                    peuE.hit = RaigPeuEsquerra(offset);
                }
                else
                {
                    maE.hit = tmpEsquerra;
                    peuD.hit = RaigPeuDret(offset);
                }
                forçat = false;
                entrar = false;
            }
            else
            {
                maD.hit = RaigMaDreta(Vector2.zero);
                maE.hit = RaigMaEsquerra(Vector2.zero);
                peuE.hit = RaigPeuEsquerra(Vector2.zero);
                peuD.hit = RaigPeuDret(Vector2.zero);
                forçat = true;
                entrar = true;
            }
        }



        static RaycastHit RaigMaDreta(Vector2 offset) => RaigCorva(transform, (transform.right + (transform.up * 0.50f)).normalized, (-transform.right + (transform.up * 0.50f)).normalized, new Vector2(0.2f, 1.25f), offset);
        static RaycastHit RaigMaEsquerra(Vector2 offset) => RaigCorva(transform, (-transform.right + (transform.up * 0.50f)).normalized, (transform.right + (transform.up * 0.50f)).normalized, new Vector2(-0.2f, 1.25f), offset);
        static RaycastHit RaigPeuDret(Vector2 offset) => RaigCorva(transform, (transform.right - transform.up).normalized, (-transform.right - transform.up).normalized, new Vector2(0.1f, 0.55f), offset);
        static RaycastHit RaigPeuEsquerra(Vector2 offset) => RaigCorva(transform, (-transform.right - transform.up).normalized, (transform.right - transform.up).normalized, new Vector2(-0.1f, 0.55f), offset);
        /*static RaycastHit RaigMaDreta(Vector2 offset) => Raig(offset, transform.up * 1.25f, transform.right * 0.5f, transform.up * 0.5f);
        static RaycastHit RaigMaEsquerra(Vector2 offset) => Raig(offset, transform.up * 1.25f, -transform.right * 0.5f, transform.up * 0.5f);
        static RaycastHit RaigPeuDret(Vector2 offset) => Raig(offset, -transform.up, transform.right * 0.4f, transform.up * 0.85f);
        static RaycastHit RaigPeuEsquerra(Vector2 offset) => Raig(offset, -transform.up, -transform.right * 0.4f, transform.up * 0.85f);
        */
        public static void Apagar()
        {
            Debugar.Log("Apagar");
            rig.weight = 0;
        }
    }
}

