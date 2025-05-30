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
        float velocidadFinal = velocidadGorgon2 * Time.deltaTime;

        if (distancia <= distanciaAtaque)
        {
            // ATACAR
            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", false);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", true);
            
            ActualizarDireccion();
        }
        else if (distancia <= distanciaDeteccion)
        {
            // ACERCARSE/PERSEGUIR
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            ActualizarDireccion();

            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", true);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", false);
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
            
            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", true);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", false);
            transform.position = Vector3.MoveTowards(transform.position, posicionInical, velocidadFinal);
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