using UnityEngine;
using UnityEditor;

[System.Serializable]
    public class PokemonData {

        public Pokemon basePokemon;
        public int pokemonLevel;
        public int currentHp;

        public int accuracyStatisticsChange = 0;
        public int evasionStatisticsChange = 0;
        public int attackStatisticsChange = 0;
        public int defenseStatisticsChange = 0;
    
        public int getAttackStat() {
            return calculateStat(basePokemon.attack);
        }

        public int getDefenseStat() {
            return calculateStat(basePokemon.defense);
        }

        public int getSpecialStat() {
            return calculateStat(basePokemon.special);
        }

        public int getSpeedStat() {
            return calculateStat(basePokemon.speed);
        }

        public int getHpStat() {
            return calculateHp();
        }

        

        private int calculateHp() {
            int iv = 12;
            int ev = 0;

            return Mathf.RoundToInt( 
                (((2 * basePokemon.hp + iv + ev/4)*pokemonLevel)/100) + pokemonLevel + 10
            );
            
        }
        private int calculateStat(int stat) {
            int iv = 12;
            int ev = 0;

            return Mathf.RoundToInt( 
                (((2 * basePokemon.hp + iv + ev/4)*pokemonLevel)/100) + 5
            );
        }
    }