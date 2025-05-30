using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caballero2Manager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;
    public float velocidadCaballero2 = 2f;
    private Animator caballero2_AnimController;
    
    void Start()
    {
        caballero2_AnimController = GetComponent<Animator>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        float velocidadFinal = velocidadCaballero2 * Time.deltaTime;

        if (distancia <= 4f)
        {
            //acercarse
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            caballero2_AnimController.SetBool("caballero2ActivarCaminar", true);
            caballero2_AnimController.SetBool("caballero2ActivarAtacar", false);

            if (distancia <= 2f)
            {
                //atacar
                caballero2_AnimController.SetBool("caballero2ActivarCaminar", false);

                caballero2_AnimController.SetBool("caballero2ActivarAtacar", true);
            }

        }
        else
        {
            //volver
            caballero2_AnimController.SetBool("caballero2ActivarCaminar", true);
            caballero2_AnimController.SetBool("caballero2ActivarAtacar", false);
            transform.position = Vector3.MoveTowards(transform.position, posicionInical, velocidadFinal);
        }
    }
}
