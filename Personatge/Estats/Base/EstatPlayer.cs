using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EstatPlayer : Estat
{
    [SerializeField] Info informacio;

    public Info Informacio { set => informacio = value; }
    protected Info i => informacio;
}
