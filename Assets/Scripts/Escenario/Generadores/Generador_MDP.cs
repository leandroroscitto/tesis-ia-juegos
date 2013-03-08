using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEditor;
using PathRuntime;

public class Generador_MDP : MonoBehaviour {
   // Representacion
   public GameObject mdp_objeto;

   // Resolucion
   [NonSerialized]
   public ResolucionMDP resolucion_mdp;
   [NonSerialized]
   public Arbol_Estados arbol_estados;

   // Operaciones
   public void inicializar(Mapa mapa, List<Jugador> jugadores, List<Accion> acciones, List<Objetivo> objetivos) {
	  if (mdp_objeto == null) {
		 mdp_objeto = new GameObject("MDP");
	  }
	  mdp_objeto.transform.parent = GetComponent<Generador_Escenario>().escenario_objeto.transform;

	  arbol_estados = new Arbol_Estados(mapa, jugadores, acciones, objetivos);
	  resolucion_mdp = new ResolucionMDP(arbol_estados);
   }

   // Generacion
   public void generar() {
	  prepararEstados();
	  //resolucion_mdp.resolverMDP();
   }

   public void borrar() {
	  while (mdp_objeto.transform.childCount > 0) {
		 DestroyImmediate(mdp_objeto.transform.GetChild(0));
	  }

	  DestroyImmediate(mdp_objeto);
   }

   // Generacion Arbol de Estado
   private Comparador_Arreglo_Vector3 comparador = new Comparador_Arreglo_Vector3();

   private Nodo_Estado nodo_estado_actual;
   private int cant_estados;

   // <numero_objetivos_cumplidos, <posicion_playes, nodo_estado>>
   private Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>> estados_dict;

   // <numero_objetivos_cumplidos, <posicion_playes, nodo_estado>>
   private Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>> frontera_dict;
   private Queue<Nodo_Estado> frontera;

   private void prepararEstados() {
	  estados_dict = new Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>();
	  arbol_estados.estados = new List<Nodo_Estado>();
	  frontera_dict = new Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>();
	  frontera = new Queue<Nodo_Estado>();

	  cant_estados = 0;
	  Estado estado_inicial = new Estado(cant_estados);
	  foreach (Objetivo objetivo in arbol_estados.objetivos) {
		 estado_inicial.objetivos_no_cumplidos.Add(objetivo.id);
	  }
	  foreach (Jugador jugador in arbol_estados.jugadores) {
		 estado_inicial.posicion_jugadores.Add(jugador.id, jugador.posicion);
	  }
	  verificarCumplimientoObjetivos(estado_inicial);

	  arbol_estados.nodo_estado_inicial = new Nodo_Estado(cant_estados, estado_inicial);
	  frontera.Enqueue(arbol_estados.nodo_estado_inicial);
	  agregarEstadoDict(arbol_estados.nodo_estado_inicial, frontera_dict);

	  while (frontera.Count > 0) {
		 nodo_estado_actual = frontera.Dequeue();
		 removerEstadoDict(nodo_estado_actual, frontera_dict);

		 List<Accion> acciones_disponibles = getAccionesDisponibles(nodo_estado_actual);
		 foreach (Accion accion in acciones_disponibles) {
			foreach (Jugador jugador in arbol_estados.jugadores) {
			   jugador.posicion = nodo_estado_actual.estado_actual.posicion_jugadores[jugador.id];
			}

			int ja_jugador_id = accion.jugador.id;
			int ja_accion_id = accion.id;
			Vector3 nueva_posicion;
			if (nodo_estado_actual.estado_actual.IntentarAccion(arbol_estados.jugadores[ja_jugador_id], arbol_estados.acciones[ja_accion_id], out nueva_posicion)) {
			   // Obtener el proximo estado a partir del actual y la accion del jugador.
			   bool en_visitado = false;
			   bool en_frontera = false;
			   Nodo_Estado proximo_estado_nodo = buscarProximoEstado(nodo_estado_actual, arbol_estados.jugadores[ja_jugador_id], nueva_posicion, out en_visitado, out en_frontera);

			   // Si no existe el proximo estado en niguna lista, lo crea.
			   if (proximo_estado_nodo == null) {
				  cant_estados++;
				  Estado proximo_estado = new Estado(cant_estados);
				  foreach (int objetivo_id in nodo_estado_actual.estado_actual.objetivos_cumplidos) {
					 proximo_estado.objetivos_cumplidos.Add(objetivo_id);
				  }
				  foreach (int objetivo_id in nodo_estado_actual.estado_actual.objetivos_no_cumplidos) {
					 proximo_estado.objetivos_no_cumplidos.Add(objetivo_id);
				  }
				  foreach (Jugador jugador in arbol_estados.jugadores) {
					 proximo_estado.posicion_jugadores.Add(jugador.id, jugador.posicion);
				  }
				  proximo_estado.posicion_jugadores[ja_jugador_id] = nueva_posicion;
				  verificarCumplimientoObjetivos(proximo_estado);

				  proximo_estado_nodo = new Nodo_Estado(cant_estados, proximo_estado);
			   }

			   // Verificar si pertenezca a 'estados' o es igual que el estado actual.
			   if (!en_visitado) {
				  // Si no pertenece, establecer la relacion padre-hijo.
				  nodo_estado_actual.AgregarHijo(proximo_estado_nodo, accion, 1.0f);

				  // Verifica si ya se encuentra en la frontera. De no ser asi lo agrega.
				  if (!en_frontera) {
					 frontera.Enqueue(proximo_estado_nodo);
					 agregarEstadoDict(proximo_estado_nodo, frontera_dict);
				  }
			   }
			   else {
				  nodo_estado_actual.AgregarHijo(proximo_estado_nodo, accion, 1.0f);
			   }
			}

			mostrarDebuging();
		 }

		 agregarEstadoDict(nodo_estado_actual, estados_dict);
		 arbol_estados.estados.Add(nodo_estado_actual);

		 //yield return true;
	  }

	  foreach (Jugador jugador in arbol_estados.jugadores) {
		 jugador.posicion = estado_inicial.posicion_jugadores[jugador.id];
	  }
   }

