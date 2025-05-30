using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxAtaqueCaballero3 : MonoBehaviour
{
    public float damage = 1f;
    public LayerMask capasJugador = 1 << 7;
    
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
        if (collision.CompareTag("Player") && 
            !jugadoresGolpeados.Contains(collision.gameObject) &&
            collision.gameObject != enemigoCreador)
        {
            ProcesarImpacto(collision.gameObject);
        }
    }

    void ProcesarImpacto(GameObject jugador)
    {
        jugadoresGolpeados.Add(jugador);
        
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
        }
    }

    public void ConfigurarAtaque(float dano, bool direccion)
    {
        damage = dano;
        mirandoDerecha = direccion;
        ActualizarOrientacion();
    }

    public void ConfigurarAtaque(float dano, bool direccion, GameObject creador)
    {
        damage = dano;
        mirandoDerecha = direccion;
        enemigoCreador = creador;
        ActualizarOrientacion();
    }
}