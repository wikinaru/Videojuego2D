using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caballero3Manager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;

    public float velocidadCaballero3 = 2f;
    
    private Animator caballero3_AnimController;
    private SpriteRenderer spriteRenderer;
    private bool mirandoDerecha = true;
    
    private bool debeMoverse = false;
    private Vector3 destinoMovimiento;
    private float velocidadMovimiento;
    
    // Variables para control de estados y audio
    private enum EstadoMovimiento { Idle, Persiguiendo, Atacando, VolviendoAInicio }
    private EstadoMovimiento estadoActual = EstadoMovimiento.Idle;
    private EstadoMovimiento estadoAnterior = EstadoMovimiento.Idle;
    
    // Variables para control de audio más específicas
    private bool deberiaReproducirAudioMovimiento = false;
    private bool audioMovimientoActivo = false;
    private float tiempoUltimoAudioMovimiento = 0f;
    private float intervalAudioMovimiento = 0.5f;

    void Start()
    {
        caballero3_AnimController = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
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
            transform.position = Vector3.MoveTowards(transform.position, destinoMovimiento, velocidadMovimiento);
            
            if (estadoActual == EstadoMovimiento.VolviendoAInicio && 
                Vector3.Distance(transform.position, posicionInical) < 0.1f)
            {
                CambiarEstado(EstadoMovimiento.Idle);
                debeMoverse = false;
            }
        }
    }

    void ProcesarEstadoIA(float distancia)
    {
        if (distancia <= 2f)
        {
            // ATACAR
            CambiarEstado(EstadoMovimiento.Atacando);
            
            caballero3_AnimController.SetBool("caballero3ActivarCaminar", false);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", true);
            
            ActualizarDireccion();
            debeMoverse = false;
            deberiaReproducirAudioMovimiento = false;
        }
        else if (distancia <= 4f)
        {
            // CAMINAR/PERSEGUIR
            CambiarEstado(EstadoMovimiento.Persiguiendo);
            
            ActualizarDireccion();

            caballero3_AnimController.SetBool("caballero3ActivarCaminar", true);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);
            
            destinoMovimiento = personaje.transform.position;
            velocidadMovimiento = velocidadCaballero3 * Time.fixedDeltaTime;
            debeMoverse = true;
            deberiaReproducirAudioMovimiento = true; 
        }
        else
        {
            // VOLVER A POSICIÓN INICIAL
            float distanciaAInicio = Vector3.Distance(transform.position, posicionInical);
            
            if (distanciaAInicio > 0.1f)
            {
                CambiarEstado(EstadoMovimiento.VolviendoAInicio);
                
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
                
                caballero3_AnimController.SetBool("caballero3ActivarCaminar", true);
                caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);
                
                destinoMovimiento = posicionInical;
                velocidadMovimiento = velocidadCaballero3 * Time.fixedDeltaTime;
                debeMoverse = true;
                deberiaReproducirAudioMovimiento = true;
            }
            else
            {
                //IDLE
                CambiarEstado(EstadoMovimiento.Idle);
                
                caballero3_AnimController.SetBool("caballero3ActivarCaminar", false);
                caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);
                
                debeMoverse = false;
                deberiaReproducirAudioMovimiento = false;
            }
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