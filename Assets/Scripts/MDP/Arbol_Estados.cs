﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Comparador_Arreglo_Vector3 : IEqualityComparer<Vector3[]> {

   public bool Equals(Vector3[] x, Vector3[] y) {
	  if (x.Length == y.Length) {
		 for (int i = 0; i < x.Length; i++) {
			if (!x[i].Equals(y[i]))
			   return false;
		 }
		 return true;
	  }
	  else
		 return false;
   }

   public int GetHashCode(Vector3[] obj) {
	  int dimensiones = Mapa.cant_x * Mapa.cant_y;
	  int valor = 0;
	  for (int i = 0; i < obj.Length; i++) {
		 valor += obj[i].GetHashCode() * (int)Math.Pow(Math.Floor(Math.Log10(dimensiones)), i);
	  }
	  return valor;
   }
}

[Serializable]
public class Arbol_Estados : ISerializable {
   private Comparador_Arreglo_Vector3 comparador = new Comparador_Arreglo_Vector3();

   public Mapa mapa_base;

   public List<Objetivo> objetivos;
   public List<Jugador> jugadores;
   public List<Accion> acciones;

   public Nodo_Estado nodo_estado_inicial;

   // <numero_objetivos_cumplidos, <posicion_playes, nodo_estado>>
   private Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>> estados_dict;
   public List<Nodo_Estado> estados;

   private Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>> frontera_dict;
   private Queue<Nodo_Estado> frontera;

   private Nodo_Estado nodo_estado_actual;
   private int cant_estados;

   public Arbol_Estados() {

   }

   public Arbol_Estados(Mapa eb, List<Jugador> jugs, List<Accion> accs, List<Objetivo> objs) {
	  mapa_base = eb;
	  acciones = accs;
	  objetivos = objs;
	  jugadores = jugs;
   }

