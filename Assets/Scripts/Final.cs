using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Final : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public void VolverAInicio()
    {
        SceneManager.LoadScene("0InicioScene");
    }
    
    public void SalirJuegoFinal()
    {
        Application.Quit();
    }
}
