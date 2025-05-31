using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inicio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EmpezarJuego()
    {
        SceneManager.LoadScene("1JuegoScene");
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}
