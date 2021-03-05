using UnityEngine;

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

        attack = Mathf.RoundToInt(attack * Calculators.statusModifierFlatToPerccentage(attacking.attackStatisticsChange));
        defense = Mathf.RoundToInt(defense * Calculators.statusModifierFlatToPerccentage(defending.defenseStatisticsChange));

        if (move.effect == Effect.Special) {
            
            attack = attacking.getSpecialStat();
            defense = defending.getSpecialStat();
        }

        return Mathf.RoundToInt( 
            (((((attacking.pokemonLevel * 2)/5) + 2) * move.power *attack/defense)/50) +2 );
    }
    
    private void executeMove(PokemonData attackingPokemon, PokemonData defendingPokemon, Move move) {
        
        // TODO need to implement sometihng else for selfdestruct
        

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
            
            Effectiveness effectiveness 
                = moveEffectiveness(move, defendingPokemon.basePokemon);

            if (effectiveness.Equals(Effectiveness.TwiceWeak)) {
                damage = damage * 2;
            }

            defendingPokemon.currentHp = defendingPokemon.currentHp - damage; 

            emitEventDelegate(new MoveEvent(defendingPokemon, move));

            if (effectiveness.Equals(Effectiveness.TwiceWeak)) {
                emitEventDelegate(
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
            
            if (enemyPokemonData.currentHp > 0) {
                executeEnemyTurn();
            }
            
        } else {
            executeEnemyTurn();
            emitEventDelegate(new TextMessageEvent(ownPokemonData.basePokemon.name.ToUpper() + " used " + ownPokemonData.basePokemon.moves[move].name.ToUpper() ));

            if (ownPokemonData.currentHp > 0) {
                executeMove(ownPokemonData, enemyPokemonData, ownPokemonData.basePokemon.moves[move]);
            }
            
        }
        
        emitEventDelegate(new TextMessageEvent("What will " + ownPokemonData.basePokemon.name.ToUpper() + " do?"));

        triggerTurnsWereExecutedDelegate();
        
    }

    private Effectiveness moveEffectiveness(Move move, Pokemon pokemon) {
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

    private enum Effectiveness {
        FourWeak, TwiceWeak, Normal, TwiceResistant, FourResistant, Immune, Absorb
    }

    private void faintPokemon(PokemonData defendingPokemon) {
        emitEventDelegate(
                    new TextMessageEvent(defendingPokemon.basePokemon.name + " fainted!"));

        emitEventDelegate(new FaintEvent(defendingPokemon));

        defendingPokemon = null;
    }

    public void switchEnemyPokemon() {
    }

    public void switchOwnPokemon(PokemonData pokemonData) {

        ownPokemonData = pokemonData;
    }

}