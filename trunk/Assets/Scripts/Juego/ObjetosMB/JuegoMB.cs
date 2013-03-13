using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using PathRuntime;

public class JuegoMB : MonoBehaviour {
   // Representacion
   private GameObject escenario;

   // Listas
   [NonSerialized]
   public List<ObjetivoMB> objetivos;
   [NonSerialized]
   public List<JugadorMB> jugadores;
   [NonSerialized]
   public List<Accion> acciones;

   // Estructuras auxiliares
   // <jugador_id, ...>
   [NonSerialized]
   private Dictionary<int, Dictionary<Waypoint, Dictionary<Waypoint, Accion>>> acciones_dict;

   // Estados
   [NonSerialized]
   public Nodo_Estado nodo_estado_actual;
   [NonSerialized]
   public Arbol_Estados arbol_estados;

   // Operaciones
   void OnGUI() {
	  GUILayout.Label("Estado Actual: " + nodo_estado_actual.estado_actual);
	  string objetivos_cumplidos, objetivos_no_cumplidos;
	  objetivos_cumplidos = objetivos_no_cumplidos = "";
	  foreach (ObjetivoMB objetivomb in objetivos) {
		 if (nodo_estado_actual.estado_actual.objetivos_cumplidos.Contains(objetivomb.objetivo.id)) {
			objetivos_cumplidos += objetivomb.objetivo.nombre + ", ";
		 }
		 else if (nodo_estado_actual.estado_actual.objetivos_no_cumplidos.Contains(objetivomb.objetivo.id)) {
			objetivos_no_cumplidos += objetivomb.objetivo.nombre + ", ";
		 }
		 else {
			throw new Exception("Objetivo no determinado");
		 }
	  }
	  GUILayout.Label("Objetivos cumplidos: " + objetivos_cumplidos);
	  GUILayout.Label("Objetivos no cumplidos: " + objetivos_no_cumplidos);

	  GUILayout.Space(100);

	  string acciones_string = "";
	  int cantidad = Mathf.Min(10, jugadores[0].jugador.acciones.Count);
	  KeyValuePair<float, Accion>[] enumerador = jugadores[0].jugador.acciones.ToArray<KeyValuePair<float, Accion>>();
	  for (int i = jugadores[0].jugador.acciones.Count - cantidad; i < jugadores[0].jugador.acciones.Count; i++) {
		 KeyValuePair<float, Accion> tiempo_accion = enumerador[i];
		 acciones_string = "(" + tiempo_accion.Key + ") " + tiempo_accion.Value.origen + " => " + tiempo_accion.Value.destino + "\n" + acciones_string;
	  }
	  GUILayout.Label("Acciones:");
	  GUILayout.Label(acciones_string);
   }

   public void inicializar(List<ObjetivoMB> objs, List<JugadorMB> jugs, List<Accion> accs, Arbol_Estados arb_ests) {
	  objetivos = objs;
	  jugadores = jugs;
	  acciones = accs;
	  arbol_estados = arb_ests;
   }

   void Start() {
	  escenario = GameObject.Find("Escenario");
	  UnityEditor.PrefabUtility.ReconnectToLastPrefab(escenario);
	  UnityEditor.PrefabUtility.RevertPrefabInstance(escenario);

	  Serializador serializador = new Serializador();
	  Objeto_Serializable datos = serializador.Deserializar("./Assets/Data/datos.bin");
	  datos.Arbol_Estados.generarEstadosDiccionario();

	  objetivos = new List<ObjetivoMB>();
	  foreach (Objetivo objetivo in datos.Objetivos) {
		 objetivos.Add(objetivo.objetivo_mb);
	  }
	  jugadores = new List<JugadorMB>();
	  foreach (Jugador jugador in datos.Jugadores) {
		 jugadores.Add(jugador.jugador_mb);
	  }
	  acciones = datos.Acciones;
	  generarDiccionarioAcciones();

	  arbol_estados = datos.Arbol_Estados;
	  nodo_estado_actual = arbol_estados.nodo_estado_inicial;
   }

   void Update() {
	  registrarAccionesJugadores();
	  actualizarEstadoActual();
   }

   // Estados
   private void actualizarEstadoActual() {
	  Vector3[] posicion_jugadores = new Vector3[jugadores.Count];
	  for (int i = 0; i < jugadores.Count; i++) {
		 posicion_jugadores[i] = jugadores[i].jugador.posicion;
	  }

	  HashSet<int> objetivos_cumplidos = new HashSet<int>();
	  HashSet<int> objetivos_no_cumplidos = new HashSet<int>();
	  foreach (ObjetivoMB objetivomb in objetivos) {
		 if (objetivomb.objetivo.cumplido) {
			objetivos_cumplidos.Add(objetivomb.objetivo.id);
		 }
		 else {
			objetivos_no_cumplidos.Add(objetivomb.objetivo.id);
		 }
	  }

	  nodo_estado_actual = arbol_estados.getEstadoActual(posicion_jugadores, objetivos_cumplidos, objetivos_no_cumplidos);
   }

   // Acciones
   private void registrarAccionesJugadores() {
	  foreach (JugadorMB jugadormb in jugadores) {
		 Waypoint waypoint_previo = Navigation.GetNearestNode(nodo_estado_actual.estado_actual.posicion_jugadores[jugadormb.jugador.id]);
		 Waypoint waypoint_actual = jugadormb.jugador.waypointMasCercano();
		 Waypoint waypoint_actual_conectado = jugadormb.jugador.waypointMasCercano(waypoint_previo);

		 if (waypoint_actual != waypoint_actual_conectado) {
			jugadormb.jugador.registrarAccion(Time.time - Time.deltaTime / 2, acciones_dict[jugadormb.jugador.id][waypoint_previo][waypoint_actual_conectado]);
			jugadormb.jugador.registrarAccion(Time.time, acciones_dict[jugadormb.jugador.id][waypoint_actual_conectado][waypoint_actual]);
		 }
		 else {
			jugadormb.jugador.registrarAccion(Time.time, acciones_dict[jugadormb.jugador.id][waypoint_previo][waypoint_actual]);
		 }
	  }
   }

   private void generarDiccionarioAcciones() {
	  acciones_dict = new Dictionary<int, Dictionary<Waypoint, Dictionary<Waypoint, Accion>>>();
	  foreach (JugadorMB jugadormb in jugadores) {
		 acciones_dict.Add(jugadormb.jugador.id, new Dictionary<Waypoint, Dictionary<Waypoint, Accion>>());
		 foreach (Accion accion in acciones) {
			if (accion.actor_id == jugadormb.jugador.id) {
			   Dictionary<Waypoint, Accion> dict = null;
			   if (acciones_dict[jugadormb.jugador.id].TryGetValue(accion.origen, out dict)) {
				  dict.Add(accion.destino, accion);
			   }
			   else {
				  dict = new Dictionary<Waypoint, Accion>();
				  dict.Add(accion.destino, accion);
				  acciones_dict[jugadormb.jugador.id].Add(accion.origen, dict);
			   }
			}
		 }
	  }
   }
}