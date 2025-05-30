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
        float velocidadFinal = velocidadCaballero3 * Time.deltaTime;

        if (distancia <= 2f)
        {
            // ATACAR
            caballero3_AnimController.SetBool("caballero3ActivarCaminar", false);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", true);
            
            ActualizarDireccion();
        }
        else if (distancia <= 4f)
        {
            // CAMINAR/PERSEGUIR
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            ActualizarDireccion();

            caballero3_AnimController.SetBool("caballero3ActivarCaminar", true);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);
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