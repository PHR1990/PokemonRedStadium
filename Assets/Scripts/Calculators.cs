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
}