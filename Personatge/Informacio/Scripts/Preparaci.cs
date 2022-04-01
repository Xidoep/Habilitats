using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Habilitats/Preparacio", fileName = "Preparacio")]
public class Preparaci : ScriptableObject
{
    public Preparaci Iniciar()
    {
        Preparaci preparacio = CreateInstance<Preparaci>();
        preparacio.coroutine = null;
        preparacio.preparat = false;
        return preparacio;
    }

    Coroutine coroutine;
    bool preparat = false;
    public bool Preparat => preparat;

    public float Preparar
    {
        set
        {
            preparat = false;
            coroutine.StopCoroutine();
            coroutine = XS_Coroutine.StartCoroutine_Ending(value, PreparatTrue);
        }
    }
    void PreparatTrue() => preparat = true;
}
