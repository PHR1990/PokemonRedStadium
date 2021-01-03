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

    public delegate void triggerTurnsWereExecuted();
    public triggerTurnsWereExecuted triggerTurnsWereExecutedDelegate;

    public delegate void emitEvent(BattleEvent battleEvent);
    public emitEvent emitEventDelegate;

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
            if (attackingPokemon.Equals(ownPokemonData)) {
            emitEventDelegate(
                    new TextMessageEvent(attackingPokemon.basePokemon.name + "'s "
                    + " attack missed!"));
            } else {
                emitEventDelegate(
                    new TextMessageEvent("Foe " +attackingPokemon.basePokemon.name + "'s "
                    + " attack missed!"));
            }

            return;
        }
        
        if (move.effect.Equals(Effect.Physical) || move.effect.Equals(Effect.Special) ) {
            
            int damage = calculateDamage(attackingPokemon, defendingPokemon, move);
            
            defendingPokemon.currentHp = defendingPokemon.currentHp - damage; 

            emitEventDelegate(new MoveEvent(defendingPokemon, move));

            // TODO Not very effective, very effective
            
        }
        // TODO missing speed change
        if (move.effect.Equals(Effect.StatusEnemyAttack)) {
            if (defendingPokemon.attackStatisticsChange > -6) {
                defendingPokemon.attackStatisticsChange-= 1;
               
                emitEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " ATTACK fell!"));
            } else {
                emitEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " ATTACK cannot go any lower."));
            }
            
        }
        if (move.effect.Equals(Effect.StatusEnemyDefense)) {
            if (defendingPokemon.defenseStatisticsChange > -6) {
                defendingPokemon.defenseStatisticsChange-= 1;
                
                emitEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " DEFENSE fell!"));
            } else {
                emitEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " DEFENSE cannot go any lower."));
            }
            
        }
        if (move.effect.Equals(Effect.StatusEnemyAccuracy)) {
            
            if (defendingPokemon.accuracyStatisticsChange > -6 && defendingPokemon.accuracyStatisticsChange < 6) {
                defendingPokemon.accuracyStatisticsChange-= 1;
                
                emitEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " ACCURACY fell!"));

            } else {
                emitEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " ACCURACY cannot go any lower."));
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
        emitEventDelegate(new TextMessageEvent("Foe " + enemyPokemonData.basePokemon.name.ToUpper() + " used " + enemyPokemonData.basePokemon.moves[moveToUse].name ));
        executeMove(enemyPokemonData, ownPokemonData, enemyPokemonData.basePokemon.moves[moveToUse]);
    }

    public void makeMove(int move) {

        if (ownPokemonData.getSpeedStat() > enemyPokemonData.getSpeedStat()) {
            emitEventDelegate(new TextMessageEvent(ownPokemonData.basePokemon.name.ToUpper() + " used " + ownPokemonData.basePokemon.moves[move].name.ToUpper() ));
            executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
            
            executeEnemyTurn();
        } else {
            executeEnemyTurn();
            emitEventDelegate(new TextMessageEvent(ownPokemonData.basePokemon.name.ToUpper() + " used " + ownPokemonData.basePokemon.moves[move].name.ToUpper() ));
            executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
        }
        
        emitEventDelegate(new TextMessageEvent("What will " + ownPokemonData.basePokemon.name.ToUpper() + " do?"));

        triggerTurnsWereExecutedDelegate();
        
    }
    
}