using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaquePersonaje : MonoBehaviour
{
    //Variables para ataque
    public GameObject hitboxActual;
    public Transform puntoAtaque;
    public Vector3 offsetDerecha = new Vector3(1f, 0f, 0f);
    public Vector3 offsetIzquierda = new Vector3(-1f, 0f, 0f);

    //Variables para la animación
    private Animator animatorController;
    private GameObject hitboxPrivada;
    private bool atacando = false;
    private bool mirandoDerecha = true;

    public GameObject personaje;

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
        if (!atacando && Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(ActivarAtaque());
        }

        ActualizarPosicionHitbox();
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

    public void CambiarDireccion(bool nuevaDireccion)
    {
        mirandoDerecha = nuevaDireccion;
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
