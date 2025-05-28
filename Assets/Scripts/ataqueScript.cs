using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ataqueScript : MonoBehaviour
{
    bool derecha = true;

    void Start()
    {
        derecha = MovPersonaje.direccion;
    }

    void Update()
    {
        if (derecha)
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemigo")
        {
            Destroy(collision.gameObject);
        }
    }
}
