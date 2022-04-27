using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    //SHAREABLE!
    public static class Entorn
    {
        public static void Iniciar(LayerMask _capaEntorn, LayerMask _capaRelliscant)
        {
            capaEntorn = _capaEntorn;
            capaRelliscant = _capaRelliscant;
        }

        

        public static LayerMask capaEntorn;
        public static LayerMask capaRelliscant;
        
        static RaycastHit nul;



        static float distMovEscalar => 0.4f;
        static float distBuscarTerra => 0.9f;

        static List<Vector3> normals = new List<Vector3>();
        static Vector3 normalCompound;
        public static void HitNormal(ref RaycastHit primerHit, Transform helper)
        {
            Debugar.Log("hola?");
            if (normals == null) normals = new List<Vector3>();
            normals.Clear();
            normalCompound = Vector3.zero;

            normals.Add(primerHit.normal);
            if (XS_Physics.RayDebug(helper.position + helper.up * 2 + helper.forward, -helper.forward, 2, capaEntorn, 3).Hitted())
                normals.Add(XS_Physics.RayDebug(helper.position + helper.up * 2 + helper.forward, -helper.forward, 2, capaEntorn, 3).normal);

            if (XS_Physics.RayDebug(helper.position + helper.up * 1 + helper.right * 0.45f + helper.forward, -helper.forward, 2, capaEntorn, 3).Hitted())
                normals.Add(XS_Physics.RayDebug(helper.position + helper.up * 1 + helper.right * 0.45f + helper.forward, -helper.forward, 2, capaEntorn, 3).normal);

            if (XS_Physics.RayDebug(helper.position + helper.up * 1 - helper.right * 0.45f + helper.forward, -helper.forward, 2, capaEntorn, 3).Hitted())
                normals.Add(XS_Physics.RayDebug(helper.position + helper.up * 1 - helper.right * 0.45f + helper.forward, -helper.forward, 2, capaEntorn, 3).normal);

            Debugar.Log($"{normals.Count} normals");
            for (int i = 0; i < normals.Count; i++)
            {
                //Debug.Log($"{normals[i]}");
                normalCompound += normals[i];
            }
            normalCompound /= normals.Count;
            primerHit.normal = normalCompound;
            //primerHit.normal = primerHit.normal;
        }

        /// <summary>
        /// Funcions destinades a quan s'està escalant explusivament
        /// </summary>
        public static class Escalant
        {
            //MOVIMENT
            public static RaycastHit Moviment(Transform helper, bool pla, Vector2 moviment, out float velocitat)
            {
                
                RaycastHit hit = nul;
                float _velocitat = 1;
                if(Buscar.Terra(helper,pla, moviment).Hitted())
                {
                    Debugar.Log("Terra");
                    hit = Buscar.Terra(helper, pla, moviment); //Terra sota els peus
                    _velocitat = 0.75f;
                }
                else
                {
                    if (Buscar.Bloquejat(helper, pla, moviment).Hitted())
                    {
                        Debugar.Log("Bloquejat");
                        hit = Buscar.Bloquejat(helper, pla, moviment); //Mur davant d'on vol anar
                        _velocitat = 0.75f;
                    }
                    else
                    {
                        if (Buscar.Recta(helper, pla, moviment).Hitted()) //Si hi ha paret per continuar
                        {
                            if (!pla)
                            {
                                if (moviment.normalized.y > 0) //Si vas cap amunt
                                {
                                    if (Buscar.Sostre(helper, pla, moviment).Hitted())
                                    {
                                        Debugar.Log("Sostre");
                                        hit = Buscar.Sostre(helper, pla, moviment); //Si mes amunt hi torves un sostre
                                        _velocitat = 0.4f;
                                    }
                                    else
                                    {
                                        if (Buscar.CantonadaPlanaAmunt(helper).Hitted())
                                        {
                                            Debugar.Log("Cantonada plana mes amunt");
                                            hit = Buscar.CantonadaPlanaAmunt(helper); //Si torves una cantonada plana
                                            _velocitat = 0.5f;
                                        }
                                        else
                                        {
                                            Debugar.Log("Recta, no pla i amunt");
                                            hit = Buscar.Recta(helper, pla, moviment);
                                            _velocitat = 0.5f;
                                        }
                                    }
                                }
                                else
                                {
                                    Debugar.Log("Recta no amunt");
                                    hit = Buscar.Recta(helper, pla, moviment);
                                    _velocitat = 1;
                                }
                            }
                            else
                            {
                                Debugar.Log("Recta i pla");
                                hit = Buscar.Recta(helper, pla, moviment);
                                _velocitat = 1;
                            }
                        }
                        else
                        {
                            if (Buscar.Cantonada(helper, pla, moviment).Hitted())
                            {
                                Debugar.Log("Cantonada");
                                hit = Buscar.Cantonada(helper, pla, moviment); //Busca cantonada
                                _velocitat = 0.5f;
                            }
                        }
                    }
                }
                
                velocitat = _velocitat;
                return hit;
            }
            static Vector3 Direccio(Transform helper, bool pla, Vector2 moviment) => !pla ? Direccio_Vertical(helper, moviment) : Direccio_Pla(moviment);
            static Vector3 Direccio_Vertical(Transform helper, Vector2 moviment) => (helper.up * moviment.normalized.y/* + helper.up * 1.75f*/) - (helper.right * moviment.normalized.x);
            static Vector3 Direccio_Pla(Vector2 moviment) => MyCamera.Transform.ACamaraRelatiu(moviment).normalized;

            /// <summary>
            /// Accions de buscar propies d'escalar
            /// </summary>
            public static class Buscar
            {
                public static RaycastHit CantonadaPlanaAmunt(Transform helper)
                {
                   

                    if (!XS_Physics.RayDebug(
                        Separacio(helper), 
                        helper.up, 
                        distMovEscalar * 2.2f, capaEntorn, 1).Hitted())
                    {
                        if (!XS_Physics.RayDebug(
                            Separacio(helper) + Amunt(helper), 
                            -helper.forward, 
                            1 + distMovEscalar, capaEntorn, 1).Hitted())
                        {
                            if (XS_Physics.RayDebug(
                                Separacio(helper) + Amunt(helper) - Endevant(helper), 
                                -helper.up * 2, 
                                distMovEscalar * 1.2f, capaEntorn, 1).Hitted())
                                return XS_Physics.RayDebug(
                                    Separacio(helper) + Amunt(helper) - Endevant(helper), 
                                    -helper.up * 2, 
                                    distMovEscalar * 1.2f, capaEntorn, 1);
                            else return nul;
                        }
                        else return nul;
                    }
                    else return nul;
                    /*if (!Fisiques.Raig(helper.position + helper.up * 2f, helper.forward * 1.5f, 1.5f, capaEntorn, 1).Impactat())
                    {
                        if (Fisiques.Raig(helper.position + helper.up * 2f + helper.forward * 1.5f, -helper.up * 1.5f, 1.5f, capaEntorn, 1).Impactat())
                        {
                            if (Fisiques.Raig(helper.position + helper.up * 2f + helper.forward * 1.5f, -helper.up * 1.5f, 1.5f, capaEntorn, 1).normal.Pla())
                                return Fisiques.Raig(helper.position + helper.up * 2f + helper.forward * 1.5f, -helper.up * 1.5f, 1.5f, capaEntorn, 1);
                        }
                        return nul;
                    }
                    else return nul;*/
                    //return nul;
                }
                public static RaycastHit CantonadaSuperior(Transform transform)
                {
                    if (XS_Physics.RayDebug(
                        transform.position + transform.up * 1.5f + transform.forward * 1.5f, 
                        -transform.up, 
                        1.5f, capaEntorn, 1).Hitted())
                    {
                        return XS_Physics.RayDebug(
                            transform.position + transform.up * 1.5f + transform.forward * 1.5f, 
                            -transform.up, 
                            1.5f, capaEntorn, 1);
                    }
                    else return nul;

                }
                static RaycastHit cantonadaSuperior;
                public static void CantonadaSuperior(Transform transform, Action<RaycastHit> action)
                {
                    cantonadaSuperior = XS_Physics.RayDebug(
                        transform.position + transform.up * 1.5f + transform.forward * 1.5f,
                        -transform.up,
                        1.5f, capaEntorn, 1);
                    
                    if (cantonadaSuperior.Hitted()) action.Invoke(cantonadaSuperior);
                }
                internal static RaycastHit Terra(Transform helper, bool pla, Vector2 moviment) 
                {
                    if (moviment.y >= 0) return nul;
                    else return XS_Physics.RayDebug(
                        Separacio(helper),
                        Direccio(helper, pla, moviment),
                        distMovEscalar * 1.75f, capaEntorn, 1);
                }
                internal static RaycastHit Bloquejat(Transform helper, bool pla, Vector2 moviment) => 
                    XS_Physics.RayDebug(
                        Separacio(helper), 
                        Direccio(helper, pla, moviment), 
                        distMovEscalar * 1.5f, capaEntorn, 1);
                internal static RaycastHit Recta(Transform helper, bool pla, Vector2 moviment) => 
                    XS_Physics.RayDebug(
                        Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar, 
                        -helper.forward, 
                        1, capaEntorn, 1);
                internal static RaycastHit Cantonada(Transform helper, bool pla, Vector2 moviment)
                {
                    if (XS_Physics.RayDebug(
                        Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar - helper.forward * 1, 
                        -Direccio(helper, pla, moviment) - helper.forward, 
                        1, capaEntorn, 1).Hitted())
                        return XS_Physics.RayDebug(
                            Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar - helper.forward * 1, 
                            -Direccio(helper, pla, moviment) - helper.forward, 
                            1, capaEntorn, 1);
                    else return XS_Physics.RayDebug(
                        Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar - helper.forward * 1, 
                        -Direccio(helper, pla, moviment), 
                        1, capaEntorn, 1);
                }
                internal static RaycastHit Sostre(Transform helper, bool pla, Vector2 moviment) => 
                    XS_Physics.RayDebug(
                        Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar, 
                        helper.up, 
                        1, capaEntorn, 1);

               
                static Vector3 Amunt(Transform helper) => helper.up * (distMovEscalar * 3.6f);
                static Vector3 Endevant(Transform helper) => helper.forward * (1 + distMovEscalar);
                static Vector3 Separacio(Transform helper) => helper.position + helper.forward * 0.5f;
                static Vector3 SeparacioMenor(Transform helper) => helper.position + helper.forward * 0.0f;
            }

            //static Vector3 HelperUp(Transform helper) => helper.up * 0.5f;
        }

        /// <summary>
        /// Accions de buscar generiques
        /// </summary>
        public static class Buscar
        {
            /// <summary>
            /// Accions de buscar per quan no s'està escalant
            /// </summary>
            public static class Dret
            {
                public static RaycastHit OnComencarAEscalar(Transform transform, out float velocitat) //no escalant
                {
                    if (Endevant(transform).Hitted()) {
                        velocitat = 2;
                        return Endevant(transform);
                    } 
                    else{
                        if (!DavantDelsPeus(transform).Hitted() && CantonadaForat(transform).Hitted()) {
                            velocitat = 2f;
                            return CantonadaForat(transform);
                        } 
                        else{
                            if (AlsPeus(transform).Hitted()) {
                                velocitat = 1;
                                return AlsPeus(transform);
                            }
                            else{
                                if (DiagonalDreta(transform).Hitted()) {
                                    velocitat = 2;
                                    return DiagonalDreta(transform);
                                } 
                                else{
                                    if (DiagonalEsquerra(transform).Hitted()) {
                                        velocitat = 2;
                                        return DiagonalEsquerra(transform);
                                    } 
                                    else{
                                        if (Dreta(transform).Hitted()) {
                                            velocitat = 1.5f;
                                            return Dreta(transform);
                                        }
                                        else{
                                            if (Esquerra(transform).Hitted()) {
                                                velocitat = 1.5f;
                                                return Esquerra(transform);
                                            }
                                            else{
                                                if (DiagonalAmunt(transform).Hitted()) {
                                                    velocitat = 1;
                                                    return DiagonalAmunt(transform);
                                                }
                                                else{
                                                    if (Amunt(transform).Hitted()) {
                                                        velocitat = 1;
                                                        return Amunt(transform);
                                                    }
                                                    else{
                                                        if (DavantDelsPeus(transform).Hitted()) {
                                                            velocitat = 2;
                                                            return DavantDelsPeus(transform);
                                                        }
                                                        else{
                                                            if (CantonadaSuperior(transform).Hitted())
                                                            {
                                                                velocitat = 0.5f;
                                                                return CantonadaSuperior(transform);
                                                            }
                                                            else{
                                                                velocitat = 1;
                                                                return nul;
                                                            } 
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                public static RaycastHit OnComencarAEscalar_Aire(Transform transform, out float velocitat) //no escalant
                {
                    if (!DavantDelCap(transform).Hitted() && CantonadaSuperior(transform).Hitted()) 
                    {
                        Debugar.Log("Donem una senyal!!!");
                        velocitat = 0.5f;
                        return CantonadaSuperior(transform);
                    } 
                    else
                    {
                        if (Endevant(transform).Hitted()) {
                            velocitat = 2;
                            return Endevant(transform);
                        } 
                        else{
                            if (DiagonalDreta(transform).Hitted()) {
                                velocitat = 2;
                                return DiagonalDreta(transform);
                            } 
                            else{
                                if (DiagonalEsquerra(transform).Hitted()) {
                                    velocitat = 2;
                                    return DiagonalEsquerra(transform);
                                } 
                                else{
                                    if (Dreta(transform).Hitted()) {
                                        velocitat = 2;
                                        return Dreta(transform);
                                    } 
                                    else{
                                        if (Esquerra(transform).Hitted()) {
                                            velocitat = 2;
                                            return Esquerra(transform);
                                        } 
                                        else{
                                            if (DiagonalAmunt(transform).Hitted()) {
                                                velocitat = 2;
                                                return DiagonalAmunt(transform);
                                            } 
                                            else{
                                                if (Amunt(transform).Hitted())
                                                {
                                                    velocitat = 2;
                                                    return Amunt(transform);
                                                }
                                                else {
                                                    velocitat = 2;
                                                    return nul;
                                                } 
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }



                public static RaycastHit Endevant(Transform transform) => XS_Physics.RayDebug(Alçada(transform), transform.forward, 1, capaEntorn);
                public static RaycastHit EndevantAprop(Transform transform) => XS_Physics.RayDebug(Alçada(transform), transform.forward, 0.5f, capaEntorn);

                //internal static RaycastHit TerraDevant(Transform transform) => Fisiques.Raig(Alçada(transform) + transform.forward, -transform.up, 2f, capaEntorn);
                public static RaycastHit CantonadaForat(Transform transform) => XS_Physics.RayDebug(Alçada(transform) + transform.forward - transform.up * 1.5f, -transform.forward, 1, capaEntorn);
                
                static RaycastHit AlsPeus(Transform transform) => XS_Physics.RayDebug(Alçada(transform), -transform.up, 1.2f, capaEntorn);
                static RaycastHit DiagonalDreta(Transform transform) => XS_Physics.RayDebug(Alçada(transform), (transform.forward + transform.right).normalized, 1, capaEntorn);
                static RaycastHit DiagonalEsquerra(Transform transform) => XS_Physics.RayDebug(Alçada(transform), (transform.forward - transform.right).normalized, 1, capaEntorn);
                static RaycastHit Dreta(Transform transform) => XS_Physics.RayDebug(Alçada(transform), transform.right, 1, capaEntorn);
                static RaycastHit Esquerra(Transform transform) => XS_Physics.RayDebug(Alçada(transform), -transform.right, 1, capaEntorn);
                static RaycastHit DiagonalAmunt(Transform transform) => XS_Physics.RayDebug(Alçada(transform), (transform.forward + transform.up).normalized, 1.45f, capaEntorn);
                static RaycastHit Amunt(Transform transform) => XS_Physics.RayDebug(Alçada(transform), transform.up, 1.45f, capaEntorn);
                static RaycastHit DavantDelsPeus(Transform transform) => XS_Physics.RayDebug(Alçada(transform) + transform.forward, -transform.up, 1.5f, capaEntorn);
                static RaycastHit DavantDelCap(Transform transform) => XS_Physics.RayDebug(Alçada(transform) + transform.up * 1.1f, transform.forward * 1f, 1, capaEntorn);
                static RaycastHit CantonadaSuperior(Transform transform) => XS_Physics.RayDebug(Alçada(transform) + transform.up * 1.1f + transform.forward * 1f, -transform.up, 1f, capaEntorn);
                static Vector3 Alçada(Transform transform) => transform.position + transform.up * 0.75f;


            }

            /// <summary>
            /// Buscar informacio del terra
            /// </summary>
            public static class Terra
            {
                static bool relliscar;
                static RaycastHit raycastHit;
                public static RaycastHit Hit(Transform transform)
                {
                    if (Devant(transform).Hitted()) return Devant(transform);
                    else if (Derrera(transform).Hitted()) return Derrera(transform);
                    else if (Dreta(transform).Hitted()) return Dreta(transform);
                    else if (Esquerra(transform).Hitted()) return Esquerra(transform);
                    else return Centre(transform);
                }
              
                /*public static RaycastHit Hit(Transform transform)
                {
                    return Unic(transform);
                }*/

                /*public static RaycastHit AmbEsfera(Transform transform)
                {
                    if (Physics.CheckSphere(transform.position + transform.up * 0.25f, .4f))
                    {
                        return Physics.SphereCastAll(transform.position + transform.up * 0.25f, .4f, -transform.up, 0.1f)[0];
                    }
                    else return nul;
                }*/

                /*public static RaycastHit CantonadaForat(Transform transform)
                {
                    if (!Dret.DavantDelsPeus(transform).Impactat() && Dret.CantonadaForat(transform).Impactat()) return Dret.CantonadaForat(transform);
                    else return nul;
                }*/
                public static Vector3 InclinacioForward(Transform transform)
                {
                    raycastHit = Hit(transform);
                    //if (raycastHit.Hitted() && raycastHit.normal.Pla())
                        return Vector3.Cross(transform.right, raycastHit.normal).normalized;
                   // else return Vector3.Cross(transform.right, Vector3.up).normalized;
                }
                public static Vector3 InclinacioRightFromHelper(Transform helper, Vector3 MovimentRelatiuACamera)
                {
                    //MovimentRelatiuACamera = MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment)
                    Vector3 inputRight = -Vector3.Cross(MovimentRelatiuACamera, helper.forward).normalized;
                    return Vector3.Cross(inputRight, helper.forward).normalized;
                    //return Vector3.Cross(transform.right, helper.forward).normalized;
                }
                public static Vector3 InclinacioForwardFromHelper(Transform helper, Vector3 MovimentRelatiuACamera)
                {
                    //MovimentRelatiuACamera = MyCamera.Transform.ACamaraRelatiu(Inputs.Moviment)
                    Vector3 inputRight = -Vector3.Cross(MovimentRelatiuACamera, helper.forward).normalized;
                    return Vector3.Cross(inputRight, helper.forward).normalized;
                }
                public static Vector3 InclinacioRightFromHelper(Transform transform, Transform helper) 
                {
                    return Vector3.Cross(-transform.right, -helper.forward).normalized;
                }
                public static Vector3 InclinacioForwardFromHelper(Transform transform, Transform helper)
                {
                    return Vector3.Cross(-transform.right, -helper.forward).normalized;
                }
                static Collider[] colliders;
                public static bool EsRelliscant(Transform transform)
                {
                    if (colliders == null) colliders = new Collider[1];

                    relliscar = false;

                    relliscar = true;

                    if (Centre(transform).Hitted() && !Centre(transform).Relliscar()) relliscar = false;
                    else if (Devant(transform).Hitted() && !Devant(transform).Relliscar()) relliscar = false;
                    else if (Derrera(transform).Hitted() && !Derrera(transform).Relliscar()) relliscar = false;
                    else if (Dreta(transform).Hitted() && !Dreta(transform).Relliscar()) relliscar = false;
                    else if (Esquerra(transform).Hitted() && !Esquerra(transform).Relliscar()) relliscar = false;
                    else if (!Hit(transform).Hitted()) relliscar = false;
                        
                    return relliscar;
                }
                public static bool HiHaEsglao(Transform transform, float altura = 0.5f)
                {
                    //Amb inclinacio
                    return XS_Physics.RayDebug(transform.position + transform.up * 0.05f, InclinacioForward(transform), 0.5f, capaEntorn).Hitted()
                        && !XS_Physics.RayDebug(transform.position + transform.up * (0.05f + altura), InclinacioForward(transform), 1.5f, capaEntorn).Hitted();

                    //Sense inlinacio
                    //return Fisiques.Raig(transform.position + transform.up * 0.1f, transform.forward, 0.5f, capaEntorn).Impactat()
                    //    && !Fisiques.Raig(transform.position + transform.up * (0.1f + altura), transform.forward, 1.5f, capaEntorn).Impactat();
                }

                public static bool NoTerra(Transform transform, bool preparat)
                {
                    return !Entorn.Buscar.Terra.Hit(transform).Hitted() &&
                        !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
                        preparat;
                }

                static RaycastHit Devant(Transform transform) => XS_Physics.RaySphereDebug(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up + transform.forward * (distBuscarTerra * 0.3f), distBuscarTerra, capaEntorn, 0.1f);
                static RaycastHit Derrera(Transform transform) => XS_Physics.RaySphereDebug(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up - transform.forward * (distBuscarTerra * 0.3f), distBuscarTerra, capaEntorn, 0.1f);
                static RaycastHit Dreta(Transform transform) => XS_Physics.RaySphereDebug(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up + transform.right * (distBuscarTerra * 0.3f), distBuscarTerra, capaEntorn, 0.1f);
                static RaycastHit Esquerra(Transform transform) => XS_Physics.RaySphereDebug(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up - transform.right * (distBuscarTerra * 0.3f), distBuscarTerra, capaEntorn, 0.1f);
                static RaycastHit Centre(Transform transform) => XS_Physics.RaySphereDebug(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up, distBuscarTerra, capaEntorn, 0.1f);

                public static RaycastHit Unic(Transform transform) => XS_Physics.RaySphereDebug(transform.position + transform.up * (distBuscarTerra + 0.4f - 0.1f), -transform.up, distBuscarTerra, capaEntorn, 0.35f);
            }


        }

        /// <summary>
        /// Informacio de l'entorn necessaria per la camara.
        /// </summary>
        public static class Camera
        {
            public static float DistanciaDesdeTerra(Transform transform) => XS_Physics.RayDistance(transform.position + Vector3.up, Vector3.down, 7, capaEntorn) - 1;

        }






    }
}