   private void mostrarDebuging() {
	  if (cant_estados % 100 == 0) {
		 Debug.Log("Estados procesados: " + cant_estados + ", y en frontera: " + frontera.Count);
	  }
   }

   private List<Accion> getAccionesDisponibles(Nodo_Estado estado) {
	  Dictionary<int, Vector3> posiciones_jugadores = new Dictionary<int, Vector3>(estado.estado_actual.posicion_jugadores.Count);
	  foreach (KeyValuePair<int, Vector3> posicion_jugador in estado.estado_actual.posicion_jugadores) {
		 posiciones_jugadores.Add(posicion_jugador.Key, Navigation.GetNearestNode(posicion_jugador.Value).Position);
	  }

	  List<Accion> acciones_disponibles = new List<Accion>();
	  foreach (Accion accion in arbol_estados.acciones) {
		 Vector3 posicion_jugador = posiciones_jugadores[accion.actor_id];
		 if (accion.origen.Position == posicion_jugador) {
			acciones_disponibles.Add(accion);
		 }
	  }
	  return acciones_disponibles;
   }

   private void agregarEstadoDict(Nodo_Estado estado, Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>> dict) {
	  int cant_obj_cumplidos = estado.estado_actual.objetivos_cumplidos.Count;
	  Vector3[] posicion_jugadores = new Vector3[estado.estado_actual.posicion_jugadores.Count];
	  estado.estado_actual.posicion_jugadores.Values.CopyTo(posicion_jugadores, 0);

	  Dictionary<Vector3[], List<Nodo_Estado>> PosJugador_Estados;
	  if (dict.TryGetValue(cant_obj_cumplidos, out PosJugador_Estados)) {
		 List<Nodo_Estado> Lista_Estados;
		 if (PosJugador_Estados.TryGetValue(posicion_jugadores, out Lista_Estados)) {
			Lista_Estados.Add(estado);
		 }
		 else {
			Lista_Estados = new List<Nodo_Estado>();
			Lista_Estados.Add(estado);
			PosJugador_Estados.Add(posicion_jugadores, Lista_Estados);
		 }
	  }
	  else {
		 PosJugador_Estados = new Dictionary<Vector3[], List<Nodo_Estado>>(comparador);
		 List<Nodo_Estado> Lista_Estados = new List<Nodo_Estado>();
		 Lista_Estados.Add(estado);
		 PosJugador_Estados.Add(posicion_jugadores, Lista_Estados);
		 dict.Add(cant_obj_cumplidos, PosJugador_Estados);
	  }
   }

   private void removerEstadoDict(Nodo_Estado estado, Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>> dict) {
	  int cant_obj_cumplidos = estado.estado_actual.objetivos_cumplidos.Count;
	  Vector3[] posicion_jugadores = new Vector3[estado.estado_actual.posicion_jugadores.Count];
	  estado.estado_actual.posicion_jugadores.Values.CopyTo(posicion_jugadores, 0);

	  Dictionary<Vector3[], List<Nodo_Estado>> PosJugador_Estados;
	  if (dict.TryGetValue(cant_obj_cumplidos, out PosJugador_Estados)) {
		 List<Nodo_Estado> Lista_Estados;
		 if (PosJugador_Estados.TryGetValue(posicion_jugadores, out Lista_Estados)) {
			Lista_Estados.Remove(estado);
		 }
	  }
   }

