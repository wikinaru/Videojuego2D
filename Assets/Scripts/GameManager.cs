using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float barraVida;
    public static float gorgonVida1;
    public static float gorgonVida2;
    public static float gorgonVida3;
    public static float armaduraVida1;
    public static float armaduraVida2;
    public static bool morir = false;

    void Start()
    {
        barraVida = 10;
        gorgonVida1 = 5;
        gorgonVida2 = 7;
        gorgonVida3 = 7.5f;
        armaduraVida1 = 9;
        armaduraVida2 = 10.5f;
    }

    void Update()
    {
        
    }
}
