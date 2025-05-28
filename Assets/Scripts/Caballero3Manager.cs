using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caballero3Manager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;
    public float velocidadCaballero3 = 2f;
    private Animator caballero3_AnimController;
    
    void Start()
    {
        caballero3_AnimController = GetComponent<Animator>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        float velocidadFinal = velocidadCaballero3 * Time.deltaTime;

        if (distancia <= 4f)
        {
            //acercarse
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            caballero3_AnimController.SetBool("caballero3ActivarCaminar", true);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);

            if (distancia <= 2f)
            {
                //atacar
                caballero3_AnimController.SetBool("caballero3ActivarCaminar", false);
                caballero3_AnimController.SetBool("caballero3ActivarAtacar", true);
            }

        }
        else
        {
            //volver
            caballero3_AnimController.SetBool("caballero3ActivarCaminar", true);
            caballero3_AnimController.SetBool("caballero3ActivarAtacar", false);
            transform.position = Vector3.MoveTowards(transform.position, posicionInical, velocidadFinal);
        } 
    }
}
