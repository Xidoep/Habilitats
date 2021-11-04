using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

namespace Moviment3D
{
    //SHAREABLE!
    public static class Entorn
    {
        public static void Iniciar(LayerMask _capaEntorn)
        {
            capaEntorn = _capaEntorn;
        }

        

        public static LayerMask capaEntorn;
        static RaycastHit nul;



        static float distMovEscalar => 0.5f;
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
            if (Fisiques.Raig(helper.position + helper.up * 2 + helper.forward, -helper.forward, 2, capaEntorn, 3).Impactat())
                normals.Add(Fisiques.Raig(helper.position + helper.up * 2 + helper.forward, -helper.forward, 2, capaEntorn, 3).normal);

            if (Fisiques.Raig(helper.position + helper.up * 1 + helper.right * 0.45f + helper.forward, -helper.forward, 2, capaEntorn, 3).Impactat())
                normals.Add(Fisiques.Raig(helper.position + helper.up * 1 + helper.right * 0.45f + helper.forward, -helper.forward, 2, capaEntorn, 3).normal);

            if (Fisiques.Raig(helper.position + helper.up * 1 - helper.right * 0.45f + helper.forward, -helper.forward, 2, capaEntorn, 3).Impactat())
                normals.Add(Fisiques.Raig(helper.position + helper.up * 1 - helper.right * 0.45f + helper.forward, -helper.forward, 2, capaEntorn, 3).normal);

