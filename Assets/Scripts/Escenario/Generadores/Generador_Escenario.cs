using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using PathRuntime;

public class Generador_Escenario : MonoBehaviour {
   public static float radio_cercania_waypoint = 2.5f;

   [Serializable]
   public class Parametros_Mapa {
	  public int min_hab_tam = 3;
	  public int max_hab_tam = 4;
	  public float max_prop_h = 1.5f;
	  public float max_prop_v = 1.5f;
	  public int recursion = 8;
	  public int conexiones_extras = 0;
   }

   [Serializable]
   public class Parametros_Tamano {
	  public int cantidad_zonas_x, cantidad_zonas_y;
	  public Vector3 tamano_tile;
   }

   [Serializable]
   public class Parametros_Navegacion {
	  public LayerMask mascara_obstaculo;
   }

   [Serializable]
   public class Parametros_Objetivos {
	  public int cantidad_objetivos = 2;
   }

   [Serializable]
   public class Parametros_Jugadores {
	  public int numero_jugadores;
	  public GameObject jugador;
	  public GameObject companero;
	  public Camara_3Persona camara;
   }

   [Serializable]
   public class Parametros_Visuales {
	  public Material mesh_material;
	  public Material piso_material;
	  public Font fuente_objetivos;
	  public Material fuente_material;
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
   public GameObject generacion_prefab;
   public GameObject datos_objeto;

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
	  generador_navegacion.inicializar();

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
	  generador_mdp.inicializar(generador_mapa.mapa, generador_jugadores.jugadores, generador_acciones.acciones, generador_objetivos.objetivos);
   }

   public void generarEscenario() {
	  reiniciarEscenario();

	  generador_mapa.generar(parametros_mapa);
	  generador_objetivos.generar(parametros_objetivos.cantidad_objetivos);
	  generador_navegacion.generar();
	  generador_jugadores.generar(parametros_jugadores.numero_jugadores, parametros_jugadores.jugador, parametros_jugadores.companero, parametros_jugadores.camara);
	  generador_acciones.generar();
	  generador_mdp.generar();
   }

   public void borrarEscenario() {
	  reiniciarEscenario();

	  generador_mapa.borrar();
	  generador_navegacion.borrar();
	  generador_objetivos.borrar();
	  generador_jugadores.borrar();
	  generador_acciones.borrar();
	  generador_mdp.borrar();

	  DestroyImmediate(escenario_objeto);
   }

   // Serializacion
   public void cargarDisplay() {
	  if (datos_objeto == null) {
		 datos_objeto = new GameObject("Display datos");
		 datos_objeto.AddComponent<DisplayDatos>();
	  }
	  DisplayDatos display = datos_objeto.GetComponent<DisplayDatos>();
	  display.transform.parent = transform.parent;

	  display.Mapa = generador_mapa.mapa;
	  display.Acciones = generador_acciones.acciones;
	  display.Jugadores = generador_jugadores.jugadores;
	  display.Objetivos = generador_objetivos.objetivos;
   }

   public void guardarDatos() {
	  if (escenario_objeto != null) {
		 Serializador serializador = new Serializador();
		 Objeto_Serializable datos = new Objeto_Serializable();
		 datos.Resolucion_MDP = generador_mdp.resolucion_mdp;

		 datos.estado = new Estado(4, generador_mapa.mapa);
		 datos.nodo_estado = new Nodo_Estado(5, datos.estado);
		 datos.mapa = generador_mapa.mapa;

		 serializador.Serializar("./Assets/Data/datos.bin", datos);

		 generacion_prefab = UnityEditor.PrefabUtility.CreatePrefab("Assets/Data/generacion.prefab", transform.parent.gameObject, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
	  }

	  if (datos_objeto == null) {
		 datos_objeto = new GameObject("Display datos");
		 datos_objeto.AddComponent<DisplayDatos>();
	  }
	  DisplayDatos display = datos_objeto.GetComponent<DisplayDatos>();
	  display.transform.parent = transform.parent;

	  display.Mapa = null;
	  display.Acciones = null;
	  display.Jugadores = null;
	  display.Objetivos = null;
   }

   public void cargarDatos() {
	  UnityEditor.PrefabUtility.ReconnectToLastPrefab(transform.parent.gameObject);
	  UnityEditor.PrefabUtility.RevertPrefabInstance(transform.parent.gameObject);
	  transform.parent.name = "Generacion";

	  Serializador serializador = new Serializador();
	  Objeto_Serializable datos = serializador.Deserializar("./Assets/Data/datos.bin");

	  if (datos_objeto == null) {
		 datos_objeto = new GameObject("Display datos");
		 datos_objeto.AddComponent<DisplayDatos>();
	  }
	  DisplayDatos display = datos_objeto.GetComponent<DisplayDatos>();
	  display.transform.parent = transform.parent;

	  display.Mapa = datos.Mapa;
	  display.Acciones = datos.Acciones;
	  display.Jugadores = datos.Jugadores;
	  display.Objetivos = datos.Objetivos;

	  generador_mapa.mapa = datos.Mapa;
	  generador_objetivos.objetivos = datos.Objetivos;
	  generador_jugadores.jugadores = datos.Jugadores;
	  generador_acciones.acciones = datos.Acciones;
	  generador_mdp.resolucion_mdp = datos.Resolucion_MDP;
	  generador_mdp.arbol_estados = datos.Arbol_Estados;
   }
}