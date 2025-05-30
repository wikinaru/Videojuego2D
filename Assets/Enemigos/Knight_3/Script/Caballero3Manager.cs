using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caballero3Manager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;

    public float velocidadCaballero3 = 2f;
    public float tiempoEntreAtaques = 1.5f;
    
    private Animator caballero3_AnimController;
    private AtaqueCaballero scriptAtaque;
    private float tiempoUltimoAtaque = 0f;
    private SpriteRenderer spriteRenderer;
    private bool mirandoDerecha = true;

    void Start()
    {
        caballero3_AnimController = GetComponent<Animator>();
        scriptAtaque = GetComponent<AtaqueCaballero>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
        
        if (scriptAtaque == null)
        {
            Debug.LogWarning("AtaqueCaballero3 no encontrado en " + gameObject.name);
        }
    }

    void Update()
    {
        if (personaje == null) return;

        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        float velocidadFinal = velocidadCaballero3 * Time.deltaTime;

        if (distancia <= 4f)
        {
            //acercarse
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            ActualizarDireccion();

            caballero3_AnimController.SetBool("caballero3ActivarCaminar", true);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);

            if (distancia <= 2f)
            {
                //atacar
                caballero3_AnimController.SetBool("caballero3ActivarCaminar", false);

                if (PuedeAtacar())
                {
                    caballero3_AnimController.SetBool("caballero3ActivarAtacar", true);

                    if (scriptAtaque != null)
                    {
                        scriptAtaque.IniciarAtaque();
                    }
                    
                    tiempoUltimoAtaque = Time.time;
                }
                else
                {
                    if (scriptAtaque == null || !scriptAtaque.EstaAtacando())
                    {
                        caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);
                    }
                }
            }
        }
        else
        {
            //volver
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