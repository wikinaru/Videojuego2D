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
    private Vector3 objetivoMovimiento;
    private bool debeMoverse = false;
    
    // Variables para audio
    private bool estabaCaminando = false;
    
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
        if (distancia <= distanciaAtaque)
        {
            // ATACAR
            CambiarEstado(EstadoMovimiento.Atacando);
            
            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", false);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", true);
            
            ActualizarDireccion();
            debeMoverse = false;
            
            if (estabaCaminando)
            {
                estabaCaminando = false;
            }
        }
        else if (distancia <= distanciaDeteccion)
        {
            // ACERCARSE/PERSEGUIR
            CambiarEstado(EstadoMovimiento.Persiguiendo);
            
            objetivoMovimiento = personaje.transform.position;
            debeMoverse = true;

            ActualizarDireccion();

            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", true);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", false);
            
            if (!estabaCaminando)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ReproducirEfectoMovimientoGorgons();
                }
                estabaCaminando = true;
            }
        }
        else
        {
            // VOLVER A POSICIÓN INICIAL
            CambiarEstado(EstadoMovimiento.VolviendoAInicio);
            
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
            
            if (!estabaCaminando)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ReproducirEfectoMovimientoGorgons();
                }
                estabaCaminando = true;
            }
        }
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
                
                if (Vector3.Distance(transform.position, posicionInical) < 0.1f)
                {
                    debeMoverse = false;
                    CambiarEstado(EstadoMovimiento.Idle);
                    
                    if (estabaCaminando)
                    {
                        estabaCaminando = false;
                    }
                }
                break;
        }
    }

    void CambiarEstado(EstadoMovimiento nuevoEstado)
    {
        if (estadoActual != nuevoEstado)
        {
            if ((estadoActual == EstadoMovimiento.Persiguiendo || estadoActual == EstadoMovimiento.VolviendoAInicio) 
                && (nuevoEstado == EstadoMovimiento.Idle || nuevoEstado == EstadoMovimiento.Atacando))
            {
                if (estabaCaminando)
                {
                    estabaCaminando = false;
                }
            }
            
            estadoActual = nuevoEstado;
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