using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caballero2Manager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;

    public float velocidadCaballero2 = 2f;
    public float distanciaDeteccion = 3f;
    public float distanciaAtaque = 2f;
    public float tiempoEntreAtaques = 1.8f;
    
    private Animator caballero2_AnimController;
    private AtaqueCaballero2 scriptAtaque;
    private SpriteRenderer spriteRenderer;
    private bool mirandoDerecha = true;
    private float tiempoUltimoAtaque = 0f;
    
    // Variables para el sistema de movimiento
    private enum EstadoMovimiento { Idle, Persiguiendo, Atacando, VolviendoAInicio }
    private EstadoMovimiento estadoActual = EstadoMovimiento.Idle;
    private EstadoMovimiento estadoAnterior = EstadoMovimiento.Idle;
    private Vector3 objetivoMovimiento;
    private bool debeMoverse = false;
    
    // Variables para control de audio más específicas
    private bool deberiaReproducirAudioMovimiento = false;
    private bool audioMovimientoActivo = false;
    private float tiempoUltimoAudioMovimiento = 0f;
    private float intervalAudioMovimiento = 0.5f;

    void Start()
    {
        caballero2_AnimController = GetComponent<Animator>();
        scriptAtaque = GetComponent<AtaqueCaballero2>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
        
        if (scriptAtaque == null)
        {
            Debug.LogWarning("AtaqueCaballero2 no encontrado en " + gameObject.name);
        }
    }

    void Update()
    {
        if (personaje == null) return;

        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        ProcesarEstadoIA(distancia);
        ManejarAudioMovimiento();
    }

    void FixedUpdate()
    {
        if (debeMoverse)
        {
            EjecutarMovimiento();
        }
    }

    void ProcesarEstadoIA(float distancia)
    {
        if (distancia <= distanciaAtaque)
        {
            // ATACAR
            CambiarEstado(EstadoMovimiento.Atacando);
            
            ActualizarDireccion();
            debeMoverse = false;
            deberiaReproducirAudioMovimiento = false;
            
            if (PuedeAtacar())
            {
                caballero2_AnimController.SetBool("caballero2ActivarAtacar", true);
                tiempoUltimoAtaque = Time.time;
            }
            else
            {
                if (scriptAtaque == null || !scriptAtaque.EstaAtacando())
                {
                    caballero2_AnimController.SetBool("caballero2ActivarAtacar", false);
                }
            }
        }
        else if (distancia <= distanciaDeteccion)
        {
            // ACERCARSE/PERSEGUIR
            CambiarEstado(EstadoMovimiento.Persiguiendo);
            
            objetivoMovimiento = personaje.transform.position;
            debeMoverse = true;
            deberiaReproducirAudioMovimiento = true;

            ActualizarDireccion();
            caballero2_AnimController.SetBool("caballero2ActivarAtacar", false);
        }
        else
        {
            // VOLVER A POSICIÓN INICIAL
            float distanciaAInicio = Vector3.Distance(transform.position, posicionInical);
            
            if (distanciaAInicio > 0.1f)
            {
                CambiarEstado(EstadoMovimiento.VolviendoAInicio);
                
                objetivoMovimiento = posicionInical;
                debeMoverse = true;
                deberiaReproducirAudioMovimiento = true;
                
                Vector3 direccionAInicial = (posicionInical - transform.position).normalized;
                if (direccionAInicial.x > 0.1f)
                {
                    mirandoDerecha = false;
                }
                else if (direccionAInicial.x < -0.1f)
                {
                    mirandoDerecha = true;
                }
                
                ActualizarFlip();
                caballero2_AnimController.SetBool("caballero2ActivarAtacar", false);
            }
            else
            {
                //IDLE
                CambiarEstado(EstadoMovimiento.Idle);
                
                debeMoverse = false;
                deberiaReproducirAudioMovimiento = false;
                caballero2_AnimController.SetBool("caballero2ActivarAtacar", false);
            }
        }
    }

    void EjecutarMovimiento()
    {
        float velocidadFinal = velocidadCaballero2 * Time.fixedDeltaTime;
        
        switch (estadoActual)
        {
            case EstadoMovimiento.Persiguiendo:
                transform.position = Vector3.MoveTowards(transform.position, objetivoMovimiento, velocidadFinal);
                break;
                
            case EstadoMovimiento.VolviendoAInicio:
                transform.position = Vector3.MoveTowards(transform.position, objetivoMovimiento, velocidadFinal);
                
                if (Vector3.Distance(transform.position, posicionInical) < 0.1f)
                {
                    debeMoverse = false;
                    deberiaReproducirAudioMovimiento = false;
                    CambiarEstado(EstadoMovimiento.Idle);
                }
                break;
        }
    }

    void CambiarEstado(EstadoMovimiento nuevoEstado)
    {
        if (estadoActual != nuevoEstado)
        {
            estadoAnterior = estadoActual;
            estadoActual = nuevoEstado;
        }
    }

    void ManejarAudioMovimiento()
    {
        if (deberiaReproducirAudioMovimiento && !audioMovimientoActivo && 
            Time.time - tiempoUltimoAudioMovimiento >= intervalAudioMovimiento)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ReproducirEfectoMovimientoCaballeros();
                audioMovimientoActivo = true;
                tiempoUltimoAudioMovimiento = Time.time;
                
                StartCoroutine(DesactivarAudioMovimientoFlag());
            }
        }
        
        if (!deberiaReproducirAudioMovimiento)
        {
            audioMovimientoActivo = false;
        }
    }

    IEnumerator DesactivarAudioMovimientoFlag()
    {
        yield return new WaitForSeconds(0.1f);
        audioMovimientoActivo = false;
    }

    bool PuedeAtacar()
    {
        bool tiempoSuficiente = Time.time - tiempoUltimoAtaque >= tiempoEntreAtaques;
        bool noEstaAtacando = scriptAtaque == null || !scriptAtaque.EstaAtacando();
        
        return tiempoSuficiente && noEstaAtacando;
    }

    void ActualizarDireccion()
    {
        if (personaje.transform.position.x > transform.position.x)
        {
            mirandoDerecha = false;
        }
        else if (personaje.transform.position.x < transform.position.x)
        {
            mirandoDerecha = true;
        }
        
        ActualizarFlip();
    }

    void ActualizarFlip()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = mirandoDerecha;
        }
    }
}