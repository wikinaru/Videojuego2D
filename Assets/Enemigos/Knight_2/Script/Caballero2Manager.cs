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
        float velocidadFinal = velocidadCaballero2 * Time.deltaTime;

        if (distancia <= distanciaAtaque)
        {
            // ATACAR
            ActualizarDireccion();
            
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
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            ActualizarDireccion();

            caballero2_AnimController.SetBool("caballero2ActivarAtacar", false);
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
            
            caballero2_AnimController.SetBool("caballero2ActivarAtacar", false);
            transform.position = Vector3.MoveTowards(transform.position, posicionInical, velocidadFinal);
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