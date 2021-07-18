public interface GenXCalculator 
{
    int calculateDamage(PokemonData attacking, PokemonData defending, Move move);
    bool willMoveHit(PokemonData attackingPokemon, PokemonData defendingPokemon, Move move);
    Effectiveness decideEffectiveness(Move move, Pokemon pokemon);

}