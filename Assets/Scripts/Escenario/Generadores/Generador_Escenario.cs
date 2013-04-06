using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using PathRuntime;

[ExecuteInEditMode]
public class Generador_Escenario : MonoBehaviour {
   public static float radio_cercania_waypoint = 2.5f;

   [Serializable]
   public class Parametros_Mapa {
	  public int min_hab_tam = 3;
	  public int max_hab_tam = 4;
	  public float max_prop_h = 1.5f;
	  public float max_prop_v = 1.5f;
	  public int recursion = 8;
	  public int conexiones_extras = 1;
   }

   [Serializable]
   public class Parametros_Tamano {
	  public int cantidad_zonas_x = 20;
	  public int cantidad_zonas_y = 20;
	  public Vector3 tamano_tile = new Vector3(2, 15, 2);
   }

   [Serializable]
   public class Parametros_Navegacion {
	  public LayerMask mascara_obstaculo;
	  public float maximo_angulo_optimizacion = 45;
   }

   [Serializable]
   public class Parametros_Objetivos {
	  public int cantidad_objetivos = 2;
   }

   [Serializable]
   public class Parametros_Jugadores {
	  public int numero_jugadores = 2;
	  public GameObject jugador;
	  public GameObject companero;
   }

   [Serializable]
   public class Parametros_Visuales {
	  public float tamano_radar = 0.35f;
	  public Material mesh_material;
	  public Material piso_material;
	  public Font fuente_objetivos;
	  public Material fuente_material;
	  public GameObject objetivo_efecto;
   }

   // Parametros
   public Parametros_Mapa parametros_mapa;
   public Parametros_Tamano parametros_tamano;
   public Parametros_Navegacion parametros_navegacion;
   public Parametros_Jugadores parametros_jugadores;
   public Parametros_Objetivos parametros_objetivos;
   public Parametros_Visuales parametros_visuales;

   public int seed;
   public bool random_seed;

   // Generadores
   [HideInInspector]
   public Generador_Mapa generador_mapa;
   [HideInInspector]
   public Generador_Navegacion generador_navegacion;
   [HideInInspector]
   public Generador_Objetivos generador_objetivos;
   [HideInInspector]
   public Generador_Jugadores generador_jugadores;
   [HideInInspector]
   public Generador_Acciones generador_acciones;
   [HideInInspector]
   public Generador_MDP generador_mdp;

   // Representacion
   public GameObject escenario_objeto;
   public GameObject juego_objeto;

   // Gizmos
   void OnDrawGizmos() {
	  if (generador_mapa != null && generador_mapa.mapa != null) {
		 Mapa mapa = generador_mapa.mapa;
		 Gizmos.color = Color.black;
		 Gizmos.DrawCube(Vector3.right * (mapa.offsetx - mapa.ancho / 2 + 1) + Vector3.forward * (mapa.offsety - mapa.largo / 2 + 1) + Vector3.up * -1, Vector3.right * mapa.ancho + Vector3.forward * mapa.largo + Vector3.up * 2);

		 Gizmos.color = Color.Lerp(Color.magenta, Color.black, 0.5f);
		 foreach (Generador_Mapa.Habitacion habitacion in generador_mapa.habitaciones) {
			Gizmos.DrawWireCube(Vector3.right * (mapa.offsetx - mapa.tamano_tile.x * (habitacion.x1 + habitacion.x2) / 2) + Vector3.forward * (mapa.offsety - mapa.tamano_tile.z * (habitacion.y1 + habitacion.y2) / 2) + Vector3.up, Vector3.right * habitacion.ancho * mapa.tamano_tile.x + Vector3.forward * habitacion.largo * mapa.tamano_tile.z + Vector3.up * 2);
		 }

		 Gizmos.color = Color.Lerp(Color.cyan, Color.black, 0.5f);
		 foreach (Generador_Mapa.Conexion conexion in generador_mapa.conexiones_horizontales) {
			Gizmos.DrawWireCube(Vector3.right * (mapa.offsetx - mapa.tamano_tile.x * (conexion.x1 + conexion.x2) / 2) + Vector3.forward * (mapa.offsety - mapa.tamano_tile.z * (conexion.y1 + conexion.y2) / 2) + Vector3.up, Vector3.right * conexion.ancho * mapa.tamano_tile.x + Vector3.forward * conexion.largo * mapa.tamano_tile.z + Vector3.up * 2);
		 }

		 foreach (Generador_Mapa.Conexion conexion in generador_mapa.conexiones_verticales) {
			Gizmos.DrawWireCube(Vector3.right * (mapa.offsetx - mapa.tamano_tile.x * (conexion.x1 + conexion.x2) / 2) + Vector3.forward * (mapa.offsety - mapa.tamano_tile.z * (conexion.y1 + conexion.y2) / 2) + Vector3.up, Vector3.right * conexion.ancho * mapa.tamano_tile.x + Vector3.forward * conexion.largo * mapa.tamano_tile.z + Vector3.up * 2);
		 }
	  }

	  /*
	  if (generador_objetivos != null && generador_objetivos.objetivos != null) {
		 foreach (Objetivo objetivo in generador_objetivos.objetivos) {
			Gizmos.color = Color.white;
			Gizmos.DrawSphere(objetivo.posicion, 1.25f);
		 }
	  }*/
   }

