using UnityEngine;

public class Gen3Calculator : GenXCalculator
{

    public int calculateDamage(PokemonData attacking, PokemonData defending, Move move) {

        int attack = attacking.getAttackStat();
        int defense = defending.getDefenseStat();

        attack = Mathf.RoundToInt(attack * Calculators.statusModifierFlatToPerccentage(attacking.attackStatisticsChange));
        defense = Mathf.RoundToInt(defense * Calculators.statusModifierFlatToPerccentage(defending.defenseStatisticsChange));

        if (move.effect == Effect.Special) {
            
            attack = attacking.getSpecialStat();
            defense = defending.getSpecialStat();
        }

        return Mathf.RoundToInt( 
            (((((attacking.pokemonLevel * 2)/5) + 2) * move.power *attack/defense)/50) +2 );
    }

    public bool willMoveHit(PokemonData attackingPokemon, PokemonData defendingPokemon, Move move) {
        float accuracyBase = move.accuracy;
        float accuracy = Calculators.evasionAccuracyFlatToPercentage(attackingPokemon.accuracyStatisticsChange);
        float evasion = Calculators.evasionAccuracyFlatToPercentage(defendingPokemon.evasionStatisticsChange);

        float calculatedAccuracy = (accuracyBase * (accuracy/evasion));
        if (calculatedAccuracy >= Random.Range(1, 100)) {
            
            return true;
        }
        
        return false;

    }

    public Effectiveness decideEffectiveness(Move move, Pokemon pokemon) {
        switch (move.type) {
            case Type.Fire : 
                return getFireEffectiveness(pokemon);
            case Type.Water : 
                return getWaterEffectiveness(pokemon);
            case Type.Grass : 
                return getGrassEffectiveness(pokemon);
        }
        return Effectiveness.Normal;
    }

    private Effectiveness getGrassEffectiveness(Pokemon pokemon) {
        if (pokemon.primaryType.Equals(Type.Water) || pokemon.secondaryType.Equals(Type.Water)) {
            return Effectiveness.TwiceWeak;
        } else {
            return Effectiveness.Normal;
        }
    }

    private Effectiveness getWaterEffectiveness(Pokemon pokemon) {
        if (pokemon.primaryType.Equals(Type.Fire) || pokemon.secondaryType.Equals(Type.Fire)) {
            return Effectiveness.TwiceWeak;
        } else {
            return Effectiveness.Normal;
        }
    }

    private Effectiveness getFireEffectiveness(Pokemon pokemon) {
        if (pokemon.primaryType.Equals(Type.Grass) || pokemon.secondaryType.Equals(Type.Grass)) {
            return Effectiveness.TwiceWeak;
        } else {
            return Effectiveness.Normal;
        }
    }

    
}