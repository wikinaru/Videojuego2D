using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorgon3Manager : MonoBehaviour
{
    Vector3 posicionInical;
    GameObject personaje;
    public float velocidadGorgon3 = 2f;
    private Animator gorgon3_AnimController;

    void Start()
    {
        gorgon3_AnimController = GetComponent<Animator>();
        posicionInical = transform.position;
        personaje = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, personaje.transform.position);
        gorgon3_AnimController.SetBool("gorgon3ActivarAtacar", false);

        if (distancia <= 2f)
        {
            gorgon3_AnimController.SetBool("gorgon3ActivarAtacar", true);
        }
    }
}
