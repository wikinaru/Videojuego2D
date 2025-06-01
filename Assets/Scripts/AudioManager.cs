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
        
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        ConfigurarAudioSources();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        Debug.Log("AudioManager inicializado correctamente");
    }

    void ConfigurarAudioSources()
    {
        // Configurar AudioSource para música
        musicaAudioSource = gameObject.AddComponent<AudioSource>();
        musicaAudioSource.loop = true;
        musicaAudioSource.volume = volumenMusica;
        musicaAudioSource.playOnAwake = false;
        musicaAudioSource.priority = 64;

        // Configurar AudioSource para efectos
        efectosAudioSource = gameObject.AddComponent<AudioSource>();
        efectosAudioSource.loop = false;
        efectosAudioSource.volume = volumenEfectos;
        efectosAudioSource.playOnAwake = false;
        efectosAudioSource.priority = 128;
        
        Debug.Log("AudioSources configurados correctamente");
    }

    void Start()
    {
        StartCoroutine(InicializarMusicaConDelay());
    }

    IEnumerator InicializarMusicaConDelay()
    {
        yield return new WaitForSeconds(0.1f);
        
        escenaActual = SceneManager.GetActiveScene().name;
        Debug.Log($"Escena actual detectada: {escenaActual}");
        CambiarMusicaSegunEscena(escenaActual);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(CambiarMusicaConDelay(scene.name));
    }

    IEnumerator CambiarMusicaConDelay(string nombreEscena)
    {
        yield return new WaitForSeconds(0.1f);
        
        escenaActual = nombreEscena;
        Debug.Log($"Cambiando a escena: {escenaActual}");
        CambiarMusicaSegunEscena(escenaActual);
    }

    void CambiarMusicaSegunEscena(string nombreEscena)
    {
        AudioClip nuevaMusica = null;

        string escenaNormalizada = nombreEscena.ToLower();
        
        if (escenaNormalizada.Contains("inicio") || escenaNormalizada.Contains("menu") || escenaNormalizada == "0inicioscene")
        {
            nuevaMusica = bandaSonoraInicio;
            Debug.Log("Música de inicio seleccionada");
        }
        else if (escenaNormalizada.Contains("juego") || escenaNormalizada.Contains("nivel") || 
                 escenaNormalizada.Contains("gameplay") || escenaNormalizada == "1juegoscene")
        {
            nuevaMusica = bandaSonoraJuego;
            Debug.Log("Música de juego seleccionada");
        }
        else if (escenaNormalizada.Contains("final") || escenaNormalizada.Contains("gameover") || 
                 escenaNormalizada.Contains("victoria") || escenaNormalizada == "2finalscene")
        {
            nuevaMusica = bandaSonoraFinal;
            Debug.Log("Música final seleccionada");
        }
        else
        {
            Debug.LogWarning($"Escena no reconocida para música: {nombreEscena}");
  
            nuevaMusica = bandaSonoraJuego;
        }

        if (nuevaMusica != null)
        {
            CambiarMusica(nuevaMusica);
        }
        else
        {
            Debug.LogError("No se pudo asignar música para la escena: " + nombreEscena);
        }
    }

    void CambiarMusica(AudioClip nuevaMusica)
    {
        if (nuevaMusica == null)
        {
            Debug.LogError("El clip de música es null");
            return;
        }

        if (musicaAudioSource == null)
        {
            Debug.LogError("musicaAudioSource es null");
            return;
        }

        if (musicaAudioSource.clip != nuevaMusica)
        {
            musicaAudioSource.Stop();
            musicaAudioSource.clip = nuevaMusica;
            musicaAudioSource.volume = volumenMusica;
            musicaAudioSource.Play();
            
            Debug.Log($"Música cambiada a: {nuevaMusica.name}");
            Debug.Log($"¿Está reproduciéndose? {musicaAudioSource.isPlaying}");
        }
    }

    public void ReproducirEfecto(AudioClip efecto)
    {
        if (efecto == null)
        {
            Debug.LogWarning("El clip de efecto es null");
            return;
        }

        if (efectosAudioSource == null)
        {
            Debug.LogError("efectosAudioSource es null");
            return;
        }

        efectosAudioSource.PlayOneShot(efecto, volumenEfectos);
        Debug.Log($"Reproduciendo efecto: {efecto.name}");
    }

    // Métodos del Jugador
    public void ReproducirEfectoAtaqueJugador() 
    {
        Debug.Log("Intentando reproducir efecto de ataque del jugador");
        ReproducirEfecto(fxAtaqueJugador);
    }
    
    public void ReproducirEfectoMovimientoJugador() 
    {
        Debug.Log("Intentando reproducir efecto de movimiento del jugador");
        if (fxJugadorMoverse == null)
        {
            Debug.LogError("fxJugadorMoverse es null! Asigna el audio clip en el inspector.");
            return;
        }
        ReproducirEfecto(fxJugadorMoverse);
    }
    
    public void ReproducirEfectoSaltoJugador() 
    {
        Debug.Log("Intentando reproducir efecto de salto del jugador");
        ReproducirEfecto(fxJugadorSaltar);
    }
    
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

    [ContextMenu("Debug Audio Status")]
    public void DebugAudioStatus()
    {
        Debug.Log("=== ESTADO DEL AUDIO MANAGER ===");
        Debug.Log($"Escena actual: {escenaActual}");
        Debug.Log($"AudioManager Instance: {(Instance != null ? "OK" : "NULL")}");
        Debug.Log($"Música AudioSource: {(musicaAudioSource != null ? "OK" : "NULL")}");
        Debug.Log($"Efectos AudioSource: {(efectosAudioSource != null ? "OK" : "NULL")}");
        
        if (musicaAudioSource != null)
        {
            Debug.Log($"Música reproduciéndose: {musicaAudioSource.isPlaying}");
            Debug.Log($"Clip actual: {(musicaAudioSource.clip != null ? musicaAudioSource.clip.name : "NULL")}");
            Debug.Log($"Volumen música: {musicaAudioSource.volume}");
        }
        
        Debug.Log($"Audio Clips asignados:");
        Debug.Log($"- Banda sonora juego: {(bandaSonoraJuego != null ? bandaSonoraJuego.name : "NULL")}");
        Debug.Log($"- FX Jugador moverse: {(fxJugadorMoverse != null ? fxJugadorMoverse.name : "NULL")}");
        Debug.Log($"- FX Jugador saltar: {(fxJugadorSaltar != null ? fxJugadorSaltar.name : "NULL")}");
    }
}