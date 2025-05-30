using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueCaballero : MonoBehaviour
{
    public GameObject hitboxEnemigo;
    public Transform puntoAtaque;
    public Vector3 offsetDerecha = new Vector3(0.7f, 0.3f, 0f);
    public Vector3 offsetIzquierda = new Vector3(-0.7f, 0.3f, 0f);
    public float danoAtaque = 1f;
    public float duracionHitbox = 0.5f;
    public float tiempoEsperaAtaque = 0.3f;

    public LayerMask capasJugador = 1 << 7;
    
    // Variables privadas
    private GameObject hitboxPrivada;
    private bool atacando = false;
    private bool mirandoDerecha = true;
    private Animator animatorController;
    
    private bool animacionAtaqueAnterior = false;

    void Start()
    {
        animatorController = GetComponent<Animator>();
        
        if (puntoAtaque == null)
        {
            puntoAtaque = transform;
        }
    }

    void Update()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            mirandoDerecha = jugador.transform.position.x > transform.position.x;
        }

        ActualizarPosicionHitbox();

        DetectarInicioAtaque();
    }

    void DetectarInicioAtaque()
    {
        if (animatorController != null)
        {
            bool atacandoAhora = animatorController.GetBool("caballero3ActivarAtacar");

            if (atacandoAhora && !animacionAtaqueAnterior && !atacando)
            {
                Debug.Log("Detectado inicio de animación de ataque del Caballero3!");
                StartCoroutine(EsperarYAtacar());
            }
            
            animacionAtaqueAnterior = atacandoAhora;
        }
    }

    IEnumerator EsperarYAtacar()
    {
        yield return new WaitForSeconds(tiempoEsperaAtaque);

        if (animatorController.GetBool("caballero3ActivarAtacar"))
        {
            IniciarAtaqueAutomatico();
        }
    }

    public void IniciarAtaque()
    {
        if (!atacando)
        {
            StartCoroutine(EjecutarAtaque());
        }
    }

    void IniciarAtaqueAutomatico()
    {
        if (!atacando)
        {
            Debug.Log("¡Iniciando ataque automático del Caballero3!");
            StartCoroutine(EjecutarAtaque());
        }
    }

    void ActualizarPosicionHitbox()
    {
        if (hitboxPrivada != null)
        {
            Vector3 offset = mirandoDerecha ? offsetDerecha : offsetIzquierda;
            hitboxPrivada.transform.position = puntoAtaque.position + offset;
        }
    }

    IEnumerator EjecutarAtaque()
    {
        atacando = true;
        
        CrearHitbox();

        yield return new WaitForSeconds(duracionHitbox);
        
        FinalizarAtaque();
    }

    void CrearHitbox()
    {
        Vector3 offset = mirandoDerecha ? offsetDerecha : offsetIzquierda;
        Vector3 posicionHitbox = puntoAtaque.position + offset;

        hitboxPrivada = Instantiate(hitboxEnemigo, posicionHitbox, Quaternion.identity);
        Debug.Log("Hitbox del Caballero3 creada en posición: " + posicionHitbox);

        HitboxAtaqueCaballero3 hitboxScript = hitboxPrivada.GetComponent<HitboxAtaqueCaballero3>();
        if (hitboxScript != null)
        {
            hitboxScript.ConfigurarAtaque(danoAtaque, mirandoDerecha, gameObject);
            Debug.Log("Hitbox del Caballero3 configurada correctamente");
        }
        else
        {
            Debug.LogError("No se encontró el script HitboxAtaqueCaballero3 en el prefab de hitbox");
        }
    }

    void FinalizarAtaque()
    {
        if (hitboxPrivada != null)
        {
            Destroy(hitboxPrivada);
            hitboxPrivada = null;
            Debug.Log("Hitbox del Caballero3 destruida");
        }

        atacando = false;
    }

    public bool EstaAtacando()
    {
        return atacando;
    }

    public void EstablecerDireccion(bool direccionDerecha)
    {
        mirandoDerecha = direccionDerecha;
    }
}