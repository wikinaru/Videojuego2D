using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaquePersonaje : MonoBehaviour
{
    //Configuración de Ataque
    public GameObject hitboxActual;
    public Transform puntoAtaque;
    public Vector3 offsetDerecha = new Vector3(0.7f, 0.9f, 0f);
    public Vector3 offsetIzquierda = new Vector3(-0.7f, 0.9f, 0f);

    //Referencias
    public GameObject personaje;

    // Variables privadas
    private Animator animatorController;
    private MovPersonaje scriptMovimiento;
    private GameObject hitboxPrivada;
    private bool atacando = false;
    
    private bool mirandoDerecha = true;

    void Start()
    {
        animatorController = GetComponent<Animator>();
        
        if (puntoAtaque == null)
        {
            puntoAtaque = transform;
        }

        if (personaje != null)
        {
            scriptMovimiento = personaje.GetComponent<MovPersonaje>();
        }
        else
        {
            scriptMovimiento = GetComponent<MovPersonaje>();
        }

        if (scriptMovimiento == null)
        {
            Debug.LogError("No se pudo encontrar el script MovPersonaje");
        }
    }

    void Update()
    {
        if (scriptMovimiento != null)
        {
            mirandoDerecha = scriptMovimiento.MirandoDerecha;
        }

        if (!atacando && Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(ActivarAtaque());
        }
    }

    void FixedUpdate()
    {
        ActualizarPosicionHitbox();
    }

    public void ActualizarDireccionExterna(bool nuevaDireccion)
    {
        mirandoDerecha = nuevaDireccion;
    }

    void ActualizarPosicionHitbox()
    {
        if (hitboxPrivada != null)
        {
            Vector3 offset = mirandoDerecha ? offsetDerecha : offsetIzquierda;
            hitboxPrivada.transform.position = puntoAtaque.position + offset;
        }
    }

    IEnumerator ActivarAtaque()
    {
        atacando = true;
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirEfectoAtaqueJugador();
        }
        
        CrearHitbox();
        animatorController.SetBool("activarAtacar", true);
        
        yield return null;
        yield return StartCoroutine(EsperarAnimacion());
        
        FinalizarAtaque();
    }

    void CrearHitbox()
    {
        Vector3 offset = mirandoDerecha ? offsetDerecha : offsetIzquierda;
        Vector3 posicionHitbox = puntoAtaque.position + offset;

        hitboxPrivada = Instantiate(hitboxActual, posicionHitbox, Quaternion.identity);

        ataqueScript hitboxScript = hitboxPrivada.GetComponent<ataqueScript>();
        if (hitboxScript != null)
        {
            hitboxScript.ConfigurarDireccion(mirandoDerecha);
        }
    }

    IEnumerator EsperarAnimacion()
    {
        while (!animatorController.GetCurrentAnimatorStateInfo(0).IsName("Ataque_1"))
        {
            yield return null;
        }

        AnimatorStateInfo estadoAnimacion = animatorController.GetCurrentAnimatorStateInfo(0);
        float tiempoRestante = estadoAnimacion.length;

        yield return new WaitForSeconds(tiempoRestante);
    }

    void FinalizarAtaque()
    {
        animatorController.SetBool("activarAtacar", false);

        if (hitboxPrivada != null)
        {
            Destroy(hitboxPrivada);
            hitboxPrivada = null;
        }

        atacando = false;
    }

    public void IniciarHitbox()
    {
        if (!atacando) return;
        CrearHitbox();
    }

    public void TerminarHitbox()
    {
        if (hitboxPrivada != null)
        {
            Destroy(hitboxPrivada);
            hitboxPrivada = null;
        }
    }
}