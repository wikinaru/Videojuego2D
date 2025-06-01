using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorgon2Manager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;

    public float velocidadGorgon2 = 2f;
    public float distanciaDeteccion = 2.5f;
    public float distanciaAtaque = 1.5f;
    
    private Animator gorgon2_AnimController;
    private AtaqueGorgon2 scriptAtaque;
    private SpriteRenderer spriteRenderer;
    private bool mirandoDerecha = true;
    
    // Variables para el sistema de movimiento
    private enum EstadoMovimiento { Idle, Persiguiendo, Atacando, VolviendoAInicio }
    private EstadoMovimiento estadoActual = EstadoMovimiento.Idle;
    private EstadoMovimiento estadoAnterior = EstadoMovimiento.Idle;
    private Vector3 objetivoMovimiento;
    private bool debeMoverse = false;
    
    // Variables para audio
    private bool sonidoMovimientoReproducido = false;
    private float tiempoUltimoSonidoMovimiento = 0f;
    private float intervalSonidoMovimiento = 1f;
    
    void Start()
    {
        gorgon2_AnimController = GetComponent<Animator>();
        scriptAtaque = GetComponent<AtaqueGorgon2>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
        
        if (scriptAtaque == null)
        {
            Debug.LogWarning("AtaqueGorgon2 no encontrado en " + gameObject.name);
        }
    }

    void Update()
    {
        if (personaje == null) return;

        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        ProcesarEstadoIA(distancia);
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
        EstadoMovimiento nuevoEstado;
        
        if (distancia <= distanciaAtaque)
        {
            // ATACAR
            nuevoEstado = EstadoMovimiento.Atacando;
            
            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", false);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", true);
            
            ActualizarDireccion();
            debeMoverse = false;
        }
        else if (distancia <= distanciaDeteccion)
        {
            // ACERCARSE/PERSEGUIR
            nuevoEstado = EstadoMovimiento.Persiguiendo;
            
            objetivoMovimiento = personaje.transform.position;
            debeMoverse = true;

            ActualizarDireccion();

            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", true);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", false);
        }
        else
        {
            // VOLVER A POSICIÓN INICIAL
            float distanciaAInicio = Vector3.Distance(transform.position, posicionInical);
            
            if (distanciaAInicio > 0.1f)
            {
                nuevoEstado = EstadoMovimiento.VolviendoAInicio;
                
                objetivoMovimiento = posicionInical;
                debeMoverse = true;
                
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
                
                gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", true);
                gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", false);
            }
            else
            {
                nuevoEstado = EstadoMovimiento.Idle;
                debeMoverse = false;
                
                gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", false);
                gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", false);
            }
        }
        
        CambiarEstado(nuevoEstado);
    }

    void EjecutarMovimiento()
    {
        float velocidadFinal = velocidadGorgon2 * Time.fixedDeltaTime;
        
        switch (estadoActual)
        {
            case EstadoMovimiento.Persiguiendo:
                transform.position = Vector3.MoveTowards(transform.position, objetivoMovimiento, velocidadFinal);
                break;
                
            case EstadoMovimiento.VolviendoAInicio:
                transform.position = Vector3.MoveTowards(transform.position, objetivoMovimiento, velocidadFinal);
                break;
        }
    }

    void CambiarEstado(EstadoMovimiento nuevoEstado)
    {
        if (estadoActual != nuevoEstado)
        {
            if (EsEstadoMovimiento(estadoActual) && !EsEstadoMovimiento(nuevoEstado))
            {
                sonidoMovimientoReproducido = false;
                Debug.Log($"[GORGON2] Dejando de caminar - Estado: {estadoActual} -> {nuevoEstado}");
            }
            
            estadoAnterior = estadoActual;
            estadoActual = nuevoEstado;
            
            if (!EsEstadoMovimiento(estadoAnterior) && EsEstadoMovimiento(estadoActual))
            {
                ReproducirSonidoMovimiento();
                Debug.Log($"[GORGON2] Empezando a caminar - Estado: {estadoAnterior} -> {estadoActual}");
            }
        }
    }
    
    bool EsEstadoMovimiento(EstadoMovimiento estado)
    {
        return estado == EstadoMovimiento.Persiguiendo || estado == EstadoMovimiento.VolviendoAInicio;
    }
    
    void ReproducirSonidoMovimiento()
    {
        if (!sonidoMovimientoReproducido || Time.time - tiempoUltimoSonidoMovimiento >= intervalSonidoMovimiento)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ReproducirEfectoMovimientoGorgons();
                Debug.Log($"[GORGON2] Sonido movimiento reproducido - Tiempo: {Time.time:F2}");
            }
            
            sonidoMovimientoReproducido = true;
            tiempoUltimoSonidoMovimiento = Time.time;
        }
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