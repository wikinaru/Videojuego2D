using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovPersonaje : MonoBehaviour
{
    GameObject respawn;
    private Animator animatorController;
    private AtaquePersonaje ataqueScript;
    private Rigidbody2D rb;

    public float multiplicador = 4f;
    public float multiplicadorSalto = 4f;
    
    public bool MirandoDerecha { get; private set; } = true;

    private bool suelo;
    private int saltosTotales = 1;
    private int saltosRestantes;
    private bool puedeMoverse = true;
    private bool dobleSaltoActivado = false;

    void Awake()
    {
        respawn = GameObject.Find("Respawn");
        if (respawn == null)
        {
            Debug.LogError("No se ha encontrado el Respawn");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();
        ataqueScript = GetComponent<AtaquePersonaje>();
        
        Spawn_Inicial();
        saltosRestantes = saltosTotales;
    }

    void Update()
    {
        float moverse = Input.GetAxis("Horizontal");

        if (!puedeMoverse || GameManager.morir) return;

        if (animatorController.GetBool("atacando"))
        {
            rb.velocity = Vector2.zero;
        }

        // Moverse
        rb.velocity = new Vector2(moverse * multiplicador, rb.velocity.y);

        // Manejo de dirección centralizado
        if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D))
        {
            // No hacer nada si ambas teclas se presionan
        }
        else if (moverse < 0)
        {
            CambiarDireccion(false); // Mirando izquierda
        }
        else if (moverse > 0)
        {
            CambiarDireccion(true); // Mirando derecha
        }

        // Animación de correr
        animatorController.SetBool("activarCorrer", moverse != 0);

        // Saltar
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
        suelo = hit.collider != null;

        if (suelo)
        {
            saltosRestantes = saltosTotales;
            animatorController.SetBool("activarSalto", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0)
        {
            rb.AddForce(new Vector2(0, multiplicadorSalto), ForceMode2D.Impulse);
            saltosRestantes--;
            animatorController.SetBool("activarSalto", true);
        }

        // Caerse del mapa
        if (transform.position.y <= -10)
        {
            Respawnear();
        }

        // Morir
        if (GameManager.barraVida <= 0)
        {
            GameManager.morir = true;
        }
    }

    // Método centralizado para cambiar dirección
    private void CambiarDireccion(bool derecha)
    {
        if (MirandoDerecha == derecha) return;

        MirandoDerecha = derecha;
        GetComponent<SpriteRenderer>().flipX = !derecha;
        
        if (ataqueScript != null)
        {
            ataqueScript.ActualizarDireccionExterna(derecha);
        }
    }

    public void ActivarDobleSalto()
    {
        dobleSaltoActivado = true;
        saltosTotales = 2;

        if (suelo)
        {
            saltosRestantes = 2;
        }

        Debug.Log("Doble Salto Activado!");
    }

    public void Spawn_Inicial()
    {
        transform.position = respawn.transform.position;
    }

    public void Movimiento(bool activar)
    {
        puedeMoverse = activar;
        if (!activar) 
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Respawnear()
    {
        Debug.Log("Vida antes: " + GameManager.barraVida);
        GameManager.barraVida = GameManager.barraVida - 2;
        Debug.Log("Vida después: " + GameManager.barraVida);
        transform.position = respawn.transform.position;
    }
}