using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorgon2Manager : MonoBehaviour
{
 Vector3 posicionInical;
    GameObject personaje;
    public float velocidadGorgon2 = 2f;
    private Animator gorgon2_AnimController;
    
    void Start()
    {
        gorgon2_AnimController = GetComponent<Animator>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        float velocidadFinal = velocidadGorgon2 * Time.deltaTime;

        if (distancia <= 3.5f)
        {
            //acercarse
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", true);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", false);

            if (distancia <= 2f)
            {
                //atacar
                gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", false);
                gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", true);
            }

        }
        else
        {
            //volver
            gorgon2_AnimController.SetBool("gorgon2ActivarCaminar", true);
            gorgon2_AnimController.SetBool("gorgon2ActivarAtacar", false);
            transform.position = Vector3.MoveTowards(transform.position, posicionInical, velocidadFinal);
        }
    }
}
