using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private float tiempoUltimoSonidoMovimiento = 0f;
    public float intervaloSonidoMovimiento = 0.3f;
    private float tiempoUltimoSalto = 0f;
    public float tiempoSilencioTrasSalto = 0.2f;
    private float tiempoUltimoAterrizar = 0f;
    public float tiempoSilencioTrasAterrizar = 0.1f;

    private bool estaMuriendo = false;
    public float tiempoEsperaTrasMuerte = 2f;

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
        
        tiempoUltimoAterrizar = Time.time - tiempoSilencioTrasAterrizar - 1f;
        
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager no encontrado al inicializar MovPersonaje");
        }
        else
        {
            Debug.Log("AudioManager encontrado correctamente");
        }
    }

    void Update()
    {
        CapturarInputs();
        
        if (estaMuriendo) return;
        
        if (!puedeMoverse || GameManager.morir) 
        {
            if (GameManager.morir && !estaMuriendo)
            {
                IniciarSecuenciaMuerte();
            }
            return;
        }

        ManejarDireccion();
        ManejarAnimaciones();
        ManejarSonidoMovimiento();
        
        DetectarSuelo();
        
        if (transform.position.y <= -10)
        {
            Respawnear();
        }
    }

    void FixedUpdate()
    {
        if (!puedeMoverse || GameManager.morir || estaMuriendo) return;

        if (animatorController.GetBool("atacando"))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        AplicarMovimientoHorizontal();
        AplicarSalto();
    }

    private void IniciarSecuenciaMuerte()
    {
        estaMuriendo = true;
        puedeMoverse = false;
        
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        if (animatorController != null)
        {
            animatorController.SetTrigger("morir");
        }
        
        Debug.Log("Iniciando secuencia de muerte...");
        
        StartCoroutine(CambiarEscenaTrasMuerte());
    }

    private IEnumerator CambiarEscenaTrasMuerte()
    {
        yield return new WaitForSeconds(tiempoEsperaTrasMuerte);
        
        Debug.Log("Cambiando a escena de muerte...");
        SceneManager.LoadScene("3MuerteScene");
    }

    public void CambiarAEscenaMuerte()
    {
        Debug.Log("Cambiando a escena de muerte desde evento del Animator...");
        SceneManager.LoadScene("3MuerteScene");
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

    private void ManejarSonidoMovimiento()
    {
        bool estaMoviendose = Mathf.Abs(moverse) > 0.1f;
        bool estaEnSuelo = suelo;
        bool noEstaAtacando = !animatorController.GetBool("atacando");
        
        bool haPasadoTiempoSuficienteTrasSalto = (Time.time - tiempoUltimoSalto) >= tiempoSilencioTrasSalto;
        bool haPasadoTiempoSuficienteTrasAterrizar = (Time.time - tiempoUltimoAterrizar) >= tiempoSilencioTrasAterrizar;
        bool haPasadoTiempoEntreSONIDOS = (Time.time - tiempoUltimoSonidoMovimiento) >= intervaloSonidoMovimiento;
        
        bool debeReproducirSonido = estaMoviendose && 
                                   estaEnSuelo && 
                                   noEstaAtacando && 
                                   haPasadoTiempoSuficienteTrasSalto && 
                                   haPasadoTiempoSuficienteTrasAterrizar &&
                                   haPasadoTiempoEntreSONIDOS;
        
        if (estaMoviendose && estaEnSuelo && noEstaAtacando)
        {
            if (!haPasadoTiempoEntreSONIDOS)
            {
                Debug.Log($"Esperando intervalo de sonido. Tiempo desde último: {Time.time - tiempoUltimoSonidoMovimiento:F2}s");
            }
        }
        
        if (debeReproducirSonido)
        {
            ReproducirSonidoMovimiento();
            tiempoUltimoSonidoMovimiento = Time.time;
        }
    }

    private void ReproducirSonidoMovimiento()
    {
        Debug.Log("Intentando reproducir sonido de movimiento...");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirEfectoMovimientoJugador();
            Debug.Log("Sonido de movimiento reproducido");
        }
        else
        {
            Debug.LogError("AudioManager.Instance es null! No se puede reproducir el sonido de movimiento.");
            
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                Debug.Log("AudioManager encontrado mediante FindObjectOfType, pero Instance es null");
            }
            else
            {
                Debug.LogError("No se encontró ningún AudioManager en la escena");
            }
        }
    }

    private void ReproducirSonidoSalto()
    {
        Debug.Log("Intentando reproducir sonido de salto...");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirEfectoSaltoJugador();
            Debug.Log("Sonido de salto reproducido");
        }
        else
        {
            Debug.LogError("AudioManager no encontrado. No se puede reproducir el sonido de salto.");
        }
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
            
            tiempoUltimoAterrizar = Time.time;
            
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
            
            tiempoUltimoSalto = Time.time;
            ReproducirSonidoSalto();
            
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
        
        tiempoUltimoAterrizar = Time.time - tiempoSilencioTrasAterrizar - 1f;
        tiempoUltimoSalto = Time.time - tiempoSilencioTrasSalto - 1f;
        tiempoUltimoSonidoMovimiento = 0f;
        
        estaMuriendo = false;
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
        GameManager.DamageDeJugador(2f);
        Debug.Log("Vida después: " + GameManager.barraVida);
        transform.position = respawn.transform.position;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        tiempoUltimoAterrizar = Time.time;
        tiempoUltimoSalto = Time.time;
        tiempoUltimoSonidoMovimiento = 0f;
        
        estaMuriendo = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = suelo ? Color.green : Color.red;
        Vector3 pos = transform.position;
        Vector3 tamaño = new Vector3(anchoDeteccionSuelo, distanciaDeteccionSuelo, 0.1f);
        Gizmos.DrawWireCube(pos + Vector3.down * distanciaDeteccionSuelo, tamaño);
    }

    public void TestAudioMovimiento()
    {
        ReproducirSonidoMovimiento();
    }

    public void TestAudioSalto()
    {
        ReproducirSonidoSalto();
    }
}