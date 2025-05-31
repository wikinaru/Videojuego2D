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
    
    // Variables para audio
    private bool estabaCaminando = false;

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

        if (distancia <= 2f)
        {
            // ATACAR
            caballero3_AnimController.SetBool("caballero3ActivarCaminar", false);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", true);
            
            ActualizarDireccion();
            
            debeMoverse = false;
            
            if (estabaCaminando)
            {
                estabaCaminando = false;
            }
        }
        else if (distancia <= 4f)
        {
            // CAMINAR/PERSEGUIR
            ActualizarDireccion();

            caballero3_AnimController.SetBool("caballero3ActivarCaminar", true);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);
            
            destinoMovimiento = personaje.transform.position;
            velocidadMovimiento = velocidadCaballero3 * Time.fixedDeltaTime;
            debeMoverse = true;
            
            if (!estabaCaminando)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ReproducirEfectoMovimientoCaballeros();
                }
                estabaCaminando = true;
            }
        }
        else
        {
            // VOLVER A POSICIÓN INICIAL
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
            
            if (!estabaCaminando)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ReproducirEfectoMovimientoCaballeros();
                }
                estabaCaminando = true;
            }
        }
        
        if (!debeMoverse && estabaCaminando)
        {
            estabaCaminando = false;
        }
    }

    void FixedUpdate()
    {
        if (debeMoverse)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinoMovimiento, velocidadMovimiento);
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