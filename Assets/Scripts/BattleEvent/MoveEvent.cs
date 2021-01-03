using UnityEngine;
using UnityEditor;

public class MoveEvent : BattleEvent
{
    public PokemonData pokemon {get; set; }

    public Move move {get; set; }

    public MoveEvent(PokemonData pokemon, Move move) {
        this.pokemon = pokemon;
        this.move = move;
    }
}