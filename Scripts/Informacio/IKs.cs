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
            oldsCreated = false;
            if (oldPoint == null) oldPoint = new GameObject("old" + (ma ? "Ma" : "Peu") + (dreta ? "D" : "E")).transform;
            if (newPoint == null) newPoint = new GameObject("new" + (ma ? "Ma" : "Peu") + (dreta ? "D" : "E")).transform;
        }
        public Transform ik;
        RaycastHit hit;
        public Vector3 point;
        public Vector3 normal;

        public Transform oldPoint;
        public Transform newPoint;


        public float offsetParet;
        public bool ma;
        bool dreta;
        bool oldsCreated;
        public bool moure;
        public void Capturar(RaycastHit hit)
        {
            moure = true;
            if (oldsCreated)
            {
            oldPoint.SetParent(newPoint.parent);
            oldPoint.position = newPoint.position;
            oldPoint.right = newPoint.right;

            }

            this.hit = hit;
            newPoint.SetParent(hit.collider.transform);
            newPoint.position = hit.point + hit.normal * offsetParet;
            newPoint.right = hit.normal;

            oldsCreated = true;
        }
        public void Actualitzar(float temps, bool forçat)
        {
            if (moure)
            {
                if (!Vector3.Distance(ik.position, newPoint.position).IsNear(0, 0.1f))
                    Posicionar(oldPoint, newPoint, temps, forçat);
            }
            else Posicionar(newPoint, newPoint, temps, forçat);
        }

        public void Apagar()
        {
            //oldPoint = null;
            //newPoint = null;
        }

        void Posicionar(Transform start, Transform end, float temps, bool forçat)
        {
            ik.position = Vector3.Lerp(start.position, end.position, !forçat ? temps : 1);
            ik.right = Vector3.Lerp(start.right, end.right, !forçat ? temps : 1);
            if (ma)
            {
                ik.localRotation = Quaternion.Euler(ik.localEulerAngles + (Vector3.back * 90) + (Vector3.right * (dreta ? 35 : -35)));
            }
        }
    }
    public static class IKs
    {
        public static void Iniciar(
            Transform _transform, 
            LayerMask _capaEntorn, 
            Rig _rig, 
            Transform _ikMaDreta, 
            Transform _ikMaEsquerra, 
            Transform _ikPeuDret, 
            Transform _ikPeuEsquerra)
        {
            transform = _transform;
            capaEntorn = _capaEntorn;
            rig = _rig;
            //ikMaDreta = _ikMaDreta;
            if (maD == null) maD = new IK(_ikMaDreta, 0.05f, true, true);
            if (maE == null) maE = new IK(_ikMaEsquerra, 0.05f, true, false);
            if (peuD == null) peuD = new IK(_ikPeuDret, 0.17f, false, true);
            if (peuE == null) peuE = new IK(_ikPeuEsquerra, 0.17f, false, false);
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

        static Vector3 Forward => -transform.forward;
        static Vector3 Up => transform.up;
        static Vector3 Right => -transform.right;
        public static void Debug()
        {
            if (maD.point != Vector3.zero) Debugar.DrawRay(maD.point, maD.normal);
            if (maE.point != Vector3.zero) Debugar.DrawRay(maE.point, maE.normal);
            if (peuD.point != Vector3.zero) Debugar.DrawRay(peuD.point, peuD.normal);
            if (peuE.point != Vector3.zero) Debugar.DrawRay(peuE.point, peuE.normal);
 
            RaigMaDreta(Vector2.zero);
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
        static Vector3 oInici(Transform transform, Vector3 direccio, Vector2 origen, Vector2 moviment) => Origen(origen) + Moviment(moviment * 0.5f) - Forward * 0.50f;
        static Vector3 oFinal(Transform transform, Vector3 direccio, Vector2 origen, Vector2 moviment) => Origen(origen) + Moviment(moviment * 0.5f) + Forward * 0.50f;
        static Vector3 dInici(Transform transform, Vector3 direccio, Vector2 origen, Vector2 moviment) => ( direccio * 0.50f) + Forward * 0.25f;
        static Vector3 dFinal(Transform transform, Vector3 direccio, Vector2 origen, Vector2 moviment) => (-direccio * 1.00f) + Forward * 0.25f;

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
            else
            {
                max = 10;
                actual = 0;
                impactat = false;
                hit = new RaycastHit();
                while (actual < max && !impactat)
                {
                    hit = XS_Physics.RayDebug(
                        Vector3.Lerp(oInici(transform, Up, origen, moviment), oFinal(transform, Up, origen, moviment), actual / (float)max),
                        Vector3.Lerp(dInici(transform, Up, origen, moviment), dFinal(transform, Up, origen, moviment), actual / (float)max),
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

            }


            return new RaycastHit();
        }

        static Vector3 Origen(Vector2 origen) => transform.position + (Right * origen.x) + (Up * origen.y);
        static Vector3 Moviment(Vector2 moviment) => (Right * (moviment.x * 1.5f)) + (Up * (moviment.y * 1.7f));
        static Vector3 RaigOrigen(Vector2 moviment, Vector3 vertical, Vector3 horitzontal, Vector3 offsetVertical) => 
            transform.position + (Up * (moviment.y * 1.9f) + Right * (moviment.x * 1.9f)) - Forward * 0.5f + vertical + offsetVertical + horitzontal;
        static Vector3 RaigDireccio(Vector3 vertical, Vector3 horitzontal) => (Forward * 2 - vertical - horitzontal).normalized * 2;

        public static void Capturar(Vector2 offset)
        {
            if(offset != Vector2.zero)
            {
                tmpDreta = RaigMaDreta(offset);
                tmpEsquerra = RaigMaEsquerra(offset);
                forçat = false;
                entrar = false;
                //if (Vector3.Distance(maD.point, tmpDreta.point) > Vector3.Distance(maE.point, tmpEsquerra.point))
                if (Vector3.Distance(maD.newPoint.position, tmpDreta.point) > Vector3.Distance(maE.newPoint.position, tmpEsquerra.point))
                    {
                    maD.Capturar(tmpDreta);
                    peuE.Capturar(RaigPeuEsquerra(offset));
                    maE.moure = false;
                    peuD.moure = false;
                }
                else
                {
                    maE.Capturar(tmpEsquerra);
                    peuD.Capturar(RaigPeuDret(offset));
                    maD.moure = false;
                    peuE.moure = false;
                }
            }
            else
            {
                forçat = true;
                entrar = true;
                maD.Capturar(RaigMaDreta(Vector2.zero));
                maE.Capturar(RaigMaEsquerra(Vector2.zero));
                peuE.Capturar(RaigPeuEsquerra(Vector2.zero));
                peuD.Capturar(RaigPeuDret(Vector2.zero));
            }
        }



        static RaycastHit RaigMaDreta(Vector2 offset) => RaigCorva(transform, (Right + (Up * 0.50f)).normalized, (-Right + (Up * 0.50f)).normalized, new Vector2(0.2f, 1.25f), offset);
        static RaycastHit RaigMaEsquerra(Vector2 offset) => RaigCorva(transform, (-Right + (Up * 0.50f)).normalized, (Right + (Up * 0.50f)).normalized, new Vector2(-0.2f, 1.25f), offset);
        static RaycastHit RaigPeuDret(Vector2 offset) => RaigCorva(transform, (Right - Up).normalized, (-Right - Up).normalized, new Vector2(0.1f, 0.55f), offset);
        static RaycastHit RaigPeuEsquerra(Vector2 offset) => RaigCorva(transform, (-Right - Up).normalized, (Right - Up).normalized, new Vector2(-0.1f, 0.55f), offset);

        public static void Apagar()
        {
            Debugar.Log("Apagar");
            rig.weight = 0;
        }
    }
}

