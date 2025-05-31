using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorgon3Manager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;

    public float velocidadGorgon3 = 2f;
    public float distanciaDeteccion = 3f;
    public float distanciaAtaque = 2f;
    public float tiempoEntreAtaques = 2f;
    
    private Animator gorgon3_AnimController;
    private AtaqueGorgon3 scriptAtaque;
    private SpriteRenderer spriteRenderer;
    private bool mirandoDerecha = true;
    private float tiempoUltimoAtaque = 0f;
    
    // Variables para el sistema de movimiento
    private enum EstadoMovimiento { Idle, Persiguiendo, Atacando, VolviendoAInicio }
    private EstadoMovimiento estadoActual = EstadoMovimiento.Idle;
    private Vector3 objetivoMovimiento;
    private bool debeMoverse = false;

    void Start()
    {
        gorgon3_AnimController = GetComponent<Animator>();
        scriptAtaque = GetComponent<AtaqueGorgon3>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
        
        if (scriptAtaque == null)
        {
            Debug.LogWarning("AtaqueGorgon3 no encontrado en " + gameObject.name);
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
            
            ActualizarDireccion();
            debeMoverse = false;
            
            if (PuedeAtacar())
            {
                gorgon3_AnimController.SetBool("gorgon3ActivarAtacar", true);
                tiempoUltimoAtaque = Time.time;
            }
            else
            {
                if (scriptAtaque == null || !scriptAtaque.EstaAtacando())
                {
                    gorgon3_AnimController.SetBool("gorgon3ActivarAtacar", false);
                }
            }
        }
        else if (distancia <= distanciaDeteccion)
        {
            // ACERCARSE/PERSEGUIR
            CambiarEstado(EstadoMovimiento.Persiguiendo);
            
            objetivoMovimiento = personaje.transform.position;
            debeMoverse = true;

            ActualizarDireccion();

            gorgon3_AnimController.SetBool("gorgon3ActivarAtacar", false);
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
            
            gorgon3_AnimController.SetBool("gorgon3ActivarAtacar", false);
        }
    }

    void EjecutarMovimiento()
    {
        float velocidadFinal = velocidadGorgon3 * Time.fixedDeltaTime;
        
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
                }
                break;
        }
    }

    void CambiarEstado(EstadoMovimiento nuevoEstado)
    {
        if (estadoActual != nuevoEstado)
        {
            estadoActual = nuevoEstado;
        }
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