   public void prepararEstados() {
	  estados_dict = new Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>();
	  estados = new List<Nodo_Estado>();
	  frontera_dict = new Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>();
	  frontera = new Queue<Nodo_Estado>();

	  cant_estados = 0;
	  Estado estado_inicial = new Estado(cant_estados, mapa_base);
	  foreach (Objetivo objetivo in objetivos) {
		 estado_inicial.objetivos_no_cumplidos.Add(objetivo.id);
	  }
	  foreach (Jugador jugador in jugadores) {
		 estado_inicial.posicion_jugadores.Add(jugador.id, jugador.posicion);
	  }
	  verificarCumplimientoObjetivos(estado_inicial);

	  nodo_estado_inicial = new Nodo_Estado(cant_estados, estado_inicial);
	  frontera.Enqueue(nodo_estado_inicial);
	  agregarEstadoDict(nodo_estado_inicial, frontera_dict);

	  while (frontera.Count > 0) {
		 nodo_estado_actual = frontera.Dequeue();
		 removerEstadoDict(nodo_estado_actual, frontera_dict);

		 mostrarDebuging();

		 List<Accion> acciones_disponibles = getAccionesDisponibles(nodo_estado_actual);
		 foreach (Accion accion in acciones_disponibles) {
			foreach (Jugador jugador in jugadores) {
			   jugador.posicion = nodo_estado_actual.estado_actual.posicion_jugadores[jugador.id];
			}

			int ja_jugador_id = accion.jugador.id;
			int ja_accion_id = accion.id;
			Vector3 nueva_posicion;
			if (nodo_estado_actual.estado_actual.IntentarAccion(jugadores[ja_jugador_id], acciones[ja_accion_id], out nueva_posicion)) {
			   // Obtener el proximo estado a partir del actual y la accion del jugador.
			   bool en_visitado = false;
			   bool en_frontera = false;
			   Nodo_Estado proximo_estado_nodo = buscarProximoEstado(nodo_estado_actual, jugadores[ja_jugador_id], nueva_posicion, out en_visitado, out en_frontera);

			   // Si no existe el proximo estado en niguna lista, lo crea.
			   if (proximo_estado_nodo == null) {
				  cant_estados++;
				  Estado proximo_estado = new Estado(cant_estados, mapa_base);
				  foreach (int objetivo_id in nodo_estado_actual.estado_actual.objetivos_cumplidos) {
					 proximo_estado.objetivos_cumplidos.Add(objetivo_id);
				  }
				  foreach (int objetivo_id in nodo_estado_actual.estado_actual.objetivos_no_cumplidos) {
					 proximo_estado.objetivos_no_cumplidos.Add(objetivo_id);
				  }
				  foreach (Jugador jugador in jugadores) {
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
		 }

		 agregarEstadoDict(nodo_estado_actual, estados_dict);
		 estados.Add(nodo_estado_actual);
	  }

	  foreach (Jugador jugador in jugadores) {
		 jugador.posicion = estado_inicial.posicion_jugadores[jugador.id];
	  }
   }

   private void mostrarDebuging() {
	  if (cant_estados % 100 == 0) {
		 Debug.Log("Estados procesados: " + cant_estados + ", y en frontera: " + frontera.Count);
	  }
   }

   private List<Accion> getAccionesDisponibles(Nodo_Estado estado) {
	  List<Accion> acciones_disponibles = new List<Accion>();
	  foreach (Accion accion in acciones) {
		 Vector3 posicion_jugador = estado.estado_actual.escenario_base.posicionRepresentacionAReal(estado.estado_actual.posicion_jugadores[accion.actor_id], accion.origen.Position.y);
		 if (Vector3.Distance(posicion_jugador, accion.origen.Position) < Generador_Escenario.radio_cercania_waypoint) {
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

   public void modificarEstado(Estado estado, Jugador jugador, Vector3 nueva_posicion, out Vector3 antigua_posicion, out List<int> objetivos_modificados) {
	  antigua_posicion = estado.posicion_jugadores[jugador.id];
	  estado.posicion_jugadores[jugador.id] = nueva_posicion;
	  objetivos_modificados = verificarCumplimientoObjetivos(estado);
   }

   public void restaurarEstado(Estado estado, Jugador jugador, Vector3 antigua_posicion, List<int> objetivos_modificados) {
	  estado.posicion_jugadores[jugador.id] = antigua_posicion;
	  foreach (int objetivo_id in objetivos_modificados) {
		 estado.objetivos_cumplidos.Remove(objetivo_id);
		 estado.objetivos_no_cumplidos.Add(objetivo_id);
	  }
   }

   public Nodo_Estado buscarProximoEstado(Nodo_Estado estado, Jugador jugador, Vector3 nueva_posicion, out bool en_visitado, out bool en_frontera) {
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

   public List<int> verificarCumplimientoObjetivos(Estado estado) {
	  bool[] objetivos_cumplidos = new bool[objetivos.Count];
	  foreach (Vector3 posicion_jugador in estado.posicion_jugadores.Values) {
		 foreach (int objetivo_id in estado.objetivos_no_cumplidos) {
			Objetivo objetivo = objetivos[objetivo_id];
			if (Vector3.Distance(posicion_jugador, objetivo.posicion) < objetivo.radio) {
			   objetivos_cumplidos[objetivo_id] = true;
			}
		 }
	  }

	  List<int> nuevos_cumplidos = new List<int>();
	  foreach (int objetivo_id in estado.objetivos_no_cumplidos) {
		 Objetivo objetivo = objetivos[objetivo_id];
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

   // Serializacion
   public Arbol_Estados(SerializationInfo info, StreamingContext ctxt) {
	  try {
		 mapa_base = info.GetValue("Escenario_Base", typeof(Mapa)) as Mapa;
		 objetivos = info.GetValue("Objectivos", typeof(List<Objetivo>)) as List<Objetivo>;
		 jugadores = info.GetValue("Jugadores", typeof(List<Jugador>)) as List<Jugador>;
		 acciones = info.GetValue("Acciones", typeof(List<Accion>)) as List<Accion>;
		 estados_dict = info.GetValue("Estados_Dict", typeof(Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>)) as Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>;
		 estados = info.GetValue("Estados", typeof(List<Nodo_Estado>)) as List<Nodo_Estado>;
		 nodo_estado_inicial = info.GetValue("Nodo_Estado_Inicial", typeof(Nodo_Estado)) as Nodo_Estado;
		 frontera_dict = info.GetValue("Frontera_Dict", typeof(Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>)) as Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>;
		 frontera = info.GetValue("Frontera", typeof(Queue<Nodo_Estado>)) as Queue<Nodo_Estado>;
	  }
	  catch (NullReferenceException nullrefexcp) {
		 Debug.LogError("ACA");
		 throw nullrefexcp;
	  }
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Escenario_Base", mapa_base);
	  info.AddValue("Objectivos", objetivos);
	  info.AddValue("Jugadores", jugadores);
	  info.AddValue("Acciones", acciones);
	  info.AddValue("Estados_Dict", estados_dict);
	  info.AddValue("Estados", estados);
	  info.AddValue("Nodo_Estado_Inicial", nodo_estado_inicial);
	  info.AddValue("Frontera_Dict", frontera_dict);
	  info.AddValue("Frontera", frontera);
   }
}