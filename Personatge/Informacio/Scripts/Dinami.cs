using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Habilitats/Dinamic", fileName = "Dinamic")]
public class Dinami : ScriptableObject
{

    public Dinami Iniciar(Transform transform)
    {
        Dinami dinamic = CreateInstance<Dinami>();
        dinamic.transform = transform;
        dinamic.rigidbody = transform.GetComponent<Rigidbody>();
        dinamic.frames = new List<Vector3>();
        dinamic.posicioFameAnterior = Vector3.zero;
        dinamic.velocitat = Vector3.zero;
        dinamic.tmpCalculs = Vector3.zero;

        XS_Coroutine.StartCoroutine_FixedUpdate(dinamic.ActualitzarSmooth);
        return dinamic;
    }

    Transform transform;
    Rigidbody rigidbody;
    List<Vector3> frames;
    Vector3 posicioFameAnterior;
    Vector3 velocitat;
    Vector3 tmpCalculs;

    public Rigidbody Rigidbody => rigidbody;
    public Vector3 Velocitat => velocitat;
    public Vector3 VelocitatSalt => velocitat * 30;
    public Vector3 VelocitatGravetat => velocitat * 4;
    public bool VelocitatVerticalNegativa => velocitat.y < 0;
    public Vector3 VelocitatHoritzontal => new Vector3(velocitat.x, 0, velocitat.z);

    //Retorna un numero entre 0 i 1 per multiplicar al moviment extra aeri, per evitar desplaçaments massa llargs pero permetent control.
    //Aquest valor te en compte la velocitat actual i si l'input va en la mateixa direccio que la velocitat.
    public float MultiplicadorMovimentAeri(Vector3 moviment) =>
        Mathf.Max(
            1 - Mathf.Clamp01(Vector3.Dot(VelocitatHoritzontal.normalized, moviment) * 2),
            1 - Mathf.Clamp01(VelocitatHoritzontal.magnitude * 35));



    /// <summary>
    /// Assigna la velocitat actual a partir de la diferencia de posico amb el frame anterior.
    /// </summary>
    public void Actualitzar(Transform transform)
    {
        velocitat = transform.position - posicioFameAnterior;
        posicioFameAnterior = transform.position;
    }

    /// <summary>
    /// asigna la velocitat actual amb la interpolacio de la diferencia de posicions dels 3 frames anteriors.
    /// </summary>
    public void ActualitzarSmooth(Transform transform)
    {
        if (frames == null) frames = new List<Vector3>();
        if (frames.Count == 0)
        {
            velocitat = transform.position - posicioFameAnterior;
            posicioFameAnterior = transform.position;
            frames.Add(transform.position);
            return;
        }

        tmpCalculs = transform.position - frames[0];
        for (int i = 1; i < frames.Count; i++)
        {
            tmpCalculs += frames[i - 1] - frames[i];
        }
        tmpCalculs /= frames.Count;
        velocitat = tmpCalculs;

        frames.Add(transform.position);
        if (frames.Count > 3) frames.RemoveAt(0);
    }
    public void ActualitzarSmooth() => ActualitzarSmooth(transform);



    public void Stop()
    {
        frames.Clear();
        tmpCalculs = Vector3.zero;
        velocitat = Vector3.zero;
    }

}
