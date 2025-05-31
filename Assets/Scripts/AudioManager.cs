using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip bandaSonoraInicio;
    public AudioClip bandaSonoraJuego;
    public AudioClip bandaSonoraFinal;
    
    public AudioClip fxGorgons;
    public AudioClip fxCaballeros;
    public AudioClip fxAtaqueGorgons;
    public AudioClip fxAtaqueCaballero2;
    public AudioClip fxAtaqueCaballero3;
    public AudioClip fxGorgonsMoverse;
    public AudioClip fxCaballerosMoverse;
    public AudioClip fxMorirGorgons;
    public AudioClip fxMorirCaballero;
    
    public AudioClip fxAtaqueJugador;
    public AudioClip fxJugadorMoverse;
    public AudioClip fxJugadorSaltar;
    public AudioClip fxMorirJugador;
    
    public AudioClip fxCofres;
    public AudioClip fxPuerta;

    public float volumenMusica = 0.2f;
    public float volumenEfectos = 0.3f;

    private AudioSource musicaAudioSource;
    private AudioSource efectosAudioSource;

    private string escenaActual;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        ConfigurarAudioSources();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void ConfigurarAudioSources()
    {
        musicaAudioSource = gameObject.AddComponent<AudioSource>();
        musicaAudioSource.loop = true;
        musicaAudioSource.volume = volumenMusica;
        musicaAudioSource.playOnAwake = false;

        efectosAudioSource = gameObject.AddComponent<AudioSource>();
        efectosAudioSource.loop = false;
        efectosAudioSource.volume = volumenEfectos;
        efectosAudioSource.playOnAwake = false;
    }

    void Start()
    {
        escenaActual = SceneManager.GetActiveScene().name;
        CambiarMusicaSegunEscena(escenaActual);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        escenaActual = scene.name;
        CambiarMusicaSegunEscena(escenaActual);
    }

    void CambiarMusicaSegunEscena(string nombreEscena)
    {
        AudioClip nuevaMusica = null;

        switch (nombreEscena)
        {
            case "0InicioScene":
            case "inicio":
            case "menu":
                nuevaMusica = bandaSonoraInicio;
                break;
                
            case "1JuegoScene":
            case "juego":
            case "nivel1":
            case "gameplay":
                nuevaMusica = bandaSonoraJuego;
                break;
                
            case "2FinalScene":
            case "final":
            case "gameover":
            case "victoria":
                nuevaMusica = bandaSonoraFinal;
                break;
                
            default:
                Debug.LogWarning($"Escena no reconocida para música: {nombreEscena}");
                return;
        }

        if (nuevaMusica != null && musicaAudioSource.clip != nuevaMusica)
        {
            CambiarMusica(nuevaMusica);
        }
    }

    void CambiarMusica(AudioClip nuevaMusica)
    {
        if (nuevaMusica == null) return;

        musicaAudioSource.Stop();
        
        musicaAudioSource.clip = nuevaMusica;
        musicaAudioSource.Play();
        
        Debug.Log($"Cambiando música a: {nuevaMusica.name}");
    }

    public void ReproducirEfecto(AudioClip efecto)
    {
        if (efecto != null && efectosAudioSource != null)
        {
            efectosAudioSource.PlayOneShot(efecto);
        }
    }

    // Métodos del Jugador
    public void ReproducirEfectoAtaqueJugador() => ReproducirEfecto(fxAtaqueJugador);
    public void ReproducirEfectoMovimientoJugador() => ReproducirEfecto(fxJugadorMoverse);
    public void ReproducirEfectoSaltoJugador() => ReproducirEfecto(fxJugadorSaltar);
    public void ReproducirEfectoMuerteJugador() => ReproducirEfecto(fxMorirJugador);
    
    // Métodos de los Gorgons
    public void ReproducirEfectoGorgons() => ReproducirEfecto(fxGorgons);
    public void ReproducirEfectoAtaqueGorgons() => ReproducirEfecto(fxAtaqueGorgons);
    public void ReproducirEfectoMovimientoGorgons() => ReproducirEfecto(fxGorgonsMoverse);
    public void ReproducirEfectoMuerteGorgons() => ReproducirEfecto(fxMorirGorgons);
    
    // Métodos de los Caballeros
    public void ReproducirEfectoCaballeros() => ReproducirEfecto(fxCaballeros);
    public void ReproducirEfectoMovimientoCaballeros() => ReproducirEfecto(fxCaballerosMoverse);
    public void ReproducirEfectoMuerteCaballero() => ReproducirEfecto(fxMorirCaballero);
    public void ReproducirEfectoAtaqueCaballero2() => ReproducirEfecto(fxAtaqueCaballero2);
    public void ReproducirEfectoAtaqueCaballero3() => ReproducirEfecto(fxAtaqueCaballero3);
    
    // Métodos de objetos
    public void ReproducirEfectoCofre() => ReproducirEfecto(fxCofres);
    public void ReproducirEfectoPuerta() => ReproducirEfecto(fxPuerta);

    public void CambiarVolumenMusica(float nuevoVolumen)
    {
        volumenMusica = Mathf.Clamp01(nuevoVolumen);
        if (musicaAudioSource != null)
            musicaAudioSource.volume = volumenMusica;
    }

    public void CambiarVolumenEfectos(float nuevoVolumen)
    {
        volumenEfectos = Mathf.Clamp01(nuevoVolumen);
        if (efectosAudioSource != null)
            efectosAudioSource.volume = volumenEfectos;
    }

    public void PausarMusica()
    {
        if (musicaAudioSource != null && musicaAudioSource.isPlaying)
            musicaAudioSource.Pause();
    }

    public void ReanudarMusica()
    {
        if (musicaAudioSource != null && !musicaAudioSource.isPlaying)
            musicaAudioSource.UnPause();
    }

    public void DetenerMusica()
    {
        if (musicaAudioSource != null)
            musicaAudioSource.Stop();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (musicaAudioSource != null)
            musicaAudioSource.volume = volumenMusica;
        
        if (efectosAudioSource != null)
            efectosAudioSource.volume = volumenEfectos;
    }
}