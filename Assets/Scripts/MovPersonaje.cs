using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovPersonaje : MonoBehaviour
{
    public float multiplicador = 4f;
    public float multiplicadorSalto = 4f;
    private Rigidbody2D rb;
    private bool salto = true;
    private int saltosTotales;
    private int saltosRestantes;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Variables
        float moverse = Input.GetAxis("Horizontal");
        float miDeltaTime = Time.deltaTime;

        //Moverse
        rb.velocity = new Vector2(moverse * multiplicador, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D))
        {

        } else if (moverse < 0) {
            this.GetComponent<SpriteRenderer>().flipX = true;
        } else if (moverse > 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
        }

        //Saltar
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
        Debug.DrawRay(transform.position, Vector2.down, Color.magenta);

        if (hit)
        {
            salto = true;
            Debug.Log(hit.collider.name);
        }
        else
        {
            salto = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && salto)
        {
            rb.AddForce(
                new Vector2(0, multiplicadorSalto),
                ForceMode2D.Impulse
                );
            salto = false;
        }
    }
}
