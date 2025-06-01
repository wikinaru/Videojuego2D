using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Morir : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.DetenerMusica();
            Debug.Log("Música detenida en escena de muerte");
        }
        else
        {
            Debug.LogWarning("AudioManager no encontrado al intentar detener la música");
        }
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