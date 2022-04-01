using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Habilitats/CoyoteTime", fileName = "CoyoteTime")]
public class CoyoteTim : ScriptableObject
{
    public CoyoteTim Iniciar()
    {
        CoyoteTim coyoteTime = CreateInstance<CoyoteTim>();
        coyoteTime.activat = false;
        coyoteTime.temps = 1;
        return coyoteTime;
    }
    bool activat;
    float temps;

    public bool Temps(bool començarAContar, float temps)
    {
        if (!començarAContar)
        {
            activat = false;
            return false;
        }

        if (!activat)
        {
            activat = true;
            this.temps = temps;
            return false;
        }

        if (this.temps <= 0)
        {
            activat = false;
            return true;
        }

        this.temps -= Time.deltaTime;
        return false;
    }
    public void Stop()
    {
        activat = false;
    }
}
