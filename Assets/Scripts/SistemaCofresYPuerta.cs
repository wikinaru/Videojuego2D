using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SistemaCofresYPuerta : MonoBehaviour
{
    public GameObject saltoCofre;
    public GameObject llaveCofre;
    public GameObject puerta;
    
    public Sprite spriteCofrecerrado;
    public Sprite spriteCofreabierto;

    public Sprite spritePuertaCerrada;
    public Sprite spritePuertaAbierta;
    
    public string nombreEscenaSiguiente = "2FinalScene";
    public float tiempoAntesDeTerminar = 2f;
    
    private string tagPersonaje = "Player";
    private MovPersonaje _movPersonaje;
    
    // Estados
    private bool dobleSaltoActivado = false;
    private bool llaveObtenida = false;
    private bool saltoCofreAbierto = false;
    private bool llaveCofreAbierto = false;
    
    void Start()
    {
        GameObject personaje = GameObject.FindGameObjectWithTag(tagPersonaje);
        if (personaje == null)
            personaje = GameObject.Find("Personaje");
            
        if (personaje != null)
        {
            _movPersonaje = personaje.GetComponent<MovPersonaje>();
            if (_movPersonaje == null)
            {
                Debug.LogError("SistemaCofresYPuerta: No se encontró MovPersonaje");
            }
        }
        else
        {
            Debug.LogError("SistemaCofresYPuerta: No se encontró el personaje");
        }
        
        ConfigurarCofre(saltoCofre, "SaltoCofre");
        ConfigurarCofre(llaveCofre, "LlaveCofre");

        ConfigurarPuerta();
    }
    
    void ConfigurarCofre(GameObject cofre, string tipoCofre)
    {
        if (cofre == null)
        {
            Debug.LogWarning($"SistemaCofresYPuerta: {tipoCofre} no está asignado");
            return;
        }
    
        Collider2D collider = cofre.GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = cofre.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
    
        SpriteRenderer spriteRenderer = cofre.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = cofre.AddComponent<SpriteRenderer>();
        }
        
        if (spriteCofrecerrado != null)
        {
            spriteRenderer.sprite = spriteCofrecerrado;
        }
        
        if (tipoCofre == "SaltoCofre")
        {
            CofreInteraccion saltoCofre = cofre.GetComponent<CofreInteraccion>();
            if (saltoCofre == null)
            {
                saltoCofre = cofre.AddComponent<CofreInteraccion>();
            }
            saltoCofre.ConfigurarCofre(this, TipoCofre.DobleSalto);
        }
        else if (tipoCofre == "LlaveCofre")
        {
            CofreInteraccion llaveCofre = cofre.GetComponent<CofreInteraccion>();
            if (llaveCofre == null)
            {
                llaveCofre = cofre.AddComponent<CofreInteraccion>();
            }
            llaveCofre.ConfigurarCofre(this, TipoCofre.Llave);
        }
    }
    
    void ConfigurarPuerta()
    {
        if (puerta == null)
        {
            Debug.LogWarning("SistemaCofresYPuerta: Puerta no está asignada");
            return;
        }
        
        Collider2D collider = puerta.GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = puerta.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
        
        PuertaInteraccion puertaScript = puerta.GetComponent<PuertaInteraccion>();
        if (puertaScript == null)
        {
            puertaScript = puerta.AddComponent<PuertaInteraccion>();
        }
        puertaScript.ConfigurarPuerta(this);
    }
    
    public void ActivarSaltoCofre()
    {
        if (saltoCofreAbierto) return;
        
        saltoCofreAbierto = true;
        dobleSaltoActivado = true;
        
        if (_movPersonaje != null)
        {
            _movPersonaje.ActivarDobleSalto();
        }
        
        CambiarSpriteCofre(saltoCofre);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirEfectoCofre();
        }
        
        Debug.Log("Cofre de salto abierto - Doble salto desbloqueado!");
    }
    
    public void ActivarLlaveCofre()
    {
        if (llaveCofreAbierto) return;
        
        llaveCofreAbierto = true;
        llaveObtenida = true;
        
        CambiarSpriteCofre(llaveCofre);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirEfectoCofre();
        }
        
        GameManager.ObtenerLlave();
        
        Debug.Log("Cofre de llave abierto - Llave obtenida!");
    }
    
    public void IntentarAbrirPuerta()
    {
        if (llaveObtenida)
        {
            AbrirPuerta();
        }
        else
        {
            Debug.Log("Necesitas la llave para abrir la puerta");
        }
    }
    
    private void AbrirPuerta()
    {
        Debug.Log("¡Puerta abierta! Terminando demo...");

        CambiarSpritePuerta();
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirEfectoPuerta();
        }

        StartCoroutine(TerminarDemo());
    }
    
    private IEnumerator TerminarDemo()
    {
        yield return new WaitForSeconds(tiempoAntesDeTerminar);

        if (!string.IsNullOrEmpty("2FinalScene"))
        {
            SceneManager.LoadScene("2FinalScene");
        }
        else
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
    
    private void CambiarSpriteCofre(GameObject cofre)
    {
        if (cofre != null && spriteCofreabierto != null)
        {
            SpriteRenderer spriteRenderer = cofre.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = spriteCofreabierto;
            }
        }
    }

    private void CambiarSpritePuerta()
    {
        if (puerta != null && spritePuertaAbierta != null)
        {
            SpriteRenderer spriteRenderer = puerta.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = spritePuertaAbierta;
            }
        }
        else if (spritePuertaAbierta == null)
        {
            Debug.LogWarning("SistemaCofresYPuerta: No se asignó sprite de puerta abierta");
        }
    }
    
    public bool TieneLlave() { return llaveObtenida; }
    public bool TieneDobleSalto() { return dobleSaltoActivado; }
}

public enum TipoCofre
{
    DobleSalto,
    Llave
}

public class CofreInteraccion : MonoBehaviour
{
    private SistemaCofresYPuerta sistema;
    private TipoCofre tipoCofre;
    private bool yaActivado = false;

    public void ConfigurarCofre(SistemaCofresYPuerta sistemaPrincipal, TipoCofre tipo)
    {
        sistema = sistemaPrincipal;
        tipoCofre = tipo;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaActivado) return;

        bool esPersonaje = collision.CompareTag("Player") || collision.name == "Personaje";

        if (esPersonaje && sistema != null)
        {
            yaActivado = true;

            if (tipoCofre == TipoCofre.DobleSalto)
            {
                sistema.ActivarSaltoCofre();
            }
            else if (tipoCofre == TipoCofre.Llave)
            {
                sistema.ActivarLlaveCofre();
            }
        }
    }
}

public class PuertaInteraccion : MonoBehaviour
{
    private SistemaCofresYPuerta sistema;
        
    public void ConfigurarPuerta(SistemaCofresYPuerta sistemaPrincipal)
    {
        sistema = sistemaPrincipal;
    }
        
    void OnTriggerEnter2D(Collider2D collision)
    {
        bool esPersonaje = collision.CompareTag("Player") || collision.name == "Personaje";
        
        if (esPersonaje && sistema != null)
        {
            sistema.IntentarAbrirPuerta();
        }
    }
}