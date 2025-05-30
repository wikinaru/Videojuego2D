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
        float velocidadFinal = velocidadGorgon3 * Time.deltaTime;

        if (distancia <= distanciaAtaque)
        {
            // ATACAR
            ActualizarDireccion();
            
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
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            ActualizarDireccion();

            gorgon3_AnimController.SetBool("gorgon3ActivarAtacar", false);
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
            
            gorgon3_AnimController.SetBool("gorgon3ActivarAtacar", false);
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