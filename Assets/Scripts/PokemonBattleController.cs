using UnityEngine;
using UnityEditor;

/**
 * handles only data changes, and business logic
 *
*/
public class PokemonBattleController
{
    private PokemonData ownPokemonData;
    private PokemonData enemyPokemonData;

    public delegate void triggerDamageWasDealt();
    public triggerDamageWasDealt triggerDamageWasDealtDelegate;
    

    public PokemonBattleController(PokemonData ownPokemonData, PokemonData enemyPokemonData) {
        this.ownPokemonData = ownPokemonData;
        this.enemyPokemonData = enemyPokemonData;
        
    }


    private int calculateDamage(PokemonData attacking, PokemonData defending, Move move) {

        int attack = attacking.getAttackStat();
        int defense = defending.getDefenseStat();
        if (move.effect == Effect.Special) {

            attack = attacking.getSpecialStat();
            defense = defending.getSpecialStat();
        }

        return Mathf.RoundToInt( 
            (((((attacking.pokemonLevel * 2)/5) + 2) * move.power *attack/defense)/50) +2 );
    }
    
    private void executeMove(PokemonData attackingPokemon, PokemonData defendingPokemon, Move move) {
        
        if (!willMoveHit(attackingPokemon, defendingPokemon, move)) {
            // DEelegate to text. moved missed
            return;
        }
        
        if (move.effect.Equals(Effect.Physical) || move.effect.Equals(Effect.Special) ) {
            
            int damage = calculateDamage(attackingPokemon, defendingPokemon, move);
            
            defendingPokemon.currentHp = defendingPokemon.currentHp - damage; 
        }
        if (move.effect.Equals(Effect.StatusEnemyAttack)) {
            if (defendingPokemon.attackStatisticsChange > -6) {
                defendingPokemon.attackStatisticsChange-= 1;
                // TODO message sucess
            } else {
                // TODO message, stopped
            }
            
        }
        if (move.effect.Equals(Effect.StatusEnemyDefense)) {
            if (defendingPokemon.defenseStatisticsChange > -6) {
                defendingPokemon.defenseStatisticsChange-= 1;
                // TODO message sucess
            } else {
                // TODO message, stopped
            }
            
        }
        if (move.effect.Equals(Effect.StatusEnemyAccuracy)) {
            
            if (defendingPokemon.accuracyStatisticsChange > -6 && defendingPokemon.accuracyStatisticsChange < 6) {
                defendingPokemon.accuracyStatisticsChange-= 1;
                // TODO message sucess
            } else {
                // TODO message, stopped
            }
            
        }
    }

    private bool willMoveHit(PokemonData attackingPokemon, PokemonData defendingPokemon, Move move) {
        float accuracyBase = move.accuracy;
        float accuracy = Calculators.evasionAccuracyFlatToPercentage(attackingPokemon.accuracyStatisticsChange);
        float evasion = Calculators.evasionAccuracyFlatToPercentage(defendingPokemon.evasionStatisticsChange);

        float calculatedAccuracy = (accuracyBase * (accuracy/evasion));
        if (calculatedAccuracy >= Random.Range(1, 100)) {
            
            return true;
        }
        
        return false;

    }

    private void executeEnemyTurn() {
        int moveToUse = Random.Range(0, enemyPokemonData.basePokemon.moves.Count);
        executeMove(enemyPokemonData, ownPokemonData, enemyPokemonData.basePokemon.moves[moveToUse]);
    }

    public void makeMove(int move) {

        if (ownPokemonData.getSpeedStat() > enemyPokemonData.getSpeedStat()) {
            executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
            executeEnemyTurn();
        } else {
            executeEnemyTurn();
            executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
        }

        triggerDamageWasDealtDelegate();
        
    }

}