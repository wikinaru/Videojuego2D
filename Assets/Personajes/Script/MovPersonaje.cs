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
    
    public LayerMask capaSuelo = -1;
    public float distanciaDeteccionSuelo = 0.1f;
    public float anchoDeteccionSuelo = 0.8f;
    
    private float moverse;
    private bool inputSalto;
    private bool inputSaltoBuffer;
    private float tiempoBufferSalto = 0.1f;
    private float timerBufferSalto;
    
    public bool MirandoDerecha { get; private set; } = true;

    private bool suelo;
    private bool sueloAnterior;
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
        CapturarInputs();
        
        if (!puedeMoverse || GameManager.morir) return;

        ManejarDireccion();
        ManejarAnimaciones();
        
        DetectarSuelo();
        
        if (transform.position.y <= -10)
        {
            Respawnear();
        }

        if (GameManager.barraVida <= 0)
        {
            GameManager.morir = true;
        }
    }

    void FixedUpdate()
    {
        if (!puedeMoverse || GameManager.morir) return;

        if (animatorController.GetBool("atacando"))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        AplicarMovimientoHorizontal();
        AplicarSalto();
    }

    private void CapturarInputs()
    {
        moverse = Input.GetAxis("Horizontal");
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputSalto = true;
            inputSaltoBuffer = true;
            timerBufferSalto = tiempoBufferSalto;
        }
        
        if (inputSaltoBuffer)
        {
            timerBufferSalto -= Time.deltaTime;
            if (timerBufferSalto <= 0)
            {
                inputSaltoBuffer = false;
            }
        }
    }

    private void ManejarDireccion()
    {
        if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D))
        {
            // No hacer nada si ambas teclas se presionan
        }
        else if (moverse < 0)
        {
            CambiarDireccion(false); //izquierda
        }
        else if (moverse > 0)
        {
            CambiarDireccion(true); //derecha
        }
    }

    private void ManejarAnimaciones()
    {
        animatorController.SetBool("activarCorrer", moverse != 0);
    }

    private void DetectarSuelo()
    {
        sueloAnterior = suelo;
        
        Vector2 posicion = transform.position;
        Vector2 tamaño = new Vector2(anchoDeteccionSuelo, distanciaDeteccionSuelo);
        
        RaycastHit2D hit = Physics2D.BoxCast(
            posicion, 
            tamaño, 
            0f, 
            Vector2.down, 
            distanciaDeteccionSuelo, 
            capaSuelo
        );
        
        suelo = hit.collider != null && hit.collider.gameObject != gameObject;

        if (suelo && !sueloAnterior)
        {
            saltosRestantes = saltosTotales;
            animatorController.SetBool("activarSalto", false);
            Debug.Log("Aterrizó - Saltos restaurados: " + saltosRestantes);
        }
    }

    private void AplicarMovimientoHorizontal()
    {
        rb.velocity = new Vector2(moverse * multiplicador, rb.velocity.y);
    }

    private void AplicarSalto()
    {
        bool deberíaSaltar = (inputSalto || inputSaltoBuffer) && saltosRestantes > 0;
        
        if (deberíaSaltar)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, multiplicadorSalto), ForceMode2D.Impulse);
            
            saltosRestantes--;
            animatorController.SetBool("activarSalto", true);
            
            inputSalto = false;
            inputSaltoBuffer = false;
            timerBufferSalto = 0;
            
            Debug.Log($"¡Saltando! Saltos restantes: {saltosRestantes}");
        }
        else
        {
            inputSalto = false;
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

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    // Método para debug visual en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = suelo ? Color.green : Color.red;
        Vector3 pos = transform.position;
        Vector3 tamaño = new Vector3(anchoDeteccionSuelo, distanciaDeteccionSuelo, 0.1f);
        Gizmos.DrawWireCube(pos + Vector3.down * distanciaDeteccionSuelo, tamaño);
    }
}