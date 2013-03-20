using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using PathRuntime;
using UnitySteer;
using UnitySteer.Helpers;

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

   // <tiempo, estado>
   [NonSerialized]
   public SortedDictionary<float, Nodo_Estado> historial_estados;
   [NonSerialized]
   public Nodo_Estado nodo_estado_actual;
   [NonSerialized]
   public Nodo_Estado nodo_estado_previo;
   [NonSerialized]
   public Arbol_Estados arbol_estados;

   // Inferencia de objetivos
   public int profundidad_acciones = 4;
   public float factor_descuento = 0.95f;

   // MDP
   public MDP<Nodo_Estado, Accion, Objetivo, ResolucionMDP.TransicionJuego, ResolucionMDP.RecompensaJuego> mdp;

   // Operaciones
   public void inicializar(List<ObjetivoMB> objs, List<JugadorMB> jugs, List<Accion> accs, Arbol_Estados arb_ests, MDP<Nodo_Estado, Accion, Objetivo, ResolucionMDP.TransicionJuego, ResolucionMDP.RecompensaJuego> mdp_in) {
	  objetivos = objs;
	  jugadores = jugs;
	  acciones = accs;
	  arbol_estados = arb_ests;
	  mdp = mdp_in;

	  historial_estados = new SortedDictionary<float, Nodo_Estado>();
   }

   void OnGUI() {
	  // Informacion de estado actual
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

	  GUILayout.Space(5);

	  // Informacion de cumplimiento de objetivos
	  GUILayout.Label("Objetivos en radio:");
	  foreach (JugadorMB jugadormb in jugadores) {
		 foreach (ObjetivoMB objetivomb in objetivos) {
			if (objetivomb.radar.Vehicles.Contains(jugadormb.gameObject.GetComponent<Vehicle>())) {
			   GUILayout.Label("Jugador: " + jugadormb.name + ", Objetivo " + objetivomb.objetivo.nombre);
			}
		 }
	  }

	  GUILayout.Space(5);

	  // Inferencia de objetivo para jugador humano
	  float[] valor_objetivo;
	  ObjetivoMB objetivomb_inferido = inferirObjetivo(jugadores[0], profundidad_acciones, factor_descuento, out valor_objetivo);
	  if (objetivomb_inferido != null) {
		 GUILayout.Label("Objetivo inferido: " + objetivomb_inferido.objetivo.nombre);
		 string valor_string = "";
		 foreach (ObjetivoMB objetivomb in objetivos) {
			valor_string += "[" + objetivomb.objetivo.nombre + "] = " + valor_objetivo[objetivomb.objetivo.id] + ", ";
		 }
		 GUILayout.Label("Valores: " + valor_string);
	  }
	  else {
		 GUILayout.Label("Objetivo inferido: ninguno");
	  }

	  GUILayout.Space(5);

	  // Historial de estados
	  string estados_string = "";
	  int cantidad_estados = Mathf.Min(10, historial_estados.Count);
	  KeyValuePair<float, Nodo_Estado>[] enumerador_estados = historial_estados.ToArray<KeyValuePair<float, Nodo_Estado>>();
	  for (int i = historial_estados.Count - cantidad_estados; i < historial_estados.Count; i++) {
		 KeyValuePair<float, Nodo_Estado> tiempo_estado = enumerador_estados[i];
		 estados_string = "(" + Mathf.Round(tiempo_estado.Key * 100) / 100f + ") => " + tiempo_estado.Value.estado_actual + "\n" + estados_string;
	  }
	  GUILayout.Label("Historial de estados:");
	  GUILayout.Label(estados_string);

	  GUILayout.Space(5);

	  // Historial de acciones para jugador humano
	  string acciones_string = "";
	  int cantidad = Mathf.Min(10, jugadores[0].jugador.acciones.Count);
	  KeyValuePair<float, Accion>[] enumerador = jugadores[0].jugador.acciones.ToArray<KeyValuePair<float, Accion>>();
	  for (int i = jugadores[0].jugador.acciones.Count - cantidad; i < jugadores[0].jugador.acciones.Count; i++) {
		 KeyValuePair<float, Accion> tiempo_accion = enumerador[i];
		 acciones_string = "(" + Mathf.Round(tiempo_accion.Key * 100) / 100f + ") " + tiempo_accion.Value.origen + " => " + tiempo_accion.Value.destino + "\n" + acciones_string;
	  }
	  GUILayout.Label("Acciones:");
	  GUILayout.Label(acciones_string);
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
	  mdp = datos.MDP;

	  nodo_estado_actual = arbol_estados.nodo_estado_inicial;
	  nodo_estado_previo = arbol_estados.nodo_estado_inicial;

	  historial_estados = new SortedDictionary<float, Nodo_Estado>();
	  registarEstado(0, nodo_estado_actual);
   }

   void Update() {
	  actualizarEstadoActual();
	  registrarAccionesJugadores();
   }

   // Estados
   private void actualizarEstadoActual() {
	  Dictionary<int, Vector3> posicion_jugadores_waypoints = new Dictionary<int, Vector3>(jugadores.Count);
	  for (int i = 0; i < jugadores.Count; i++) {
		 Waypoint waypoint = Navigation.GetNearestNode(jugadores[i].jugador.posicion);
		 if (waypoint.tag == "Objetivo") {
			ObjetivoMB objetivomb = Objetivo.mapeo_waypoint_objetivo[waypoint];
			if (objetivomb.radar.Vehicles.Contains(jugadores[i].gameObject.GetComponent<Vehicle>())) {
			   posicion_jugadores_waypoints.Add(i, waypoint.Position);
			}
			else {
			   Waypoint waypoint_previo = null;
			   if (nodo_estado_actual != null) {
				  waypoint_previo = Navigation.GetNearestNode(nodo_estado_actual.estado_actual.posicion_jugadores[i]);
			   }
			   else {
				  waypoint_previo = jugadores[i].jugador.waypointMasCercanoNoObjetivo();
				  Debug.LogWarning("Aca");
			   }
			   posicion_jugadores_waypoints.Add(i, jugadores[i].jugador.waypointMasCercanoNoObjetivo(waypoint_previo).Position);
			}
		 }
		 else {
			posicion_jugadores_waypoints.Add(i, waypoint.Position);
		 }
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

	  nodo_estado_previo = nodo_estado_actual;
	  nodo_estado_actual = arbol_estados.getEstadoActual(posicion_jugadores_waypoints, objetivos_cumplidos, objetivos_no_cumplidos);
   }

   private void registarEstado(float tiempo, Nodo_Estado nodo_estado) {
	  if (historial_estados.Count > 0) {
		 KeyValuePair<float, Nodo_Estado> ultimo_estado = historial_estados.Last<KeyValuePair<float, Nodo_Estado>>();
		 if (nodo_estado == ultimo_estado.Value) {
			historial_estados.Remove(ultimo_estado.Key);
		 }
	  }
	  historial_estados.Add(tiempo, nodo_estado);
   }

   // Acciones
   private void registrarAccionesJugadores() {
	  List<Accion> acciones_realizadas = new List<Accion>();
	  if (nodo_estado_actual.id != nodo_estado_previo.id) {
		 Queue<KeyValuePair<Nodo_Estado, List<Accion>>> cola_estados = new Queue<KeyValuePair<Nodo_Estado, List<Accion>>>();
		 cola_estados.Enqueue(new KeyValuePair<Nodo_Estado, List<Accion>>(nodo_estado_previo, new List<Accion>()));
		 KeyValuePair<Nodo_Estado, List<Accion>> nodo_estado_aux;
		 do {
			nodo_estado_aux = cola_estados.Dequeue();
			for (int i = 0; i < nodo_estado_aux.Key.estados_hijos.Count; i++) {
			   if (nodo_estado_aux.Key.estados_hijos[i].id == nodo_estado_actual.id) {
				  Accion accion = nodo_estado_aux.Key.acciones_hijos[i];
				  nodo_estado_aux = new KeyValuePair<Nodo_Estado, List<Accion>>(nodo_estado_aux.Key.estados_hijos[i], nodo_estado_aux.Value);
				  nodo_estado_aux.Value.Add(accion);
				  break;
			   }
			   else {
				  List<Accion> acciones_padre = new List<Accion>(nodo_estado_aux.Value);
				  acciones_padre.Add(nodo_estado_aux.Key.acciones_hijos[i]);
				  cola_estados.Enqueue(new KeyValuePair<Nodo_Estado, List<Accion>>(nodo_estado_aux.Key.estados_hijos[i], acciones_padre));
			   }
			}
		 } while (nodo_estado_aux.Key.id != nodo_estado_actual.id);

		 acciones_realizadas = nodo_estado_aux.Value;
	  }
	  else {
		 // No hubo desplazamiento a otro estado.
		 foreach (JugadorMB jugadormb in jugadores) {
			Waypoint waypoint = Navigation.GetNearestNode(nodo_estado_actual.estado_actual.posicion_jugadores[jugadormb.jugador.id]);
			acciones_realizadas.Add(acciones_dict[jugadormb.jugador.id][waypoint][waypoint]);
		 }
	  }

	  Nodo_Estado nodo_estado_temp = nodo_estado_previo;
	  bool[] realizo_accion = new bool[jugadores.Count];
	  int q = 1;
	  foreach (Accion accion in acciones_realizadas) {
		 float tiempo = Time.time - (acciones_realizadas.Count - q) * (Time.deltaTime / acciones_realizadas.Count);

		 registarEstado(tiempo, nodo_estado_temp);
		 nodo_estado_temp = nodo_estado_temp.hijoAccion(accion) as Nodo_Estado;

		 realizo_accion[accion.actor_id] = true;
		 jugadores[accion.actor_id].jugador.registrarAccion(tiempo, accion);
		 q++;
	  }

	  for (int k = 0; k < realizo_accion.Length; k++) {
		 if (!realizo_accion[k]) {
			Waypoint waypoint = Navigation.GetNearestNode(nodo_estado_actual.estado_actual.posicion_jugadores[jugadores[k].jugador.id]);
			jugadores[k].jugador.registrarAccion(Time.time, acciones_dict[jugadores[k].jugador.id][waypoint][waypoint]);
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

   // Inferencia de objetivos debug
   private ObjetivoMB inferirObjetivo(JugadorMB jugadormb, int cant_acciones_inferencia, float factor_descuento_inferencia, out float[] valor_objetivo) {
	  valor_objetivo = new float[objetivos.Count];
	  float descuento = 1.0f;
	  float suma = 0;

	  Nodo_Estado nodo;
	  float tiempo;

	  int tope_inferior = cant_acciones_inferencia - Mathf.Min(cant_acciones_inferencia, historial_estados.Count);
	  int t = historial_estados.Count;
	  foreach (KeyValuePair<float, Nodo_Estado> tiempo_estado in historial_estados) {
		 if (t < tope_inferior) {
			break;
		 }

		 tiempo = tiempo_estado.Key;
		 nodo = tiempo_estado.Value;
		 foreach (ObjetivoMB objetivomb in objetivos) {
			Accion accion = mdp.Politica[jugadormb.jugador.id][objetivomb.objetivo.id][nodo.estado_actual.id];
			Accion accion_jugador;
			if (jugadormb.jugador.acciones.TryGetValue(tiempo, out accion_jugador) && accion.id == accion_jugador.id) {
			   valor_objetivo[objetivomb.objetivo.id] += descuento;
			   suma += descuento;
			}
		 }

		 descuento = descuento * factor_descuento_inferencia;

		 t--;
	  }


	  if (suma > 0) {
		 for (int i = 0; i < valor_objetivo.Length; i++) {
			valor_objetivo[i] = valor_objetivo[i] / suma;
		 }

		 float max_valor = float.MinValue;
		 int objetivo_id = -1;
		 for (int i = 0; i < valor_objetivo.Length; i++) {
			if (!objetivos[i].objetivo.cumplido && valor_objetivo[i] > max_valor) {
			   max_valor = valor_objetivo[i];
			   objetivo_id = i;
			}
		 }

		 if (objetivo_id != -1) {
			return objetivos[objetivo_id];
		 }

		 return null;
	  }
	  return null;
   }

   // Deteccion de objetivos
   public static void OnRadarDetect(SteeringEvent<Radar> evento) {
	  ObjetivoMB objetivomb = evento.Parameter.gameObject.GetComponent<ObjetivoMB>();
	  foreach (Vehicle vehiculo in evento.Parameter.Vehicles) {
		 JugadorMB jugadormb = vehiculo.gameObject.GetComponent<JugadorMB>();
		 if (jugadormb != null) {
			foreach (Vehicle vehiculo_complementario in objetivomb.objetivo.complementario.objetivo_mb.radar.Vehicles) {
			   JugadorMB jugadormb_complementario = vehiculo_complementario.gameObject.GetComponent<JugadorMB>();
			   if (jugadormb_complementario != null && jugadormb != jugadormb_complementario) {
				  objetivomb.objetivo.cumplido = true;
				  objetivomb.objetivo.complementario.cumplido = true;
				  break;
			   }
			}
		 }
	  }
   }
}