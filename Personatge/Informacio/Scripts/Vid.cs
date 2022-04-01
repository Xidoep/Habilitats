using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Habilitats/Vida", fileName = "Vida")]
public class Vid : ScriptableObject
{
    public Vid Iniciar()
    {
        Vid vida = CreateInstance<Vid>();
        vida.canalMort = canalMort;
        vida.canalTrencat = canalTrencat;
        vida.trencat = false;
        return vida;
    }

    [SerializeField] Canal_Void canalMort;
    [SerializeField] Canal_Bool canalTrencat;
    bool trencat;
    public void Damage()
    {
        if (!trencat)
        {
            trencat = true;
            canalTrencat.Invocar(trencat);
            return;
        }

        trencat = false;
        canalMort.Invocar();
    }

    public void Restore()
    {
        trencat = false;
        canalTrencat.Invocar(trencat);
    }
}
