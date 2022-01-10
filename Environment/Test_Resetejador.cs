using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Resetejador : MonoBehaviour
{
    public GameObject elementReinizable;
    public Vector3 posicioInicial;


    void Start()
    {
        posicioInicial = elementReinizable.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == elementReinizable)
        {
            elementReinizable.transform.position = posicioInicial;
        }
    }
}
