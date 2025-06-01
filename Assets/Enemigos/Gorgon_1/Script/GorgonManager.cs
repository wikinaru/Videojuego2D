using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorgonManager : MonoBehaviour
{
    public float velocidadGorgon1 = 2f;
    public float distanciaAtaque = 2f;
    public float distanciaPerseguir = 3.5f;
    
    // Variables privadas
    private Vector3 posicionInical;
    private GameObject personaje;
    private Animator gorgon1_AnimController;
    
    // Variables para el sprite flip - AÑADIDAS
    private SpriteRenderer spriteRenderer;
    private bool mirandoDerecha = false; // Inicia mirando a la izquierda
    
    // Variables para FixedUpdate
    private float distanciaActual;
    private bool deberiaMoverse;
    private bool deberiaAtacar;
    
    // Variables para control de estados y audio
    private enum EstadoMovimiento { Idle, Persiguiendo, Atacando }
    private EstadoMovimiento estadoActual = EstadoMovimiento.Idle;
    private EstadoMovimiento estadoAnterior = EstadoMovimiento.Idle;
    private bool audioMovimientoReproduciendose = false;

    void Start()
    {
        gorgon1_AnimController = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
        
        if (personaje == null)
        {
            Debug.LogError("No se encontró el Player. Asegúrate de que tenga el tag 'Player'");
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError("No se encontró SpriteRenderer en " + gameObject.name);
        }
    }

    void Update()
    {
        if (personaje == null) return;
        
        distanciaActual = Vector3.Distance(transform.position, personaje.transform.position);
        ProcesarEstadoIA();
    }

    void FixedUpdate()
    {
        if (personaje == null) return;
        
        if (deberiaMoverse)
        {
            float velocidadFinal = velocidadGorgon1 * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);
        }
    }

    void ProcesarEstadoIA()
    {
        if (distanciaActual <= distanciaAtaque)
        {
            // ATACAR
            CambiarEstado(EstadoMovimiento.Atacando);
            
            deberiaMoverse = false;
            deberiaAtacar = true;
            
            ActualizarDireccion();
            
            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", false);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", true);
        }
        else if (distanciaActual <= distanciaPerseguir)
        {
            // CAMINAR/PERSEGUIR
            CambiarEstado(EstadoMovimiento.Persiguiendo);
            
            deberiaMoverse = true;
            deberiaAtacar = false;
            
            ActualizarDireccion();
            
            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", true);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);
        }
        else
        {
            // IDLE/VOLVER
            CambiarEstado(EstadoMovimiento.Idle);
            
            deberiaMoverse = false;
            deberiaAtacar = false;
            
            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", false);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);
        }
    }

    void ActualizarDireccion()
    {
        if (personaje == null) return;
        
        if (personaje.transform.position.x > transform.position.x)
        {
            mirandoDerecha = true;
        }
        else if (personaje.transform.position.x < transform.position.x)
        {
            mirandoDerecha = false;
        }
        
        ActualizarFlip();
    }

    void ActualizarFlip()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !mirandoDerecha;
        }
    }

    void CambiarEstado(EstadoMovimiento nuevoEstado)
    {
        if (estadoActual != nuevoEstado)
        {
            estadoAnterior = estadoActual;
            estadoActual = nuevoEstado;
            
            ManejarAudioPorEstado();
        }
    }

    void ManejarAudioPorEstado()
    {
        if (estadoActual == EstadoMovimiento.Persiguiendo && !audioMovimientoReproduciendose)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ReproducirEfectoMovimientoGorgons();
            }
            audioMovimientoReproduciendose = true;
            
            StartCoroutine(DetenerAudioMovimientoDespuesDeTiempo());
        }
        else if ((estadoActual == EstadoMovimiento.Idle || estadoActual == EstadoMovimiento.Atacando) 
                 && audioMovimientoReproduciendose)
        {
            audioMovimientoReproduciendose = false;
        }
    }
    
    private IEnumerator DetenerAudioMovimientoDespuesDeTiempo()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (estadoActual == EstadoMovimiento.Persiguiendo)
        {
            audioMovimientoReproduciendose = false;
        }
    }
}