            Debug.Log($"{normals.Count} normals");
            for (int i = 0; i < normals.Count; i++)
            {
                Debug.Log($"{normals[i]}");
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
            public static RaycastHit Moviment(Transform helper, bool pla, Vector2 moviment)
            {
                RaycastHit hit = nul;
                if (Buscar.Bloquejat(helper, pla, moviment).Impactat()) 
                {
                    Debugar.Log("BLoquejat");
                    hit = Buscar.Bloquejat(helper, pla, moviment); //Mur davant d'on vol anar
                    //HitNormal(ref hit, helper);
                } 
                else
                {
                    if (Buscar.Recta(helper, pla, moviment).Impactat()) //Si hi ha paret per continuar
                    {
                        if (!pla)
                        {
                            if (moviment.normalized.y > 0) //Si vas cap amunt
                            {
                                if (Buscar.Sostre(helper, pla, moviment).Impactat()) 
                                {
                                    Debugar.Log("Sostre");
                                    hit = Buscar.Sostre(helper, pla, moviment); //Si mes amunt hi torves un sostre
                                    //HitNormal(ref hit, helper);
                                }
                                else
                                {
                                    if (Buscar.CantonadaPlanaAmunt(helper).Impactat())
                                    {
                                        Debugar.Log("Cantonada plana mes amunt");
                                        hit = Buscar.CantonadaPlanaAmunt(helper); //Si torves una cantonada plana
                                        //HitNormal(ref hit, helper);
                                    }
                                    else 
                                    {
                                        Debugar.Log("Recta, no pla i amunt");
                                        hit = Buscar.Recta(helper, pla, moviment);
                                        //HitNormal(ref hit, helper);
                                    } 
                                }
                            }
                            else
                            {
                                Debugar.Log("Recta no amunt");
                                hit = Buscar.Recta(helper, pla, moviment);
                                //HitNormal(ref hit, helper);
                            }
                        }
                        else
                        {
                            Debugar.Log("Recta i pla");
                            hit = Buscar.Recta(helper, pla, moviment);
                            //HitNormal(ref hit, helper);
                        }
                    }
                    else
                    {
                        if (Buscar.Cantonada(helper, pla, moviment).Impactat()) 
                        {
                            Debugar.Log("Cantonada");
                            hit = Buscar.Cantonada(helper, pla, moviment); //Busca cantonada
                            //HitNormal(ref hit, helper);
                        } 
                    }
                }
                return hit;
            }
            static Vector3 Direccio(Transform helper, bool pla, Vector2 moviment) => !pla ? Direccio_Vertical(helper, moviment) : Direccio_Pla(moviment);
            static Vector3 Direccio_Vertical(Transform helper, Vector2 moviment) => helper.up * moviment.normalized.y - helper.right * moviment.normalized.x;
            static Vector3 Direccio_Pla(Vector2 moviment) => LaMevaCamara.Transform.ACamaraRelatiu(moviment).normalized;

            /// <summary>
            /// Accions de buscar propies d'escalar
            /// </summary>
            public static class Buscar
            {
                public static RaycastHit CantonadaPlanaAmunt(Transform helper)
                {
                   

                    if (!Fisiques.Raig(
                        Separacio(helper), 
                        helper.up, 
                        distMovEscalar * 2.2f, capaEntorn, 1).Impactat())
                    {
                        if (!Fisiques.Raig(
                            Separacio(helper) + Amunt(helper), 
                            -helper.forward, 
                            1 + distMovEscalar, capaEntorn, 1).Impactat())
                        {
                            if (Fisiques.Raig(
                                Separacio(helper) + Amunt(helper) - Endevant(helper), 
                                -helper.up, 
                                distMovEscalar * 4, capaEntorn, 1).Impactat())
                                return Fisiques.Raig(
                                    Separacio(helper) + Amunt(helper) - Endevant(helper), 
                                    -helper.up, 
                                    distMovEscalar * 4, capaEntorn, 1);
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
                    // if (!Fisiques.Raig(transform.position + transform.up * 1.5f, -transform.forward, 1.5f, capaEntorn, 1).Impactat())
                    // {
                    if (Fisiques.Raig(
                        transform.position + transform.up * 1.5f + transform.forward * 1.5f, 
                        -transform.up, 
                        1.5f, capaEntorn, 1).Impactat())
                    {
                        return Fisiques.Raig(
                            transform.position + transform.up * 1.5f + transform.forward * 1.5f, 
                            -transform.up, 
                            1.5f, capaEntorn, 1);
                    }
                    else return nul;
                    //  }
                    // else return nul;
                }
                internal static RaycastHit Bloquejat(Transform helper, bool pla, Vector2 moviment) => 
                    Fisiques.Raig(
                        Separacio(helper), 
                        Direccio(helper, pla, moviment), 
                        distMovEscalar * 1.5f, capaEntorn, 1);
                internal static RaycastHit Recta(Transform helper, bool pla, Vector2 moviment) => 
                    Fisiques.Raig(
                        Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar, 
                        -helper.forward, 
                        1, capaEntorn, 1);
                internal static RaycastHit Cantonada(Transform helper, bool pla, Vector2 moviment)
                {
                    if (Fisiques.Raig(
                        Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar - helper.forward * 1, 
                        -Direccio(helper, pla, moviment) - helper.forward, 
                        1, capaEntorn, 1).Impactat())
                        return Fisiques.Raig(
                            Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar - helper.forward * 1, 
                            -Direccio(helper, pla, moviment) - helper.forward, 
                            1, capaEntorn, 1);
                    else return Fisiques.Raig(
                        Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar - helper.forward * 1, 
                        -Direccio(helper, pla, moviment), 
                        1, capaEntorn, 1);
                }
                internal static RaycastHit Sostre(Transform helper, bool pla, Vector2 moviment) => 
                    Fisiques.Raig(
                        Separacio(helper) + Direccio(helper, pla, moviment) * distMovEscalar, 
                        helper.up, 
                        1, capaEntorn, 1);

               
                static Vector3 Amunt(Transform helper) => helper.up * (distMovEscalar * 2.2f);
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
                public static RaycastHit OnComencarAEscalar(Transform transform) //no escalant
                {
                    if (Endevant(transform).Impactat()) return Endevant(transform);
                    else
                    {
                        if (!DavantDelsPeus(transform).Impactat() && CantonadaForat(transform).Impactat()) return CantonadaForat(transform);
                        else
                        {
                            if (AlsPeus(transform).Impactat()) return AlsPeus(transform);
                            else
                            {
                                if (DiagonalDreta(transform).Impactat()) return DiagonalDreta(transform);
                                else
                                {
                                    if (DiagonalEsquerra(transform).Impactat()) return DiagonalEsquerra(transform);
                                    else
                                    {
                                        if (Dreta(transform).Impactat()) return Dreta(transform);
                                        else
                                        {
                                            if (Esquerra(transform).Impactat()) return Esquerra(transform);
                                            else
                                            {
                                                if (DiagonalAmunt(transform).Impactat()) return DiagonalAmunt(transform);
                                                else
                                                {
                                                    if (Amunt(transform).Impactat()) return Amunt(transform);
                                                    else
                                                    {
                                                        if (DavantDelsPeus(transform).Impactat()) return DavantDelsPeus(transform);
                                                        else return nul;
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
                public static RaycastHit Endevant(Transform transform) => Fisiques.Raig(Alçada(transform), transform.forward, 1, capaEntorn);
                //internal static RaycastHit TerraDevant(Transform transform) => Fisiques.Raig(Alçada(transform) + transform.forward, -transform.up, 2f, capaEntorn);
                public static RaycastHit CantonadaForat(Transform transform) => Fisiques.Raig(Alçada(transform) + transform.forward - transform.up * 1.5f, -transform.forward, 1, capaEntorn);
                
                static RaycastHit AlsPeus(Transform transform) => Fisiques.Raig(Alçada(transform), -transform.up, 1.2f, capaEntorn);
                static RaycastHit DiagonalDreta(Transform transform) => Fisiques.Raig(Alçada(transform), (transform.forward + transform.right).normalized, 1, capaEntorn);
                static RaycastHit DiagonalEsquerra(Transform transform) => Fisiques.Raig(Alçada(transform), (transform.forward - transform.right).normalized, 1, capaEntorn);
                static RaycastHit Dreta(Transform transform) => Fisiques.Raig(Alçada(transform), transform.right, 1, capaEntorn);
                static RaycastHit Esquerra(Transform transform) => Fisiques.Raig(Alçada(transform), -transform.right, 1, capaEntorn);
                static RaycastHit DiagonalAmunt(Transform transform) => Fisiques.Raig(Alçada(transform), (transform.forward + transform.up).normalized, 1.45f, capaEntorn);
                static RaycastHit Amunt(Transform transform) => Fisiques.Raig(Alçada(transform), transform.up, 1.45f, capaEntorn);
                static RaycastHit DavantDelsPeus(Transform transform) => Fisiques.Raig(Alçada(transform) + transform.forward, -transform.up, 1.5f, capaEntorn);
                static Vector3 Alçada(Transform transform) => transform.position + transform.up * 0.75f;

            }

            /// <summary>
            /// Buscar informacio del terra
            /// </summary>
            public static class Terra
            {
                static bool relliscar;

                public static RaycastHit Hit(Transform transform)
                {
                    if (Devant(transform).Impactat()) return Devant(transform);
                    else if (Derrera(transform).Impactat()) return Derrera(transform);
                    else if (Dreta(transform).Impactat()) return Dreta(transform);
                    else if (Esquerra(transform).Impactat()) return Esquerra(transform);
                    else return Centre(transform);
                }
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
                    if (Hit(transform).Impactat())
                        return Vector3.Cross(transform.right, Hit(transform).normal).normalized;
                    else return Vector3.Cross(transform.right, Vector3.up).normalized;
                }
                public static bool EsRelliscant(Transform transform)
                {

                    relliscar = true;
                    if (Centre(transform).Impactat() && !Centre(transform).normal.Relliscar()) relliscar = false;
                    else if (Devant(transform).Impactat() && !Devant(transform).normal.Relliscar()) relliscar = false;
                    else if (Derrera(transform).Impactat() && !Derrera(transform).normal.Relliscar()) relliscar = false;
                    else if (Dreta(transform).Impactat() && !Dreta(transform).normal.Relliscar()) relliscar = false;
                    else if (Esquerra(transform).Impactat() && !Esquerra(transform).normal.Relliscar()) relliscar = false;
                    else if (!Hit(transform).Impactat()) relliscar = false;
                    return relliscar;
                    //return Physics.CheckSphere(transform.position + transform.up * 0.3f, .4f, capaEntorn);
                }
                public static bool HiHaEsglao(Transform transform, float altura = 0.5f)
                {
                    //Amb inclinacio
                    return Fisiques.Raig(transform.position + transform.up * 0.1f, InclinacioForward(transform), 0.5f, capaEntorn).Impactat()
                        && !Fisiques.Raig(transform.position + transform.up * (0.1f + altura), InclinacioForward(transform), 1.5f, capaEntorn).Impactat();

                    //Sense inlinacio
                    //return Fisiques.Raig(transform.position + transform.up * 0.1f, transform.forward, 0.5f, capaEntorn).Impactat()
                    //    && !Fisiques.Raig(transform.position + transform.up * (0.1f + altura), transform.forward, 1.5f, capaEntorn).Impactat();
                }


                static RaycastHit Devant(Transform transform) => Fisiques.RaigEsfera(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up + transform.forward * (distBuscarTerra * 0.3f), distBuscarTerra, capaEntorn, 0.1f);
                static RaycastHit Derrera(Transform transform) => Fisiques.RaigEsfera(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up - transform.forward * (distBuscarTerra * 0.3f), distBuscarTerra, capaEntorn, 0.1f);
                static RaycastHit Dreta(Transform transform) => Fisiques.RaigEsfera(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up + transform.right * (distBuscarTerra * 0.3f), distBuscarTerra, capaEntorn, 0.1f);
                static RaycastHit Esquerra(Transform transform) => Fisiques.RaigEsfera(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up - transform.right * (distBuscarTerra * 0.3f), distBuscarTerra, capaEntorn, 0.1f);
                static RaycastHit Centre(Transform transform) => Fisiques.RaigEsfera(transform.position + transform.up * (distBuscarTerra - 0.1f), -transform.up, distBuscarTerra, capaEntorn, 0.1f);

            }


        }

        /// <summary>
        /// Informacio de l'entorn necessaria per la camara.
        /// </summary>
        public static class Camera
        {
            public static float DistanciaDesdeTerra(Transform transform) => Fisiques.RaigDistancia(transform.position + Vector3.up, Vector3.down, 7, capaEntorn) - 1;

        }






    }
}

