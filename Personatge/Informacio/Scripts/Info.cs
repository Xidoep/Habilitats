using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[DefaultExecutionOrder(-10)]
[CreateAssetMenu(menuName = "Xido Studio/Habilitats/Informacio", fileName = "Informacio")]
public class Info : ScriptableObject
{
    [SerializeField] LayerMask capaEntorn;

    [SerializeField] Resist resist;
    [SerializeField] AnimPlayer anim;
    [SerializeField] Inpt input;

    public Info Iniciar(Transform transform)
    {
        Debugar.Log("Iniciar");
        Info info = CreateInstance<Info>();
        info.capaEntorn = capaEntorn;
        info.resist = resist.Iniciar();
        info.anim = (AnimPlayer)anim.Iniciar(transform);
        info.input = input.Iniciar();
        return info;
    }

    public Resist Resist => resist;
    public Anim Anim => anim;

}
