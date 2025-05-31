using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public class ConfiguracionEnemigo
    {
        public string nombre;
        public float vidaMaxima;
        public float vidaActual;
        public Color colorDamage = Color.red;
        public float duracionEfectoDamage = 0.2f;
        
        public ConfiguracionEnemigo(string nom, float vida)
        {
            nombre = nom;
            vidaMaxima = vida;
            vidaActual = vida;
        }
    }

    public static float barraVida;
    public static bool morir = false;
    
    GameObject vida0, vida1, vida2, vida3;
    Image imagenVida0, imagenVida1, imagenVida2, imagenVida3;
    
    public Sprite corazonEntero;
    public Sprite corazonMedio;
    public Sprite corazonVacio;
    
    public static bool llaveObtenida = false;
    private Image imagenLlaveUI;
    
    //Configurar Enemigos
    public static Dictionary<string, ConfiguracionEnemigo> enemigos;
    
    public static GameManager Instance;
    private Animator animator;
    
    private bool actualizarUIPendiente = false;
    private bool muerteYaProcesada = false;
    
    // Variables para manejo de sonido idle de Gorgons
    private float tiempoUltimoSonidoIdleGorgons = 0f;
    private float intervalSonidoIdleGorgons = 4f;
    private float probabilidadSonidoIdleGorgons = 0.25f;
    
    // Variables para manejo de sonido idle de Caballeros
    private float tiempoUltimoSonidoIdleCaballeros = 0f;
    private float intervalSonidoIdleCaballeros = 5f;
    private float probabilidadSonidoIdleCaballeros = 0.3f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarSistema();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InicializarSistema()
    {
        barraVida = 10f;
        morir = false;
        llaveObtenida = false;
        muerteYaProcesada = false;

        vida0 = GameObject.Find("Vida0");
        vida1 = GameObject.Find("Vida1");
        vida2 = GameObject.Find("Vida2");
        vida3 = GameObject.Find("Vida3");

        if (vida0 != null) imagenVida0 = vida0.GetComponent<Image>();
        if (vida1 != null) imagenVida1 = vida1.GetComponent<Image>();
        if (vida2 != null) imagenVida2 = vida2.GetComponent<Image>();
        if (vida3 != null) imagenVida3 = vida3.GetComponent<Image>();

        GameObject llaveUIObj = GameObject.Find("Llave");
        if (llaveUIObj != null)
        {
            imagenLlaveUI = llaveUIObj.GetComponent<Image>();
        }

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            animator = jugador.GetComponent<Animator>();
        }

        enemigos = new Dictionary<string, ConfiguracionEnemigo>();
        enemigos["Gorgon1"] = new ConfiguracionEnemigo("Gorgon1", 5f);
        enemigos["Gorgon2"] = new ConfiguracionEnemigo("Gorgon2", 7f);
        enemigos["Gorgon3"] = new ConfiguracionEnemigo("Gorgon3", 7.5f);
        enemigos["Caballero2"] = new ConfiguracionEnemigo("Caballero2", 9f);
        enemigos["Caballero3"] = new ConfiguracionEnemigo("Caballero3", 10.5f);
        
        tiempoUltimoSonidoIdleGorgons = Time.time;
        tiempoUltimoSonidoIdleCaballeros = Time.time;
        
        ActualizarUICorazones();
        ActualizarUILlave();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarSistemaVidas();
        }
        
        if (actualizarUIPendiente)
        {
            actualizarUIPendiente = false;
            ActualizarUICorazones();
            ActualizarUILlave();
        }
        
        ManejarSonidoIdleGorgons();
        ManejarSonidoIdleCaballeros();
    }
    
    void ManejarSonidoIdleGorgons()
    {
        if (HayGorgonsVivos() && Time.time - tiempoUltimoSonidoIdleGorgons >= intervalSonidoIdleGorgons)
        {
            if (Random.Range(0f, 1f) <= probabilidadSonidoIdleGorgons)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ReproducirEfectoGorgons();
                }
            }
            tiempoUltimoSonidoIdleGorgons = Time.time;
        }
    }
    
    void ManejarSonidoIdleCaballeros()
    {
        if (HayCaballerosVivos() && Time.time - tiempoUltimoSonidoIdleCaballeros >= intervalSonidoIdleCaballeros)
        {
            if (Random.Range(0f, 1f) <= probabilidadSonidoIdleCaballeros)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.ReproducirEfectoCaballeros();
                }
            }
            tiempoUltimoSonidoIdleCaballeros = Time.time;
        }
    }
    
    bool HayGorgonsVivos()
    {
        GameObject[] gorgons = GameObject.FindGameObjectsWithTag("Enemigo");
        foreach (GameObject enemigo in gorgons)
        {
            if (enemigo.name.Contains("Gorgon"))
            {
                return true;
            }
        }
        return false;
    }
    
    bool HayCaballerosVivos()
    {
        GameObject[] caballeros = GameObject.FindGameObjectsWithTag("Enemigo");
        foreach (GameObject enemigo in caballeros)
        {
            if (enemigo.name.Contains("Caballero"))
            {
                return true;
            }
        }
        return false;
    }

    void ActualizarUICorazones()
    {
        ActualizarCorazon(imagenVida3, 7.5f, 10f);
        ActualizarCorazon(imagenVida2, 5f, 7.5f);
        ActualizarCorazon(imagenVida1, 2.5f, 5f);
        ActualizarCorazon(imagenVida0, 0f, 2.5f);
    }
    
    void ActualizarUILlave()
    {
        if (imagenLlaveUI != null)
        {
            imagenLlaveUI.gameObject.SetActive(llaveObtenida);
        }
    }
    
    void ActualizarCorazon(Image imagenCorazon, float vidaMinima, float vidaMaxima)
    {
        if (imagenCorazon == null) return;
        
        if (barraVida <= vidaMinima)
        {
            imagenCorazon.sprite = corazonVacio;
        }
        else if (barraVida >= vidaMaxima)
        {
            imagenCorazon.sprite = corazonEntero;
        }
        else
        {
            imagenCorazon.sprite = corazonMedio;
        }
    }

    // ===== MÉTODOS PARA EL JUGADOR =====
    public static void DamageDeJugador(float damage)
    {
        barraVida -= damage;
        
        if (barraVida <= 0)
        {
            barraVida = 0;
            if (!Instance.muerteYaProcesada)
            {
                morir = true;
                Instance.muerteYaProcesada = true;
                Instance.OnJugadorMuere();
            }
        }
        else
        {
            Instance.OnJugadorDanado();
        }
        
        Instance.actualizarUIPendiente = true;
    }
    
    public static void CurarJugador(float curacion)
    {
        barraVida += curacion;
        if (barraVida > 10f) barraVida = 10f;
        
        if (barraVida > 0 && morir)
        {
            morir = false;
            Instance.muerteYaProcesada = false;
        }
        
        Instance.actualizarUIPendiente = true;
    }

    public static void DanarJugador(GameObject jugador, float damage)
    {
        DamageDeJugador(damage);

        if (Instance != null)
        {
            Instance.StartCoroutine(Instance.EfectoDanoJugador(jugador));
        }
    }

    public static void ObtenerLlave()
    {
        llaveObtenida = true;
        Instance.actualizarUIPendiente = true;
        Debug.Log("Llave obtenida! UI actualizada.");
    }
    
    public static bool TieneLlave()
    {
        return llaveObtenida;
    }

    IEnumerator EfectoDanoJugador(GameObject jugador)
    {
        SpriteRenderer sr = jugador.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color colorOriginal = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            sr.color = colorOriginal;
        }
    }
    
    void OnJugadorDanado()
    {
        Debug.Log("Jugador dañado. Vida actual: " + barraVida);
    }
    
    void OnJugadorMuere()
    {
        if (animator != null)
            animator.SetBool("activarMuerte", true);
            
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ReproducirEfectoMuerteJugador();
        }
        
        Debug.Log("¡El jugador ha muerto!");
    }

    // ===== MÉTODOS PARA ENEMIGOS =====
    public static float ObtenerVidaEnemigo(string tipoEnemigo)
    {
        if (enemigos.ContainsKey(tipoEnemigo))
        {
            return enemigos[tipoEnemigo].vidaActual;
        }
        return 0f;
    }
    
    public static bool DanarEnemigo(GameObject enemigo, float damge)
    {
        string tipoEnemigo = ObtenerTipoEnemigo(enemigo);
        
        if (enemigos.ContainsKey(tipoEnemigo))
        {
            enemigos[tipoEnemigo].vidaActual -= damge;
            
            if (enemigos[tipoEnemigo].vidaActual <= 0)
            {
                Instance.OnEnemigoDerrotado(enemigo, tipoEnemigo);
                return true;
            }
            else
            {
                Instance.OnEnemigoDanado(enemigo, tipoEnemigo);
                return false;
            }
        }
        
        return false;
    }
    
    static string ObtenerTipoEnemigo(GameObject enemigo)
    {
        if (enemigo.name.Contains("Gorgon"))
        {
            if (enemigo.name.Contains("1")) return "Gorgon1";
            if (enemigo.name.Contains("2")) return "Gorgon2";
            if (enemigo.name.Contains("3")) return "Gorgon3";
        }
        else if (enemigo.name.Contains("Caballero"))
        {
            if (enemigo.name.Contains("2")) return "Caballero2";
            if (enemigo.name.Contains("3")) return "Caballero3";
        }
        
        return "";
    }
    
    void OnEnemigoDanado(GameObject enemigo, string tipo)
    {
        StartCoroutine(EfectoDanoEnemigo(enemigo, tipo));
        Debug.Log( tipo + " dañado. Vida restante: "  + enemigos[tipo].vidaActual);
    }
    
    void OnEnemigoDerrotado(GameObject enemigo, string tipo)
    {
        Debug.Log( tipo + " derrotado!");
        
        if (tipo.Contains("Gorgon"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ReproducirEfectoMuerteGorgons();
            }
        }
        else if (tipo.Contains("Caballero"))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ReproducirEfectoMuerteCaballero();
            }
        }
   
        enemigos[tipo].vidaActual = enemigos[tipo].vidaMaxima;
        
        StartCoroutine(EfectoMuerteEnemigo(enemigo));
    }
    
    IEnumerator EfectoDanoEnemigo(GameObject enemigo, string tipo)
    {
        SpriteRenderer sr = enemigo.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color colorOriginal = sr.color;
            sr.color = enemigos[tipo].colorDamage;
            yield return new WaitForSeconds(enemigos[tipo].duracionEfectoDamage);
            sr.color = colorOriginal;
        }
    }
    
    IEnumerator EfectoMuerteEnemigo(GameObject enemigo)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(enemigo);
    }

    // ===== MÉTODOS DE UTILIDAD =====
    public static void ReiniciarSistemaVidas()
    {
        barraVida = 10f;
        morir = false;
        llaveObtenida = false;
        Instance.muerteYaProcesada = false;

        foreach (var enemigo in enemigos.Values)
        {
            enemigo.vidaActual = enemigo.vidaMaxima;
        }

        if (Instance.animator != null)
        {
            Instance.animator.SetBool("activarMuerte", false);
        }

        Instance.actualizarUIPendiente = true;
    }
    
    public static void ConfigurarVidaEnemigo(string tipo, float nuevaVida)
    {
        if (enemigos.ContainsKey(tipo))
        {
            enemigos[tipo].vidaMaxima = nuevaVida;
            enemigos[tipo].vidaActual = nuevaVida;
        }
    }
}