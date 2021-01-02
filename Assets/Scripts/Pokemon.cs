using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "pokemon", menuName = "Pokemon/Pokemon")]
public class Pokemon : ScriptableObject
{
    public string name;

    public int hp;
    
    public int attack;
    public int defense;
    public int special;
    public int speed;

    public int level;

    public Sprite sprite;

    public List<Move> moves;
}
