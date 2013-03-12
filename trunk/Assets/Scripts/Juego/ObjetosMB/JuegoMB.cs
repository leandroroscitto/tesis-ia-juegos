using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using PathRuntime;

public class JuegoMB : MonoBehaviour {
   // Listas
   public List<ObjetivoMB> objetivos;
   public List<JugadorMB> jugadores;
   public List<Accion> acciones;

   // Estructuras auxiliares
   private Dictionary<Waypoint, Dictionary<Waypoint, Accion>> acciones_dict;

   // Estados
   public Nodo_Estado nodo_estado_actual;
   public Arbol_Estados arbol_estados;

   // Operaciones
   public void inicializar(List<ObjetivoMB> objs, List<JugadorMB> jugs, List<Accion> accs, Arbol_Estados arb_ests) {
	  objetivos = objs;
	  jugadores = jugs;
	  acciones = accs;
	  arbol_estados = arb_ests;

	  acciones_dict = new Dictionary<Waypoint, Dictionary<Waypoint, Accion>>();
	  foreach (Accion accion in acciones) {
		 Dictionary<Waypoint, Accion> dict = null;
		 if (acciones_dict.TryGetValue(accion.origen, out dict)) {
			dict.Add(accion.destino, accion);
		 }
		 else {
			dict = new Dictionary<Waypoint, Accion>();
			dict.Add(accion.destino, accion);
			acciones_dict.Add(accion.origen, dict);
		 }
	  }
   }

   void Start() {

   }

   void Update() {
	  actualizarEstadoActual();
	  registrarAccionesJugadores();
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
		 Waypoint waypoint_actual = Navigation.GetNearestNode(jugadormb.jugador.posicion);
		 Waypoint waypoint_previo = Navigation.GetNearestNode(nodo_estado_actual.estado_actual.posicion_jugadores[jugadormb.jugador.id]);

		 Accion accion = acciones_dict[waypoint_previo][waypoint_actual];
		 jugadormb.jugador.registrarAccion(Time.time, accion);
	  }
   }
}