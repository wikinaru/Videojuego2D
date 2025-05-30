using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueGorgon : MonoBehaviour
{
    public GameObject hitboxEnemigo;
    public Transform puntoAtaque;
    public Vector3 offsetDerecha = new Vector3(0.7f, 0f, 0f);
    public Vector3 offsetIzquierda = new Vector3(-0.7f, 0f, 0f);
    public float danoAtaque = 1f;
    public float duracionHitbox = 0.5f;

    public LayerMask capasJugador = 1 << 0;
    
    // Variables privadas
    private GameObject hitboxPrivada;
    private bool atacando = false;
    private bool mirandoDerecha = true;
    private Animator animatorController;
    
    // Variables para detección automática
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
            bool atacandoAhora = animatorController.GetBool("gorgon1ActivarAtacar");

            if (atacandoAhora && !animacionAtaqueAnterior && !atacando)
            {
                Debug.Log("Detectado inicio de animación de ataque!");
                StartCoroutine(EsperarYAtacar());
            }
            
            animacionAtaqueAnterior = atacandoAhora;
        }
    }

    IEnumerator EsperarYAtacar()
    {
        yield return new WaitForSeconds(0.3f);

        if (animatorController.GetBool("gorgon1ActivarAtacar"))
        {
            IniciarAtaque();
        }
    }

    public void IniciarAtaque()
    {
        if (!atacando)
        {
            Debug.Log("¡Iniciando ataque del Gorgon!");
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
        Debug.Log("Hitbox creada en posición: " + posicionHitbox);

        HitboxAtaqueGorgon hitboxScript = hitboxPrivada.GetComponent<HitboxAtaqueGorgon>();
        if (hitboxScript != null)
        {
            hitboxScript.ConfigurarAtaque(danoAtaque, mirandoDerecha, gameObject);
            Debug.Log("Hitbox configurada correctamente");
        }
        else
        {
            Debug.LogError("No se encontró el script HitboxAtaqueGorgon en el prefab de hitbox");
        }
    }

    void FinalizarAtaque()
    {
        if (hitboxPrivada != null)
        {
            Destroy(hitboxPrivada);
            hitboxPrivada = null;
            Debug.Log("Hitbox destruida");
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