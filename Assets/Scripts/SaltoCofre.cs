using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltoCofre : MonoBehaviour
{
    private string tagPersonaje = "Player";
    public Sprite spriteCofrecerrado;
    public Sprite spriteCofreabierto;
    private AudioClip sonidoActivacion;
    
    private MovPersonaje _movPersonaje;
    private SpriteRenderer _spriteRenderer;
    private bool yaActivado = false;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (_spriteRenderer == null)
        {
            Debug.LogError("SaltoCofre: No se encontró SpriteRenderer en " + gameObject.name);
        }

        if (spriteCofrecerrado == null && _spriteRenderer != null)
        {
            spriteCofrecerrado = _spriteRenderer.sprite;
        }

        GameObject personaje = GameObject.FindGameObjectWithTag(tagPersonaje);
        
        if (personaje == null)
        {
            personaje = GameObject.Find("Personaje");
        }
        
        if (personaje != null)
        {
            _movPersonaje = personaje.GetComponent<MovPersonaje>();
            
            if (_movPersonaje == null)
            {
                Debug.LogError("SaltoCofre: No se encontró el componente MovPersonaje en " + personaje.name);
            }
        }
        else
        {
            Debug.LogError("SaltoCofre: No se encontró el personaje");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaActivado) return;
        
        bool esPersonaje = collision.CompareTag(tagPersonaje) || collision.name == "Personaje";
        
        if (esPersonaje && _movPersonaje != null)
        {
            ActivarDobleSalto();
        }
    }
    
    private void ActivarDobleSalto()
    {
        yaActivado = true;
        
        _movPersonaje.ActivarDobleSalto();

        CambiarSpriteCofre();

        if (sonidoActivacion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoActivacion, transform.position);
        }
        
        Debug.Log("Cofre abierto - Doble salto desbloqueado!");
    }
    
    private void CambiarSpriteCofre()
    {
        if (_spriteRenderer != null && spriteCofreabierto != null)
        {
            _spriteRenderer.sprite = spriteCofreabierto;
        }
        else if (spriteCofreabierto == null)
        {
            Debug.LogWarning("SaltoCofre: No se asignó sprite de cofre abierto en " + gameObject.name);
        }
    }
    
    public void CerrarCofre()
    {
        if (_spriteRenderer != null && spriteCofrecerrado != null)
        {
            _spriteRenderer.sprite = spriteCofrecerrado;
            yaActivado = false;
        }
    }
}