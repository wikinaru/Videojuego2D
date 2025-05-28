using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaquePersonaje : MonoBehaviour
{
    private Animator animatorController;
    private bool atacando = false;
    public GameObject hitbox;
    public GameObject personaje;
    public Vector3 offset = new Vector3(1f, 0f, 0f);

    private GameObject hitboxPrivada;

    void Start()
    {
        animatorController = GetComponent<Animator>();
    }

    void Update()
    {
        if (!atacando && Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(ActivarAtaque());
        }

        if (hitboxPrivada != null)
        {
            hitboxPrivada.transform.position = personaje.transform.position + offset;
        }
    }

    IEnumerator ActivarAtaque()
    {
        atacando = true;

        hitboxPrivada = Instantiate(hitbox, personaje.transform.position + offset, Quaternion.identity);

        animatorController.SetBool("activarAtacar", true);

        yield return null;

        AnimatorStateInfo animatorInfo = animatorController.GetCurrentAnimatorStateInfo(0);
        float duracionAtaque = animatorInfo.length;

        yield return new WaitForSeconds(duracionAtaque);

        animatorController.SetBool("activarAtacar", false);
        Destroy(hitboxPrivada);
        atacando = false;
    }
}
