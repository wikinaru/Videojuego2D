using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueGorgon3 : MonoBehaviour
{
    public float danoAtaque = 1.5f;
    public float rangoAtaque = 2.2f;
    public LayerMask capasJugador = 1 << 7;
    
    public int frameInicioAtaque = 3;
    public int frameFinAtaque = 12;
    
    // Variables privadas
    private bool atacando = false;
    private bool mirandoDerecha = true;
    private Animator animatorController;
    private HashSet<GameObject> jugadoresGolpeados;
    private bool animacionAtaqueAnterior = false;
    private bool frameAtaqueActivo = false;
    
    // Variables para físicas
    private bool aplicarKnockbackPendiente = false;
    private GameObject jugadorParaKnockback;
    
    // Variable para controlar el audio (para evitar múltiples reproducciones)
    private bool audioAtaqueReproducido = false;

    void Start()
    {
        animatorController = GetComponent<Animator>();
        jugadoresGolpeados = new HashSet<GameObject>();
    }

    void Update()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            mirandoDerecha = jugador.transform.position.x > transform.position.x;
        }

        DetectarInicioAtaque();
        
        if (atacando)
        {
            VerificarFramesAtaque();
        }
    }

    void FixedUpdate()
    {
        if (aplicarKnockbackPendiente && jugadorParaKnockback != null)
        {
            AplicarKnockBack(jugadorParaKnockback);
            aplicarKnockbackPendiente = false;
            jugadorParaKnockback = null;
        }
    }

    void DetectarInicioAtaque()
    {
        if (animatorController != null)
        {
            bool atacandoAhora = animatorController.GetBool("gorgon3ActivarAtacar");

            if (atacandoAhora && !animacionAtaqueAnterior)
            {
                Debug.Log("Detectado inicio de animación de ataque del Gorgon3!");
                IniciarAtaque();
            }
            else if (!atacandoAhora && animacionAtaqueAnterior)
            {
                FinalizarAtaque();
            }
            
            animacionAtaqueAnterior = atacandoAhora;
        }
    }

    void VerificarFramesAtaque()
    {
        if (animatorController != null)
        {
            AnimatorStateInfo stateInfo = animatorController.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("ataqueGorgon3Anim") || stateInfo.IsTag("Attack"))
            {
                float frameActual = stateInfo.normalizedTime * GetFramesTotales();
                
                bool enFrameAtaque = frameActual >= frameInicioAtaque && frameActual <= frameFinAtaque;

                if (enFrameAtaque && !frameAtaqueActivo)
                {
                    frameAtaqueActivo = true;
                    Debug.Log($"Frames de ataque iniciados: {frameActual:F1} (frames {frameInicioAtaque}-{frameFinAtaque})");
                }
                else if (!enFrameAtaque && frameAtaqueActivo)
                {
                    frameAtaqueActivo = false;
                    Debug.Log("Frames de ataque finalizados");
                }

                if (frameAtaqueActivo)
                {
                    EjecutarAtaqueEnFrame();
                }
            }
        }
    }

    int GetFramesTotales()
    {
        return 16;
    }

    void IniciarAtaque()
    {
        atacando = true;
        jugadoresGolpeados.Clear();
        frameAtaqueActivo = false;
        audioAtaqueReproducido = false;
        Debug.Log("Gorgon3 iniciando secuencia de ataque");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirEfectoAtaqueGorgons();
            audioAtaqueReproducido = true;
        }
    }

    void EjecutarAtaqueEnFrame()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        
        if (jugador != null && !jugadoresGolpeados.Contains(jugador))
        {
            float distancia = Vector3.Distance(transform.position, jugador.transform.position);
            
            if (distancia <= rangoAtaque)
            {
                Debug.Log($"¡Gorgon3 golpea al jugador en frames {frameInicioAtaque}-{frameFinAtaque}!");
                ProcesarImpacto(jugador);
            }
        }
    }

    void ProcesarImpacto(GameObject jugador)
    {
        jugadoresGolpeados.Add(jugador);
        Debug.Log("Aplicando " + danoAtaque + " de daño al jugador: " + jugador.name);
        
        AplicarDamage(jugador);
    }

    void AplicarDamage(GameObject jugador)
    {
        GameManager.DanarJugador(jugador, danoAtaque);
        
        jugadorParaKnockback = jugador;
        aplicarKnockbackPendiente = true;
    }

    void AplicarKnockBack(GameObject jugador)
    {
        Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
        if (rbJugador != null)
        {
            Vector2 direccionKnockback = mirandoDerecha ? Vector2.right : Vector2.left;
            float fuerzaKnockback = 5f;
            rbJugador.AddForce(direccionKnockback * fuerzaKnockback, ForceMode2D.Impulse);
            Debug.Log("Aplicando knockback hacia: " + direccionKnockback);
        }
    }

    void FinalizarAtaque()
    {
        atacando = false;
        frameAtaqueActivo = false;
        audioAtaqueReproducido = false;
        Debug.Log("Gorgon3 finalizando ataque");
    }

    public bool EstaAtacando()
    {
        return atacando;
    }

    public void ActivarFrameAtaque()
    {
        if (atacando)
        {
            frameAtaqueActivo = true;
        }
    }

    public void DesactivarFrameAtaque()
    {
        frameAtaqueActivo = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}