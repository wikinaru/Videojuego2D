using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorgonManager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;

    public float velocidadGorgon1 = 2f;
    public float tiempoEntreAataques = 1.5f;
    
    private Animator gorgon1_AnimController;
    private AtaqueGorgon scriptAtaque;
    private float tiempoUltimoAtaque = 0f;
    private SpriteRenderer spriteRenderer;
    private bool mirandoDerecha = true;

    void Start()
    {
        gorgon1_AnimController = GetComponent<Animator>();
        scriptAtaque = GetComponent<AtaqueGorgon>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
        
        if (scriptAtaque == null)
        {
            Debug.LogWarning("AtaqueEnemigo no encontrado en " + gameObject.name);
        }
    }

    void Update()
    {
        if (personaje == null) return;

        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        float velocidadFinal = velocidadGorgon1 * Time.deltaTime;

        if (distancia <= 3.5f)
        {
            //acercarse
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            ActualizarDireccion();

            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", true);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);

            if (distancia <= 2f)
            {
                //atacar
                gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", false);
                
                if (PuedeAtacar())
                {
                    gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", true);
                    
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
                        gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);
                    }
                }
            }
        }
        else
        {
            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", false);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);
        }
    }

    bool PuedeAtacar()
    {
        bool tiempoSuficiente = Time.time - tiempoUltimoAtaque >= tiempoEntreAataques;
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