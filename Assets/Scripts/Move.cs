using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(fileName = "move", menuName = "Pokemon/Move")]

public class Move : ScriptableObject
{

    public string name;

    public int power;

    public int accuracy;

    public Type type; 
    [Serializable]
    public enum Type {

        Normal, Flying
    }

    public Effect effect;
    // list effects
}