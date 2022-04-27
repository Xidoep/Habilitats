using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EstatPersonatge : Estat
{
    Moviment3D.Informacio informacio;

    public Moviment3D.Informacio Informacio { set => informacio = value; }
    protected Moviment3D.Informacio i => informacio;

}
