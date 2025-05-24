using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltoCofre : MonoBehaviour
{
    private GameObject _personaje;
    private MovPersonaje _movPersonaje;

    void Start()
    {
        _personaje = GameObject.Find("Personaje");
        _movPersonaje = _personaje.GetComponent<MovPersonaje>();
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.name == "Personaje")
        {
            _movPersonaje.ActivarDobleSalto();
        }
    }
}
