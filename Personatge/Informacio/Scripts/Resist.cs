using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Habilitats/Resistance", fileName = "Resistance")]
public class Resist : ScriptableObject
{
    public Resist Iniciar() 
    {
        Resist resist = CreateInstance<Resist>();
        resist.canalResistencia = canalResistencia;
        resist.maxim = maxim;
        resist.actual = maxim;
        resist.testing = testing;
        return resist;
    }
    [SerializeField] Canal_FloatFloat canalResistencia;

    [SerializeField] float maxim = 10;
    [SerializeField] float actual = 10;
    [SerializeField] bool testing;
    public bool Testing { get => testing; set => testing = value; }

    float Actual
    {
        set
        {
            actual = Mathf.Clamp(value, 0, maxim);
            canalResistencia.Invocar(actual, maxim);
        }
        get => !testing ? actual : maxim;
    }
    public bool Zero => actual <= 0;
    public bool UnaMica => actual > 0.1f;

    public float Percentatge => actual / maxim;

    public void SaltarFort()
    {
        Actual -= 2;
        Actual = Mathf.Max(Actual, 0.1f);
    }
    public void Saltar()
    {
        Actual -= 1;
    }
    public void NoBuidarDelTot() => Actual = Mathf.Max(Actual, 0.1f);

    public void Gastar() => Actual -= 0.75f * Time.deltaTime;
    public void GastarLentament() => Actual -= 0.05f * Time.deltaTime;

    public void Recuperar() => Actual += 3 * Time.deltaTime;
    public void RescuperarLentament() => Actual += 0.5f * Time.deltaTime;
}
