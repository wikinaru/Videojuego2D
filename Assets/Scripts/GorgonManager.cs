using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorgonManager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;

    public float velocidadGorgon1 = 2f;
    private Animator gorgon1_AnimController;

    void Start()
    {
        gorgon1_AnimController = GetComponent<Animator>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        float velocidadFinal = velocidadGorgon1 * Time.deltaTime;

        if (distancia <= 3.5f)
        {
            //acercarse
            transform.position = Vector3.MoveTowards(transform.position, personaje.transform.position, velocidadFinal);

            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", true);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);

            if (distancia <= 2f)
            {
                //atacar
                gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", false);

                gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", true);
            }

        }
        else
        {
            //volver
            gorgon1_AnimController.SetBool("gorgon1ActivarCaminar", false);
            gorgon1_AnimController.SetBool("gorgon1ActivarAtacar", false);
        }
    }
}
