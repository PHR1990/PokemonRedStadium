using UnityEngine;
using UnityEditor;

public class Calculators 
{
    public static int evasionAccuracyFlatToPercentage(int flatValue) {
        switch (flatValue) {
            case -6 :
                return 33;
            case -5 :
                return 36;
            case -4 :
                return 43;
            case -3 :
                return 50;
            case -2 :
                return 66;
            case -1 :
                return 75;
            case 0 :
                return 100;
            case 1 :
                return 133;
            case 2 :
                return 166;
            case 3 :
                return 200;
            case 4 :
                return 250;
            case 5 :
                return 266;
            case 6 :
                return 300;
                
        }
        return 100;
    }

    public static float statusModifierFlatToPerccentage(int flatValue) {
        switch (flatValue) {
            case -6 :
                return 0.25f;
            case -5 :
                return 0.28f;
            case -4 :
                return 0.33f;
            case -3 :
                return 0.4f;
            case -2 :
                return 0.5f;
            case -1 :
                return 0.66f;
            case 0 :
                return 1;
            case 1 :
                return 1.5f;
            case 2 :
                return 2f;
            case 3 :
                return 2.5f;
            case 4 :
                return 3f;
            case 5 :
                return 3.5f;
            case 6 :
                return 4f;
                
        }
        return 1;

    }
}