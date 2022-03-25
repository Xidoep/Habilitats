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

    static public int dret;
    static public int dreta;
    static public int pla;
    static public int saltar;
    static public int aire;
    static public int caure;
    static public int escalar;
    static public int escalant;
    static public int relliscar;
    static public int velocitatVertical;
    static public int moviment;
    static public int movimentX;
    static public int movimentY;
    static public int saltPreparat;
    static public int saltAjupit;
    static public int saltEscalant;
    static public int reenganxarCantondaSuperior;

    //static bool aireFlanc;
    public object Iniciar(Transform transform)
    {
        AnimPlayer anim = CreateInstance<AnimPlayer>();
        anim.animator = transform.GetComponentInChildren<Animator>();

        AnimPlayer.dret = dretNom.ToHash();
        AnimPlayer.dreta = dretaNom.ToHash();
        AnimPlayer.pla = plaNom.ToHash();
        AnimPlayer.saltar = saltarNom.ToHash();
        AnimPlayer.aire = aireNom.ToHash();
        AnimPlayer.caure = caureNom.ToHash();
        AnimPlayer.escalar = escalarNom.ToHash();
        AnimPlayer.escalant = escalantNom.ToHash();
        AnimPlayer.relliscar = relliscarNom.ToHash();
        AnimPlayer.velocitatVertical = velocitatVerticalNom.ToHash();
        AnimPlayer.moviment = movimentNom.ToHash();
        AnimPlayer.movimentX = movimentXNom.ToHash();
        AnimPlayer.movimentY = movimentYNom.ToHash();
        AnimPlayer.saltPreparat = saltPreparatNom.ToHash();
        AnimPlayer.saltAjupit = saltAjupitNom.ToHash();
        AnimPlayer.saltEscalant = saltEscalantNom.ToHash();
        AnimPlayer.reenganxarCantondaSuperior = reenganxarCantondaSuperiorNom.ToHash();
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
    public void NoTerra(Transform transform)
    {
        /*if (Aire(transform) && !aireFlanc)
        {
            Caure();
            Float(velocitatVertical, Dinamic.VelocitatGravetat.y);
            aireFlanc = true;
        }
        else if (!Aire(transform) && aireFlanc)
        {
            Dret();
            aireFlanc = false;
        }*/
    }
    public void VelocitatVertical()
    {
        Float(velocitatVertical, Dinamic.VelocitatGravetat.y);
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






    bool Aire(Transform transform)
    {
        return !Entorn.Buscar.Terra.Hit(transform).Hitted() &&
         !Entorn.Buscar.Terra.HiHaEsglao(transform) &&
           Preparacio.Preparat;
    }


}
