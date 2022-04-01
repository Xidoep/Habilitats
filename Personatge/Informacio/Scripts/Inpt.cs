using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Habilitats/Inputs", fileName = "Inputs")]
public class Inpt : ScriptableObject
{
    public Inpt Iniciar(Transform transform)
    {
        Inpt input = CreateInstance<Inpt>();
        input.moviment = moviment;
        input.saltar = saltar;
        input.agafar = agafar;
        input.deixar = deixar;
        input.saltEscalantPreparat = false;
        input.saltEscalantReenganxarse = false;
        input.agafarPerformed = false;
        input.agafar.OnPerformedAdd(input.AgafarPerformed);

        playerInput = transform.GetComponent<PlayerInput>();
        playerInput.enabled = false;
        playerInput.enabled = true;

        return input;
    }
    private void OnDisable()
    {
        agafar.OnPerformedRemove(AgafarPerformed);
    }

    [SerializeField] InputActionReference moviment;
    [SerializeField] InputActionReference saltar;
    [SerializeField] InputActionReference agafar;
    [SerializeField] InputActionReference deixar;

    PlayerInput playerInput;

    EscalarVectors vectors;

    bool saltEscalantPreparat;
    bool saltEscalantReenganxarse;

    bool agafarPerformed = false;



    public bool SaltEscalantPreparat
    {
        get => saltEscalantPreparat;
        set => saltEscalantPreparat = value;
    }
    public bool SaltEscalantReenganxarse
    {
        get => saltEscalantReenganxarse;
        set => saltEscalantReenganxarse = value;
    }

    //public static bool Escalar => agafar.GetBool();
    public bool Escalar => agafarPerformed;
    public void DisableEscalar() => agafarPerformed = false;
    public Vector2 Moviment => moviment.GetVector2();

    public bool MovimentZero => moviment.GetVector2() == Vector2.zero;
    public bool Saltar => saltar.GetBool();
    public bool Deixar => deixar.GetBool();
    public Transform SetHelperVectors
    {
        set
        {
            vectors.up = value.up;
            vectors.right = value.right;
            vectors.forward = value.forward;
        }
    }
    public Vector3 GetHelperUp => vectors.up;
    public Vector3 GetHelperRight => vectors.right;
    public Vector3 GetHelperForward => vectors.forward;


    public bool AreActived => moviment != null;

    public void AgafarPerformed(InputAction.CallbackContext obj) => agafarPerformed = agafar.GetBool();



    internal struct EscalarVectors
    {
        public Vector3 up;
        public Vector3 right;
        public Vector3 forward;
    }
}

