using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Vida Jugador
    public static float barraVida;
    public static bool morir = false;
    
    //Configurar Enemigos
    public static Dictionary<string, ConfiguracionEnemigo> enemigos;
    
    public static GameManager Instance;
    
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

        enemigos = new Dictionary<string, ConfiguracionEnemigo>();

        enemigos["Gorgon1"] = new ConfiguracionEnemigo("Gorgon1", 5f);
        enemigos["Gorgon2"] = new ConfiguracionEnemigo("Gorgon2", 7f);
        enemigos["Gorgon3"] = new ConfiguracionEnemigo("Gorgon3", 7.5f);
        enemigos["Caballero2"] = new ConfiguracionEnemigo("Caballero2", 9f);
        enemigos["Caballero3"] = new ConfiguracionEnemigo("Caballero3", 10.5f);
    }

    // ===== MÉTODOS PARA EL JUGADOR =====
    public static void DamageDeJugador(float damage)
    {
        barraVida -= damage;
        
        if (barraVida <= 0)
        {
            barraVida = 0;
            morir = true;
            Instance.OnJugadorMuere();
        }
        
        Instance.OnJugadorDanado();
    }
    
    public static void CurarJugador(float curacion)
    {
        barraVida += curacion;
        if (barraVida > 10f) barraVida = 10f;
    }
    
    void OnJugadorDanado()
    {
        // Aquí puedes agregar efectos cuando el jugador recibe daño
        Debug.Log("Jugador dañado. Vida actual: " + barraVida);
    }
    
    void OnJugadorMuere()
    {
        Debug.Log("¡El jugador ha muerto!");
        // Reiniciar nivel, mostrar pantalla de game over, etc.
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

        foreach (var enemigo in enemigos.Values)
        {
            enemigo.vidaActual = enemigo.vidaMaxima;
        }
    }
    
    public static void ConfigurarVidaEnemigo(string tipo, float nuevaVida)
    {
        if (enemigos.ContainsKey(tipo))
        {
            enemigos[tipo].vidaMaxima = nuevaVida;
            enemigos[tipo].vidaActual = nuevaVida;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReiniciarSistemaVidas();
        }
    }
}