using System.Collections;
using UnityEngine;

public class BattleController : MonoBehaviour
{
   
    public Team ownTeam;

    public Team enemyTeam;
    
    public AnimationController animationController;

    private bool isBusy = false;

    private GenXCalculator calculator = new Gen3Calculator();

    public delegate void triggerTurnsWereExecuted();
    public triggerTurnsWereExecuted triggerTurnsWereExecutedDelegate;

    public delegate void enqueueBattleEvent(BattleEvent battleEvent);
    public enqueueBattleEvent enqueueBattleEventDelegate;

    public void faintPokemon(PokemonData targetPokemon) {
        faintPokemonEnumerator(targetPokemon);
    }

    private IEnumerator faintPokemonEnumerator(PokemonData targetPokemon) {
        isBusy = true;

        if (getActiveOwnPokemon() == targetPokemon) {
            animationController.faintOwnPokemon();
        } else {
            animationController.faintEnemyPokemon();
        }
        isBusy = false;

        yield return new WaitForSeconds(0.5f);
    }

    private PokemonData getActiveEnemyPokemon() {
        return enemyTeam.pokemonData[0];
    }

    private PokemonData getActiveOwnPokemon() {
        return ownTeam.pokemonData[0];
    }

    private void executeMove(PokemonData attackingPokemon, PokemonData defendingPokemon, Move move) {

        if (!calculator.willMoveHit(attackingPokemon, defendingPokemon, move)) {
            if (attackingPokemon.Equals(getActiveOwnPokemon())) {
            enqueueBattleEventDelegate(
                    new TextMessageEvent(attackingPokemon.basePokemon.name + "'s "
                    + " attack missed!"));
            } else {
                enqueueBattleEventDelegate(
                    new TextMessageEvent("Foe " +attackingPokemon.basePokemon.name + "'s "
                    + " attack missed!"));
            }

            return;
        }
        
        if (move.effect.Equals(Effect.Physical) || move.effect.Equals(Effect.Special) ) {
            
            int damage = calculator.calculateDamage(attackingPokemon, defendingPokemon, move);
            
            Effectiveness effectiveness 
                = calculator.decideEffectiveness(move, defendingPokemon.basePokemon);

            if (effectiveness.Equals(Effectiveness.TwiceWeak)) {
                damage = damage * 2;
            }

            defendingPokemon.currentHp = defendingPokemon.currentHp - damage; 

            enqueueBattleEventDelegate(new MoveEvent(defendingPokemon, move));

            if (effectiveness.Equals(Effectiveness.TwiceWeak)) {
                enqueueBattleEventDelegate(
                    new TextMessageEvent("It's super effective!"));
            }

            if (defendingPokemon.currentHp < 1) {
                faintPokemon(defendingPokemon);
            }

            // TODO separete effectiveness in a separate class
            
        }
        // TODO missing speed change
        if (move.effect.Equals(Effect.StatusEnemyAttack)) {
            if (defendingPokemon.attackStatisticsChange > -6) {
                defendingPokemon.attackStatisticsChange-= 1;
               
                enqueueBattleEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " ATTACK fell!"));
            } else {
                enqueueBattleEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " ATTACK cannot go any lower."));
            }
            
        }
        if (move.effect.Equals(Effect.StatusEnemyDefense)) {
            if (defendingPokemon.defenseStatisticsChange > -6) {
                defendingPokemon.defenseStatisticsChange-= 1;
                
                enqueueBattleEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " DEFENSE fell!"));
            } else {
                enqueueBattleEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " DEFENSE cannot go any lower."));
            }
            
        }
        if (move.effect.Equals(Effect.StatusEnemyAccuracy)) {
            
            if (defendingPokemon.accuracyStatisticsChange > -6 && defendingPokemon.accuracyStatisticsChange < 6) {
                defendingPokemon.accuracyStatisticsChange-= 1;
                
                enqueueBattleEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " ACCURACY fell!"));

            } else {
                enqueueBattleEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + "'s "
                    + " ACCURACY cannot go any lower."));
            }
            
        }
    }

    private void executeEnemyTurn() {

        PokemonData enemyPokemonData, ownPokemonData;
        enemyPokemonData = getActiveEnemyPokemon();
        ownPokemonData = getActiveOwnPokemon();

        int moveToUse = Random.Range(0, enemyPokemonData.basePokemon.moves.Count);
        enqueueBattleEventDelegate(new TextMessageEvent("Foe " + enemyPokemonData.basePokemon.name.ToUpper() + " used " + enemyPokemonData.basePokemon.moves[moveToUse].name ));
        executeMove(enemyPokemonData, ownPokemonData, enemyPokemonData.basePokemon.moves[moveToUse]);
    }

    public void executeMove(int move) {
        
        enqueueBattleEventDelegate(new PlayerMustWaitEvent());
        
        PokemonData ownPokemonData = getActiveOwnPokemon();
        PokemonData enemyPokemonData = getActiveEnemyPokemon();

        if (ownPokemonData.getSpeedStat() > enemyPokemonData.getSpeedStat()) {
            enqueueBattleEventDelegate(new TextMessageEvent(ownPokemonData.basePokemon.name.ToUpper() + " used " + ownPokemonData.basePokemon.moves[move].name.ToUpper() ));
            executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
            
            if (enemyPokemonData.currentHp > 0) {
                executeEnemyTurn();
            }
            
        } else {
            executeEnemyTurn();
            enqueueBattleEventDelegate(new TextMessageEvent(ownPokemonData.basePokemon.name.ToUpper() + " used " + ownPokemonData.basePokemon.moves[move].name.ToUpper() ));

            if (ownPokemonData.currentHp > 0) {
                executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
            }
            
        }

        triggerTurnsWereExecutedDelegate();
        enqueueBattleEventDelegate(new TextMessageEvent("What will \n" + ownPokemonData.basePokemon.name.ToUpper() + " do?"));
    }
    
}