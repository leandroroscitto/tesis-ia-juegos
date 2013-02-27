using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using PathRuntime;

public class Generador_Mapa : MonoBehaviour {
   [System.Serializable]
   public class Parametros_Generador {
	  public int min_hab_tam = 3;
	  public int max_hab_tam = 4;
	  public float max_prop_h = 1.5f;
	  public float max_prop_v = 1.5f;
	  public int recursion = 8;
	  public int conexiones_extras = 0;
	  public int cantidad_objetivos = 2;
	  public LayerMask mascara_obstaculo;
   }

   [System.Serializable]
   public class Parametros_Visuales {
	  public Material mesh_material;
	  public Material piso_material;
	  public Font fuente_objetivos;
	  public Material fuente_material;
   }

   [System.Serializable]
   public class Parametros_Jugadores {
	  public int numero_jugadores;
	  public GameObject jugador;
	  public GameObject companero;
	  public Camara_3Persona camara;
   }

   [System.Serializable]
   public class Parametros_Tamano {
	  public int cantidad_zonas_x, cantidad_zonas_y;
	  public Vector3 escala;
   }

   // Parametros
   public Parametros_Generador parametros;
   public Parametros_Visuales visuales;
   public Parametros_Tamano tamano;
   public Parametros_Jugadores jugadores;

   public int seed;
   public bool random_seed;

   // Generacion
   [SerializeField, HideInInspector]
   public Tile tile_piso, tile_pared;
   [SerializeField, HideInInspector]
   private Mapa mapa;
   [SerializeField, HideInInspector]
   private GameObject mapa_objeto;
   private BSPTree arbol;

   public void reiniciarEscenario() {
	  if (random_seed) {
		 seed = (int)((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) % int.MaxValue);
		 Random.seed = seed;
	  }
	  else {
		 Random.seed = seed;
	  }

	  if (tile_piso == null) {
		 tile_piso = new Tile(Tile.TTile.PISO, true, ".");
	  }
	  else {
		 tile_piso.inicializar(Tile.TTile.PISO, true, ".");
	  }
	  if (tile_pared == null) {
		 tile_pared = new Tile(Tile.TTile.PARED, false, "#");
	  }
	  else {
		 tile_pared.inicializar(Tile.TTile.PARED, false, "#");
	  }

	  if (mapa != null) {
		 mapa.inicializarMapa(tamano.cantidad_zonas_x, tamano.cantidad_zonas_y, tamano.escala);
		 mapa.tamano_zona = tamano.escala;
		 mapa.mascara = parametros.mascara_obstaculo.value;

		 mapa.mesh_material = visuales.mesh_material;
		 mapa.piso_material = visuales.piso_material;

		 mapa.fuente_objetivos = visuales.fuente_objetivos;
		 mapa.fuente_material = visuales.fuente_material;
	  }
	  else {
		 mapa_objeto = new GameObject("Mapa");
		 mapa = mapa_objeto.AddComponent<Mapa>();

		 mapa.inicializarMapa(tamano.cantidad_zonas_x, tamano.cantidad_zonas_y, tamano.escala);
		 mapa.tamano_zona = tamano.escala;
		 mapa.mascara = parametros.mascara_obstaculo.value;

		 mapa.mesh_material = visuales.mesh_material;
		 mapa.piso_material = visuales.piso_material;

		 mapa.fuente_objetivos = visuales.fuente_objetivos;
		 mapa.fuente_material = visuales.fuente_material;
	  }

	  mapa.rellenarRectangulo(0, 0, tamano.cantidad_zonas_x - 1, tamano.cantidad_zonas_y - 1, tile_pared);
   }

   public void generarEscenario() {
	  reiniciarEscenario();

	  VisitadorHabitacion vhab = new VisitadorHabitacion(mapa, this);
	  VisitadorConexion vcon = new VisitadorConexion(mapa, this, parametros.conexiones_extras, vhab);

	  arbol = new BSPTree(0, 0, tamano.cantidad_zonas_x, tamano.cantidad_zonas_y);
	  arbol.dividirArbolRecursivo(
		  parametros.recursion,
		  parametros.max_hab_tam,
		  parametros.max_hab_tam,
		  parametros.max_prop_h,
		  parametros.max_prop_v
	  );

	  arbol.recorrerOrdenPorNivelInverso(vhab);
	  arbol.recorrerOrdenPorNivelInverso(vcon);

	  arbol.crearTXT("./arbol.txt");
	  mapa.crearTXT("./mapa.txt");

	  mapa.crearMesh();
	  mapa.crearObjetivos(parametros.cantidad_objetivos);
	  mapa.crearNavegacion();
	  mapa.crearJugadores(jugadores.numero_jugadores, jugadores.jugador, jugadores.companero, jugadores.camara);

	  UnityEditor.PrefabUtility.CreatePrefab("Assets/Prefabs/Mapa.prefab", mapa.gameObject, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);

	  //UnityEditor.Selection.activeGameObject = mapa.navigation_objeto;
   }

   public void borrarEscenario() {
	  if (mapa_objeto != null) {
		 mapa_objeto.GetComponent<Mapa>().borrar();
		 DestroyImmediate(mapa_objeto.GetComponent<Mapa>());
		 DestroyImmediate(mapa_objeto);
	  }
   }

   public void borrarWaypoints() {
	  ArrayList waypoints = new ArrayList(Navigation.Waypoints);
	  foreach (Waypoint way in waypoints) {
		 DestroyImmediate(way.gameObject);
	  }
   }
}
