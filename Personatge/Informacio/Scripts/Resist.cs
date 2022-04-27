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
        Instantiate(ui);
        return resist;
    }
    [SerializeField] GameObject ui;
    [SerializeField] Canal_FloatFloatFloat canalResistencia;

    [SerializeField] float maxim = 10;
    [SerializeField] float actual = 10;
    [SerializeField] float gastant = 0;
    [SerializeField] bool testing;
    public bool Testing { get => testing; set => testing = value; }

    float Actual
    {
        set
        {
            actual = Mathf.Clamp(value, 0, maxim);
            canalResistencia.Invocar(actual, maxim, gastant);
        }
        get => !testing ? actual : maxim;
    }
    public bool Zero => actual <= 0;
    public bool UnaMica => actual > 0.1f;

    public float Percentatge => actual / maxim;

    public void SaltarFort()
    {
        gastant = 2;
        Actual = Mathf.Max(Actual - 2, 0.1f);
    }
    public void Saltar()
    {
        gastant = 1;
        Actual -= 1;
    }
    public void NoBuidarDelTot() {
        Actual = Mathf.Max(Actual, 0.1f);
    }

    public void Gastar() {
        gastant = 0.75f;
        Actual -= 0.75f * Time.deltaTime;
    }
    public void Gastar(float extra) {
        gastant = 0.75f + (extra < 0 ? extra * .5f : extra);
        Actual -= (0.75f + (extra < 0 ? extra * .5f : extra)) * Time.deltaTime;
    }
    public void GastarLentament() {
        gastant = 0.25f;
        Actual -= 0.05f * Time.deltaTime;
    }

    public void Recuperar() {
        gastant = 0;
        Actual += 3 * Time.deltaTime;
    }
    public void RescuperarLentament() {
        gastant = 0;
        Actual += 0.5f * Time.deltaTime;
    }
}
