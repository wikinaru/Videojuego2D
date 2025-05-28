using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltoCofre : MonoBehaviour
{
    private GameObject _personaje;
    private MovPersonaje _movPersonaje;
    // Start is called before the first frame update
    void Start()
    {
        _personaje = GameObject.Find("Personaje");
        _movPersonaje = _personaje.GetComponent<MovPersonaje>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Personaje")
        {
            _movPersonaje.ActivarDobleSalto();
            gameObject.SetActive(false);
        }
    }
}
