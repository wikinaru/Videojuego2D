using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAtaqueGorgon : MonoBehaviour
{
    public float damage = 1f;
    
    private bool mirandoDerecha = true;
    private HashSet<GameObject> jugadoresGolpeados;
    private GameObject enemigoCreador;

    void Start()
    {
        jugadoresGolpeados = new HashSet<GameObject>();
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
        Debug.Log("Hitbox detectó colisión con: " + collision.name + " Tag: " + collision.tag);
        
        if (collision.CompareTag("Player") && 
            !jugadoresGolpeados.Contains(collision.gameObject) &&
            collision.gameObject != enemigoCreador)
        {
            Debug.Log("¡Procesando impacto en jugador!");
            ProcesarImpacto(collision.gameObject);
        }
        else
        {
            Debug.Log("No es jugador válido. Tag: " + collision.tag + " Ya golpeado: " + jugadoresGolpeados.Contains(collision.gameObject));
        }
    }

    void ProcesarImpacto(GameObject jugador)
    {
        jugadoresGolpeados.Add(jugador);
        Debug.Log("Aplicando " + damage + " de daño al jugador: " + jugador.name);
        AplicarDamage(jugador);
    }

    void AplicarDamage(GameObject jugador)
    {
        GameManager.DanarJugador(jugador, damage);
        AplicarKnockBack(jugador);
    }

    void AplicarKnockBack(GameObject jugador)
    {
        Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
        if (rbJugador != null)
        {
            Vector2 direccionKnockback = mirandoDerecha ? Vector2.right : Vector2.left;
            float fuerzaKnockback = 3f;
            rbJugador.AddForce(direccionKnockback * fuerzaKnockback, ForceMode2D.Impulse);
            Debug.Log("Aplicando knockback hacia: " + direccionKnockback);
        }
        else
        {
            Debug.LogWarning("Jugador no tiene Rigidbody2D para knockback");
        }
    }

    public void ConfigurarAtaque(float dano, bool direccion)
    {
        damage = dano;
        mirandoDerecha = direccion;
        ActualizarOrientacion();
        Debug.Log("Hitbox configurada - Daño: " + dano + " Dirección derecha: " + direccion);
    }

    public void ConfigurarAtaque(float dano, bool direccion, GameObject creador)
    {
        damage = dano;
        mirandoDerecha = direccion;
        enemigoCreador = creador;
        ActualizarOrientacion();
        Debug.Log("Hitbox configurada - Daño: " + dano + " Dirección derecha: " + direccion + " Creador: " + creador.name);
    }
}