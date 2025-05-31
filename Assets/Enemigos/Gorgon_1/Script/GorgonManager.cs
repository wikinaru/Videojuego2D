using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorgonManager : MonoBehaviour
{
    public float velocidadGorgon1 = 2f;
    public float distanciaAtaque = 2f;
    public float distanciaPerseguir = 3.5f;
    
    // Variables privadas
    private Vector3 posicionInical;
    private GameObject personaje;
    private Animator gorgon1_AnimController;
    
    // Variables para FixedUpdate
    private float distanciaActual;
    private bool deberiaMoverse;
    private bool deberiaAtacar;
    
    // Variables para audio
    private bool estabaCaminando = false;

    void Start()
    {
        gorgon1_AnimController = GetComponent<Animator>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
        
        if (personaje == null)
        {
            Debug.LogError("No se encontró el Player. Asegúrate de que tenga el tag 'Player'");
        }
    }

    void Update()
    {
        if (personaje == null) return;
        
        distanciaActual = Vector3.Distance(transform.position, personaje.transform.position);
        
        if (distanciaActual <= distanciaAtaque)
        {
            // ATACAR
            deberiaMoverse = false;
            deberiaAtacar = true;
            
            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", false);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", true);
            
            if (estabaCaminando)
            {
                estabaCaminando = false;
            }
        }
        else if (distanciaActual <= distanciaPerseguir)
        {
            // CAMINAR/PERSEGUIR
            deberiaMoverse = true;
            deberiaAtacar = false;
            
            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", true);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);
            
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
            // IDLE/VOLVER
            deberiaMoverse = false;
            deberiaAtacar = false;
            
            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", false);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);
            
            if (estabaCaminando)
            {
                estabaCaminando = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (personaje == null) return;
        
        if (deberiaMoverse)
        {
            float velocidadFinal = velocidadGorgon1 * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);
        }
    }
}