using UnityEngine;
using UnityEditor;

public class FaintEvent : BattleEvent
{
    public PokemonData faintTarget {get;set; }
    
    public FaintEvent(PokemonData faintTarget) {
        this.faintTarget = faintTarget;
    }
    
}