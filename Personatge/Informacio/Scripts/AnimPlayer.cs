using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Moviment3D;

[CreateAssetMenu(menuName = "Xido Studio/Habilitats/Animacio player", fileName = "AnimPlayer")]
public class AnimPlayer : Anim
{

    const string dretNom = "Dret";
    const string dretaNom = "Dreta";
    const string plaNom = "Pla";
    const string saltarNom = "Salt";
    const string aireNom = "Aire";
    const string caureNom = "Caure";
    const string escalarNom = "Escalar";
    const string escalantNom = "Escalant";
    const string relliscarNom = "Relliscar";
    const string velocitatVerticalNom = "VelocitatVertical";
    const string movimentNom = "Moviment";
    const string movimentXNom = "MovimentX";
    const string movimentYNom = "MovimentY";
    const string saltPreparatNom = "SaltPreparat";
    const string saltAjupitNom = "SaltAjupit";
    const string saltEscalantNom = "SaltEscalant";
    const string reenganxarCantondaSuperiorNom = "ReenganxarCantondaSuperior";

    int dret;
    int dreta;
    int pla;
    int saltar;
    int aire;
    int caure;
    int escalar;
    int escalant;
    int relliscar;
    int velocitatVertical;
    int moviment;
    int movimentX;
    int movimentY;
    int saltPreparat;
    int saltAjupit;
    int saltEscalant;
    int reenganxarCantondaSuperior;

    bool aireFlanc;

    public object Iniciar(Transform transform)
    {
        AnimPlayer anim = CreateInstance<AnimPlayer>();
        anim.animator = transform.GetComponentInChildren<Animator>();

        dret = dretNom.ToHash();
        dreta = dretaNom.ToHash();
        pla = plaNom.ToHash();
        saltar = saltarNom.ToHash();
        aire = aireNom.ToHash();
        caure = caureNom.ToHash();
        escalar = escalarNom.ToHash();
        escalant = escalantNom.ToHash();
        relliscar = relliscarNom.ToHash();
        velocitatVertical = velocitatVerticalNom.ToHash();
        moviment = movimentNom.ToHash();
        movimentX = movimentXNom.ToHash();
        movimentY = movimentYNom.ToHash();
        saltPreparat = saltPreparatNom.ToHash();
        saltAjupit = saltAjupitNom.ToHash();
        saltEscalant = saltEscalantNom.ToHash();
        reenganxarCantondaSuperior = reenganxarCantondaSuperiorNom.ToHash();

        aireFlanc = false;

        return anim;
    }
    //REFERENCIES

    public void Dret()
    {
        Trigger(dret);
        Bool(aire, false);
        Bool(escalant, false);
    }
    public void Dreta(bool _dreta)
    {
        Bool(dreta, _dreta);
    }
    public void Saltar()
    {
        Trigger(saltar);
        Bool(aire, true);
        Bool(escalant, false);
    }
    public void Caure()
    {
        Trigger(caure);
        Bool(aire, true);
        Bool(escalant, false);
    }
    public void Escalar()
    {
        Trigger(escalar);
        Bool(aire, false);
        Bool(escalant, true);
    }
    public void Relliscar()
    {
        Trigger(relliscar);
        Bool(aire, false);
        Bool(escalant, false);
    }
    public void NoTerra(Transform transform, float velocitatGravetat, bool preparat)
    {
        //velocitatGravetat = i.Dinamic.VelocitatGravetat.y
        if (Aire(transform, preparat) && !aireFlanc)
        {
            Caure();
            Float(velocitatVertical, velocitatGravetat);
            aireFlanc = true;
        }
        else if (!Aire(transform, preparat) && aireFlanc)
        {
            Dret();
            aireFlanc = false;
        }
    }
    public void VelocitatVertical(float velocitatGravetat)
    {
        //velocitatGravetat = Dinamic.VelocitatGravetat.y
        Float(velocitatVertical, velocitatGravetat);
    }
    public void Moviment(Vector2 valor)
    {
        Float(movimentX, valor.x);
        Float(movimentY, valor.y);
    }
    public void MovimentY(float valor)
    {
        Float(movimentY, valor);
    }
    public void EnMoviment(bool enMoviment)
    {
        if (animator.GetBool(moviment).Equals(enMoviment))
            return;

        Bool(moviment, enMoviment);
    }
    public void Pla(bool _pla)
    {
        Bool(pla, _pla);
    }
    public void SaltPreparat(bool _saltPreparat)
    {
        Bool(saltPreparat, _saltPreparat);
    }
    public void SaltAjupid()
    {
        Trigger(saltAjupit);
        Bool(aire, true);
        Bool(escalant, false);
    }
    public void SaltEscalant()
    {
        Trigger(saltEscalant);
        Bool(aire, true);
        Bool(escalant, false);
    }
    public void ReenganxarCantondaSuperior()
    {
        Trigger(reenganxarCantondaSuperior);
    }






    bool Aire(Transform transform, bool preparat)
    {
        return !Entorn.Buscar.Terra.Hit(transform).Hitted() &&
         !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
         preparat;
    }


}