   private bool getEstadoDict(Nodo_Estado estado, Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>> dict, out Nodo_Estado nodo_estado_resultado) {
	  int cant_obj_cumplidos = estado.estado_actual.objetivos_cumplidos.Count;
	  Vector3[] posicion_jugadores = new Vector3[estado.estado_actual.posicion_jugadores.Count];
	  estado.estado_actual.posicion_jugadores.Values.CopyTo(posicion_jugadores, 0);

	  Dictionary<Vector3[], List<Nodo_Estado>> PosJugador_Estados;
	  if (dict.TryGetValue(cant_obj_cumplidos, out PosJugador_Estados)) {
		 List<Nodo_Estado> Lista_Estados;
		 if (PosJugador_Estados.TryGetValue(posicion_jugadores, out Lista_Estados)) {
			foreach (Nodo_Estado nodo_estado in Lista_Estados) {
			   if (nodo_estado.estado_actual.mismoEstado(estado.estado_actual)) {
				  nodo_estado_resultado = nodo_estado;
				  return true;
			   }
			}
		 }
	  }

	  nodo_estado_resultado = null;
	  return false;
   }

   private void modificarEstado(Estado estado, Jugador jugador, Vector3 nueva_posicion, out Vector3 antigua_posicion, out List<int> objetivos_modificados) {
	  antigua_posicion = estado.posicion_jugadores[jugador.id];
	  estado.posicion_jugadores[jugador.id] = nueva_posicion;
	  objetivos_modificados = verificarCumplimientoObjetivos(estado);
   }

   private void restaurarEstado(Estado estado, Jugador jugador, Vector3 antigua_posicion, List<int> objetivos_modificados) {
	  estado.posicion_jugadores[jugador.id] = antigua_posicion;
	  foreach (int objetivo_id in objetivos_modificados) {
		 estado.objetivos_cumplidos.Remove(objetivo_id);
		 estado.objetivos_no_cumplidos.Add(objetivo_id);
	  }
   }

   private Nodo_Estado buscarProximoEstado(Nodo_Estado estado, Jugador jugador, Vector3 nueva_posicion, out bool en_visitado, out bool en_frontera) {
	  Vector3 posicion_actual;
	  List<int> objetivos_modificados;
	  modificarEstado(estado.estado_actual, jugador, nueva_posicion, out posicion_actual, out objetivos_modificados);
	  if (!nueva_posicion.Equals(posicion_actual)) {
		 Nodo_Estado nodo_estado;
		 if (getEstadoDict(estado, estados_dict, out nodo_estado)) {
			restaurarEstado(estado.estado_actual, jugador, posicion_actual, objetivos_modificados);
			en_visitado = true;
			en_frontera = false;
			return nodo_estado;
		 }

		 if (getEstadoDict(estado, frontera_dict, out nodo_estado)) {
			restaurarEstado(estado.estado_actual, jugador, posicion_actual, objetivos_modificados);
			en_visitado = false;
			en_frontera = true;
			return nodo_estado;
		 }

		 restaurarEstado(estado.estado_actual, jugador, posicion_actual, objetivos_modificados);
		 en_visitado = false;
		 en_frontera = false;
		 return null;
	  }
	  else {
		 restaurarEstado(estado.estado_actual, jugador, posicion_actual, objetivos_modificados);
		 en_visitado = false;
		 en_frontera = true;
		 return nodo_estado_actual;
	  }
   }

   private List<int> verificarCumplimientoObjetivos(Estado estado) {
	  bool[] objetivos_cumplidos = new bool[arbol_estados.objetivos.Count];
	  foreach (Vector3 posicion_jugador in estado.posicion_jugadores.Values) {
		 foreach (int objetivo_id in estado.objetivos_no_cumplidos) {
			Objetivo objetivo = arbol_estados.objetivos[objetivo_id];
			if (Vector3.Distance(posicion_jugador, objetivo.posicion) < objetivo.radio) {
			   objetivos_cumplidos[objetivo_id] = true;
			}
		 }
	  }

	  List<int> nuevos_cumplidos = new List<int>();
	  foreach (int objetivo_id in estado.objetivos_no_cumplidos) {
		 Objetivo objetivo = arbol_estados.objetivos[objetivo_id];
		 if (objetivos_cumplidos[objetivo.id] && objetivos_cumplidos[objetivo.complementario.id]) {
			nuevos_cumplidos.Add(objetivo.id);
			nuevos_cumplidos.Add(objetivo.complementario.id);
		 }
	  }

	  foreach (int objetivo_id in nuevos_cumplidos) {
		 estado.objetivos_no_cumplidos.Remove(objetivo_id);
		 estado.objetivos_cumplidos.Add(objetivo_id);
	  }

	  return nuevos_cumplidos;
   }
}