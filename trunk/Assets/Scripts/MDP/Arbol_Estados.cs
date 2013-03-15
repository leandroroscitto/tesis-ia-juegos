using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;

[Serializable]
public class Arbol_Estados : ISerializable {
   // Listas
   public List<Objetivo> objetivos;
   public List<Jugador> jugadores;
   public List<Accion> acciones;

   // Estados
   public Nodo_Estado nodo_estado_inicial;
   public List<Nodo_Estado> estados;

   // <numero_objetivos_cumplidos, <posicion_jugadores, nodo_estado>>
   public Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>> estados_dict;

   // Operaciones
   public Arbol_Estados() {
   }

   public Arbol_Estados(List<Jugador> jugs, List<Accion> accs, List<Objetivo> objs) {
	  acciones = accs;
	  objetivos = objs;
	  jugadores = jugs;
   }

   // Utilidades
   public void generarEstadosDiccionario() {
	  estados_dict = new Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>();
	  foreach (Nodo_Estado estado in estados) {
		 Generador_MDP.agregarEstadoDict(estado, estados_dict);
	  }
   }

   public Nodo_Estado getEstadoActual(Dictionary<int, Vector3> posicion_jugadores, HashSet<int> objetivos_cumplidos, HashSet<int> objetivos_no_cumplidos) {
	  Estado estado_buscado = new Estado();

	  estado_buscado.posicion_jugadores = posicion_jugadores;
	  estado_buscado.objetivos_cumplidos = objetivos_cumplidos;
	  estado_buscado.objetivos_no_cumplidos = objetivos_no_cumplidos;

	  Nodo_Estado nodo_buscado = null;
	  if (getEstadoDict(estado_buscado, out nodo_buscado)) {
		 return nodo_buscado;
	  }
	  else {
		 return null;
	  }
   }

   private bool getEstadoDict(Estado estado, out Nodo_Estado nodo_estado_resultado) {
	  int cant_obj_cumplidos = estado.objetivos_cumplidos.Count;
	  Vector3[] posicion_jugadores = new Vector3[estado.posicion_jugadores.Count];
	  estado.posicion_jugadores.Values.CopyTo(posicion_jugadores, 0);

	  Dictionary<Vector3[], List<Nodo_Estado>> PosJugador_Estados;
	  if (estados_dict.TryGetValue(cant_obj_cumplidos, out PosJugador_Estados)) {
		 List<Nodo_Estado> Lista_Estados;
		 if (PosJugador_Estados.TryGetValue(posicion_jugadores, out Lista_Estados)) {
			foreach (Nodo_Estado nodo_estado in Lista_Estados) {
			   if (nodo_estado.estado_actual.mismoEstado(estado)) {
				  nodo_estado_resultado = nodo_estado;
				  return true;
			   }
			}
		 }
		 else {
			Debug.LogWarning("No encontro por la posicion de jugadores.");
			foreach (Vector3 posicion in posicion_jugadores) {
			   Debug.LogWarning(Navigation.GetNearestNode(posicion).name + "," + Navigation.GetNearestNode(posicion).Position + "; " + posicion);
			}
		 }
	  }
	  else {
		 Debug.LogWarning("No encontro cantidad de objetivos cumplidos: " + cant_obj_cumplidos);
	  }

	  nodo_estado_resultado = null;
	  return false;
   }

   // Serializacion
   public Arbol_Estados(SerializationInfo info, StreamingContext ctxt) {
	  objetivos = info.GetValue("Objectivos", typeof(List<Objetivo>)) as List<Objetivo>;
	  jugadores = info.GetValue("Jugadores", typeof(List<Jugador>)) as List<Jugador>;
	  acciones = info.GetValue("Acciones", typeof(List<Accion>)) as List<Accion>;
	  estados = info.GetValue("Estados", typeof(List<Nodo_Estado>)) as List<Nodo_Estado>;
	  nodo_estado_inicial = info.GetValue("Nodo_Estado_Inicial", typeof(Nodo_Estado)) as Nodo_Estado;
	  //estados_dict = info.GetValue("Diccionario_Estados", typeof(Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>)) as Dictionary<int, Dictionary<Vector3[], List<Nodo_Estado>>>;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Objectivos", objetivos);
	  info.AddValue("Jugadores", jugadores);
	  info.AddValue("Acciones", acciones);
	  info.AddValue("Estados", estados);
	  info.AddValue("Nodo_Estado_Inicial", nodo_estado_inicial);
	  //info.AddValue("Diccionario_Estados", estados_dict);
   }
}