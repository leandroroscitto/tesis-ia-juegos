using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class ResolucionMDP : MonoBehaviour, ISerializable {
   [Serializable]
   public class TransicionJuego : Transicion_MDP<Estado, Accion>, ISerializable {
	  public override float valor(Accion a, Estado s, Estado sp) {
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
   public class RecompensaJuego : Recompensa_MDP<Estado, Objetivo>, ISerializable {
	  Objetivo[] objetivos;

	  public RecompensaJuego(ref Objetivo[] objs)
		 : base() {
		 objetivos = objs;
	  }

	  public override Vector2 posicion_objetivo(Objetivo obj) {
		 return obj.posicion;
	  }

	  public override float valor(Estado s, Objetivo o, int actor_id) {
		 float resultado;
		 resultado = (s.estado_actual.escenario_base.mapa.Length) * 2;
		 resultado += (s.estado_actual.objetivos_cumplidos.Count - s.estado_actual.objetivos_no_cumplidos.Count);
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
		 objetivos = info.GetValue("Objetivos", typeof(Objetivo[])) as Objetivo[];
	  }
	  public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
		 info.AddValue("Objetivos", objetivos);
	  }
   }

   [HideInInspector]
   public Arbol_Estados arbol_estados;
   public MDP<Estado, Accion, Objetivo, TransicionJuego, RecompensaJuego> mdp;

   public ResolucionMDP() {

   }

   public void resolverMDP() {
	  TransicionJuego transicion = new TransicionJuego();
	  RecompensaJuego recompensa = new RecompensaJuego(ref arbol_estados.objetivos);

	  Estado[] estados = arbol_estados.estados.ToArray();
	  Accion[] acciones = arbol_estados.acciones_individuales.ToArray();
	  mdp = new MDP<Estado, Accion, Objetivo, TransicionJuego, RecompensaJuego>(estados, acciones, arbol_estados.objetivos, arbol_estados.jugadores.Length, transicion, recompensa, 0.85f);
   }

   // Serializacion
   public ResolucionMDP(SerializationInfo info, StreamingContext ctxt) {
	  arbol_estados = info.GetValue("Arbol_Estados", typeof(Arbol_Estados)) as Arbol_Estados;
	  mdp = info.GetValue("MDP", typeof(MDP<Estado, Accion, Objetivo, TransicionJuego, RecompensaJuego>)) as MDP<Estado, Accion, Objetivo, TransicionJuego, RecompensaJuego>;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Arbol_Estados", arbol_estados);
	  info.AddValue("MDP", mdp);
   }
}