using UnityEngine;
using UnityEngine.Events;

public abstract class Estat : MonoBehaviour
{
    protected Moviment3D.Informacio i;
    [SerializeField] private Condicio[] sortides;
    public Moviment3D.Informacio Informacio { set => i = value; }

    internal abstract void EnEntrar();
    internal abstract void EnSortir();
    internal abstract void EnUpdate();
    internal virtual void EnFixedUpdate() { }

    public void OnEnable()
    {
        EnEntrar();
        for (int i = 0; i < sortides.Length; i++)
        {
            sortides[i].My = this;
        }
    }
    private void OnDisable() => EnSortir();
    private void Update() => EnUpdate();
    private void FixedUpdate() => EnFixedUpdate();

    private void LateUpdate()
    {
        for (int i = 0; i < sortides.Length; i++)
        {
            sortides[i].Invocar(sortides[i]);
        }
    }

    internal static void Sortida(Condicio hab)
    {
        hab.My.enabled = false;
        hab.Objectiu.enabled = true;
    }


    [System.Serializable]
    public class Condicio
    {
        Estat my;
        [SerializeField] Estat objectiu;
        [SerializeField] UnityEvent<Condicio> condicions;
        public Estat My { get => my; set => my = value; }

        public Estat Objectiu => objectiu;

        public void Invocar(Condicio condicio)
        {
            condicions.Invoke(condicio);
        }
    }

}

