using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class ResolucionMDP : ISerializable {
   [Serializable]
   public class TransicionJuego : Transicion_MDP<Nodo_Estado, Accion>, ISerializable {
	  public override float getValor(Accion a, Nodo_Estado s, Nodo_Estado sp) {
		 if (s.estados_hijos != null) {
			float probabilidad_base = s.probabilidadHijoAccion(sp, a);
			return probabilidad_base;
		 }
		 else {
			// Nunca deberia llegar por aca.
			System.Diagnostics.Debug.Assert(false);
			return -1f;
		 }
	  }

	  // Serializacion
	  public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {

	  }
   }

   [Serializable]
   public class RecompensaJuego : Recompensa_MDP<Nodo_Estado, Objetivo>, ISerializable {
	  List<Objetivo> objetivos;

	  public RecompensaJuego(List<Objetivo> objs)
		 : base() {
		 objetivos = objs;
	  }

	  public override Vector2 getPosicionObjetivo(Objetivo obj) {
		 return obj.posicion;
	  }

	  public override float getValor(Nodo_Estado s, Objetivo o, int actor_id) {
		 float resultado;
		 resultado = Mapa.Mapa_Instancia.tiles.Length * 2;
		 resultado += s.estado_actual.objetivos_cumplidos.Count - s.estado_actual.objetivos_no_cumplidos.Count;
		 // TODO: Verificar la validez de usar distancia directa en vez de la distancia real,
		 // TODO: ver si se puede usar distancia real (no creo, por cuestiones de tiempo de pathfinding),
		 // TODO: en ese caso no usar.
		 if (s.estado_actual.objetivos_no_cumplidos.Contains(o.id)) {
			float distancia_minima = float.MaxValue;
			for (int actor = 0; actor < s.estado_actual.posicion_jugadores.Count; actor++) {
			   if (actor == actor_id) {
				  resultado -= Vector2.Distance(s.estado_actual.posicion_jugadores[actor], o.posicion);
			   }
			   else {
				  distancia_minima = Math.Min(distancia_minima, Vector2.Distance(s.estado_actual.posicion_jugadores[actor], o.complementario.posicion));
				  //distancia_minima = 0;
			   }
			}
			if (s.estado_actual.posicion_jugadores.Count > 1)
			   resultado -= distancia_minima;
		 }
		 else {
			resultado *= 2;
		 }

		 return resultado;
	  }

	  // Serializacion
	  public RecompensaJuego(SerializationInfo info, StreamingContext ctxt) {
		 objetivos = info.GetValue("Objetivos", typeof(List<Objetivo>)) as List<Objetivo>;
	  }
	  public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
		 info.AddValue("Objetivos", objetivos);
	  }
   }

   [HideInInspector]
   public Arbol_Estados arbol_estados;
   public MDP<Nodo_Estado, Accion, Objetivo, TransicionJuego, RecompensaJuego> mdp;

   public ResolucionMDP() {

   }

   public ResolucionMDP(Arbol_Estados ae) {
	  arbol_estados = ae;
   }

   public void resolverMDP() {
	  TransicionJuego transicion = new TransicionJuego();
	  RecompensaJuego recompensa = new RecompensaJuego(arbol_estados.objetivos);

	  mdp = new MDP<Nodo_Estado, Accion, Objetivo, TransicionJuego, RecompensaJuego>(arbol_estados.estados, arbol_estados.acciones, arbol_estados.objetivos, arbol_estados.jugadores.Count, transicion, recompensa, 0.85f);
   }

   // Serializacion
   public ResolucionMDP(SerializationInfo info, StreamingContext ctxt) {
	  arbol_estados = info.GetValue("Arbol_Estados", typeof(Arbol_Estados)) as Arbol_Estados;
	  mdp = info.GetValue("MDP", typeof(MDP<Nodo_Estado, Accion, Objetivo, TransicionJuego, RecompensaJuego>)) as MDP<Nodo_Estado, Accion, Objetivo, TransicionJuego, RecompensaJuego>;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Arbol_Estados", arbol_estados);
	  info.AddValue("MDP", mdp);
   }
}