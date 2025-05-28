using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ataqueScript : MonoBehaviour
{
    //Variables de daño
    public float damage = 2.5f;
    public LayerMask capasEnemigos = 1 << 6;

    private bool mirandoDerecha = true;
    private HashSet<GameObject> enemigosGolpeados;

    void Start()
    {
        enemigosGolpeados = new HashSet<GameObject>();

        Destroy(gameObject, 2f);
    }

    void Update()
    {
        ActualizarOrientacion();
    }

    void ActualizarOrientacion()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !mirandoDerecha;
        }
    }

    public void ConfigurarDireccion(bool direccionDerecha)
    {
        mirandoDerecha = direccionDerecha;
        ActualizarOrientacion();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo") && !enemigosGolpeados.Contains(collision.gameObject))
        {
            ProcesarImpacto(collision.gameObject);
        }
    }

    void ProcesarImpacto(GameObject enemigo)
    {
        enemigosGolpeados.Add(enemigo);

        AplicarDamage(enemigo);
    }

    void AplicarDamage(GameObject enemigo)
    {
        bool enemigoDerrotado = GameManager.DanarEnemigo(enemigo, damage);

        if (!enemigoDerrotado && string.IsNullOrEmpty(ObtenerTipoEnemigo(enemigo)))
        {
            Destroy(enemigo);
        }

        AplicarKnockBack(enemigo);
    }

    string ObtenerTipoEnemigo(GameObject enemigo)
    {
        if (enemigo.name.Contains("Gorgon"))
        {
            if (enemigo.name.Contains("1")) return "Gorgon1";
            if (enemigo.name.Contains("2")) return "Gorgon2";
            if (enemigo.name.Contains("3")) return "Gorgon3";
        }
        else if (enemigo.name.Contains("Caballero"))
        {
            if (enemigo.name.Contains("2")) return "Caballero2";
            if (enemigo.name.Contains("3")) return "Caballero3";
        }
        
        return "";
    }

    void AplicarKnockBack(GameObject enemigo)
    {
        Rigidbody2D rbEnemigo = enemigo.GetComponent<Rigidbody2D>();
        if (rbEnemigo != null)
        {
            Vector2 direccionKnockback = mirandoDerecha ? Vector2.right : Vector2.left;
            float fuerzaKnockback = 5f;
            rbEnemigo.AddForce(direccionKnockback * fuerzaKnockback, ForceMode2D.Impulse);
        }
    }

    public void ConfigurarAtaque(int dano, bool direccion)
    {
        damage = dano;
        mirandoDerecha = direccion;
        ActualizarOrientacion();
    }
}