   // Generacion
   private void determinarSeed() {
	  if (random_seed) {
		 seed = (int)((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % int.MaxValue);
		 Random.seed = seed;
	  }
	  else {
		 Random.seed = seed;
	  }
   }

   public void reiniciarEscenario() {
	  if (escenario_objeto == null) {
		 escenario_objeto = new GameObject("Escenario");
		 escenario_objeto.transform.parent = transform.parent;
	  }

	  determinarSeed();

	  // Generador Mapa
	  if (generador_mapa == null) {
		 generador_mapa = gameObject.AddComponent<Generador_Mapa>();
	  }
	  generador_mapa.inicializar(parametros_tamano.cantidad_zonas_x, parametros_tamano.cantidad_zonas_y, parametros_tamano.tamano_tile, parametros_navegacion.mascara_obstaculo, parametros_visuales.mesh_material, parametros_visuales.piso_material);

	  // Generador Navegacion
	  if (generador_navegacion == null) {
		 generador_navegacion = gameObject.AddComponent<Generador_Navegacion>();
	  }
	  generador_navegacion.inicializar(parametros_navegacion.maximo_angulo_optimizacion);

	  // Generador Objetivos
	  if (generador_objetivos == null) {
		 generador_objetivos = gameObject.AddComponent<Generador_Objetivos>();
	  }
	  generador_objetivos.inicializar(parametros_visuales.fuente_material, parametros_visuales.fuente_objetivos);

	  // Generador Jugadores
	  if (generador_jugadores == null) {
		 generador_jugadores = gameObject.AddComponent<Generador_Jugadores>();
	  }
	  generador_jugadores.inicializar();

	  // Generador Acciones
	  if (generador_acciones == null) {
		 generador_acciones = gameObject.AddComponent<Generador_Acciones>();
	  }
	  generador_acciones.inicializar();

	  // Generador MDP
	  if (generador_mdp == null) {
		 generador_mdp = gameObject.AddComponent<Generador_MDP>();
	  }
	  generador_mdp.inicializar(generador_jugadores.jugadores, generador_acciones.acciones, generador_objetivos.objetivos);
   }

   public void generarEscenario() {
	  reiniciarEscenario();

	  generador_mapa.generar(parametros_mapa);
	  generador_objetivos.generar(parametros_objetivos.cantidad_objetivos, parametros_visuales.objetivo_efecto);
	  generador_navegacion.generar();
	  generador_jugadores.generar(parametros_jugadores.numero_jugadores, parametros_jugadores.jugador, parametros_jugadores.companero);
	  generador_acciones.generar();
	  generador_mdp.generar();

	  cargarJuego();
   }

   public void borrarEscenario() {
	  if (generador_mapa != null) {
		 generador_mapa.borrar();
	  }
	  if (generador_navegacion != null) {
		 generador_navegacion.borrar();
	  }
	  if (generador_objetivos != null) {
		 generador_objetivos.borrar();
	  }
	  if (generador_jugadores != null) {
		 generador_jugadores.borrar();
	  }
	  if (generador_acciones != null) {
		 generador_acciones.borrar();
	  }
	  if (generador_mdp != null) {
		 generador_mdp.borrar();
	  }

	  DestroyImmediate(generador_mapa);
	  DestroyImmediate(generador_navegacion);
	  DestroyImmediate(generador_objetivos);
	  DestroyImmediate(generador_jugadores);
	  DestroyImmediate(generador_acciones);
	  DestroyImmediate(generador_mdp);

	  DestroyImmediate(escenario_objeto);
   }

   // Serializacion
   public void guardarDatos() {
	  if (escenario_objeto != null) {
		 Serializador serializador = new Serializador();
		 Objeto_Serializable datos = new Objeto_Serializable();
		 datos.Resolucion_MDP = generador_mdp.resolucion_mdp;
		 datos.Mapa = generador_mapa.mapa;

		 serializador.Serializar("./Assets/Data/datos.bin", datos);

		 UnityEditor.PrefabUtility.CreatePrefab("Assets/Data/generacion.prefab", transform.parent.gameObject, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
	  }
   }

   public void cargarDatos() {
	  UnityEditor.PrefabUtility.ReconnectToLastPrefab(transform.parent.gameObject);
	  UnityEditor.PrefabUtility.RevertPrefabInstance(transform.parent.gameObject);
	  transform.parent.name = "Generacion";

	  Serializador serializador = new Serializador();
	  Objeto_Serializable datos = serializador.Deserializar("./Assets/Data/datos.bin");
	  datos.Arbol_Estados.generarEstadosDiccionario();

	  generador_mapa.mapa = datos.Mapa;
	  generador_objetivos.objetivos = datos.Objetivos;
	  generador_jugadores.jugadores = datos.Jugadores;
	  generador_acciones.acciones = datos.Acciones;
	  generador_mdp.resolucion_mdp = datos.Resolucion_MDP;
	  generador_mdp.arbol_estados = datos.Arbol_Estados;

	  cargarJuego();
   }

   // Juego
   public void cargarJuego() {
	  List<ObjetivoMB> objetivos = generador_objetivos.objetivos_mb;
	  List<JugadorMB> jugadores = generador_jugadores.jugadores_mb;
	  List<Accion> acciones = generador_acciones.acciones;
	  Arbol_Estados arbol_estado = generador_mdp.arbol_estados;
	  MDP<Nodo_Estado, Accion, Objetivo, ResolucionMDP.TransicionJuego, ResolucionMDP.RecompensaJuego> mdp = generador_mdp.resolucion_mdp.mdp;

	  juego_objeto = GameObject.Find("Juego");
	  if (juego_objeto == null) {
		 juego_objeto = new GameObject("Juego");
	  }
	  else {
		 if (juego_objeto.GetComponent<JuegoMB>() != null) {
			DestroyImmediate(juego_objeto.GetComponent<JuegoMB>());
		 }
	  }
	  juego_objeto.AddComponent<JuegoMB>().inicializar(objetivos, jugadores, acciones, arbol_estado, mdp);

	  GameObject camara_top = GameObject.FindGameObjectWithTag("Camara Top");
	  if (camara_top == null) {
		 camara_top = new GameObject("Radar");
		 camara_top.AddComponent<Camera>();
	  }
	  camara_top.transform.position = Vector3.up * 25;
	  camara_top.transform.eulerAngles = Vector3.right * 90;
	  camara_top.tag = "Camara Top";

	  camara_top.camera.depth = 1;
	  camara_top.camera.farClipPlane = 50;
	  camara_top.camera.orthographic = true;
	  camara_top.camera.orthographicSize = Mapa.Mapa_Instancia.cant_y;

	  float proporcion_mapa = Mapa.Mapa_Instancia.cant_x * 1f / Mapa.Mapa_Instancia.cant_y;
	  float proporcion_pantalla = 9f / 16f;

	  float ancho = proporcion_mapa * proporcion_pantalla * parametros_visuales.tamano_radar;
	  float largo = proporcion_mapa * 1f / proporcion_mapa * parametros_visuales.tamano_radar;
	  camara_top.camera.rect = new Rect(1 - ancho, 1 - largo, ancho, largo);
   }
}