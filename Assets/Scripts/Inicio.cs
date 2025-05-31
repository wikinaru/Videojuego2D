using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inicio : MonoBehaviour
{
    GameObject ajustes;

    void Start()
    {
        ajustes = GameObject.Find("PanelAjustes");
        ajustes.SetActive(false);
    }

    void Update()
    {

    }

    public void EmpezarJuego()
    {
        SceneManager.LoadScene("1JuegoScene");
    }

    public void AbrirAjustes()
    {
        ajustes.SetActive(true);
    }

    public void SalirAjustes()
    {
        ajustes.SetActive(false);
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}
