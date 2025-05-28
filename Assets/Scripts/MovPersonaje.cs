using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovPersonaje : MonoBehaviour
{
    GameObject respawn;
    private Animator animatorController;
    private AtaquePersonaje ataqueScript;
    private Rigidbody2D rb;

    public float multiplicador = 4f;
    public float multiplicadorSalto = 4f;
    public static bool direccion = true;

    private bool suelo;
    private int saltosTotales = 1;
    private int saltosRestantes;
    private bool puedeMoverse = true;

    void Awake()
    {
        respawn = GameObject.Find("Respawn");

        if (respawn == null)
        {
            Debug.LogError("No se ha encontrado el Respawn");
        }
        
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animatorController = GetComponent<Animator>();
        Spawn_Inicial();
        saltosRestantes = saltosTotales;
        ataqueScript = GetComponent<AtaquePersonaje>();
    }

    void Update()
    {
        //Variables
        float moverse = Input.GetAxis("Horizontal");
        float miDeltaTime = Time.deltaTime;

        if (!puedeMoverse || GameManager.morir) return;

        if (animatorController.GetBool("atacando"))
        {
            rb.velocity = Vector2.zero;
        }

        //Moverse
        rb.velocity = new Vector2(moverse * multiplicador, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D))
        {

        }
        else if (moverse < 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
            direccion = true;
        }
        else if (moverse > 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
            direccion = false;
        }

        if (moverse != 0)
        {
            animatorController.SetBool("activarCorrer", true);
        }
        else
        {
            animatorController.SetBool("activarCorrer", false);
        }

        //Saltar
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
        suelo = hit.collider != null;

        if (suelo)
        {
            saltosRestantes = saltosTotales;
            animatorController.SetBool("activarSalto", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0)
        {
            rb.AddForce(
                new Vector2(0, multiplicadorSalto),
                ForceMode2D.Impulse
                );
            saltosRestantes--;
            animatorController.SetBool("activarSalto", true);
        }

        //Caerse del mapa
        if (transform.position.y <= -10)
        {
            Respawnear();
        }

        //Morir
        if (GameManager.barraVida <= 0)
        {
            GameManager.morir = true;
        }
    }

    public void ActivarDobleSalto()
    {
        saltosTotales = 2;
        saltosRestantes = saltosTotales;
    }

    public void Spawn_Inicial()
    {
        transform.position = respawn.transform.position;
    }

    public void Movimiento(bool activar)
    {
        puedeMoverse = activar;
        if (!activar) 
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Respawnear()
    {
        Debug.Log(GameManager.barraVida);
        GameManager.barraVida = GameManager.barraVida - 2;
        Debug.Log(GameManager.barraVida);
        transform.position = respawn.transform.position;
    }
}
