using UnityEngine;
using System;
using System.Runtime.Serialization;
using PathRuntime;
using System.Collections.Generic;

[Serializable]
public class Objetivo : Objetivo_MDP, ISerializable {
   public bool cumplido;
   public Vector2 posicion;
   public string nombre;
   public Objetivo complementario;

   public Waypoint waypoint_asociado;
   public Vector3 waypoint_posicion {
	  get {
		 if (waypoint_asociado != null) {
			return waypoint_asociado.Position;
		 }
		 else {
			return Vector3.zero;
		 }
	  }
	  set {
		 waypoint_asociado = null;
		 foreach (Waypoint waypoint in Navigation.Waypoints) {
			if (waypoint.Position == value) {
			   waypoint_asociado = waypoint;
			   break;
			}
		 }
	  }
   }

   private ObjetivoMB _objetivomb;
   public ObjetivoMB objetivo_mb {
	  get {
		 if (_objetivomb == null) {
			GameObject objetivo_objeto = GameObject.Find("Objetivo_" + nombre);
			if (objetivo_objeto != null) {
			   _objetivomb = objetivo_objeto.GetComponent<ObjetivoMB>();
			}
		 }

		 return _objetivomb;
	  }

	  set {
		 _objetivomb = value;
	  }
   }

   public float radio {
	  get {
		 return objetivo_mb.radar.DetectionRadius;
	  }
	  set {
		 objetivo_mb.radar.DetectionRadius = value;
	  }
   }

   public Objetivo(int i, string nombre_in, Vector2 p, Waypoint wayp_asociado_in)
	  : base(i) {
	  nombre = nombre_in;
	  posicion = p;
	  waypoint_asociado = wayp_asociado_in;
   }

   public void agregarComplementario(Objetivo obj) {
	  complementario = obj;
   }

   public override string ToString() {
	  return "Objetivo_id: " + id + ", cumplido: " + cumplido + ", complementario_id: " + complementario.id;
   }

   // Serializacion
   public Objetivo(SerializationInfo info, StreamingContext ctxt)
	  : base(info, ctxt) {
	  cumplido = info.GetBoolean("Cumplido");
	  posicion = (Vector2)info.GetValue("Posicion", typeof(Vector2));
	  nombre = info.GetString("Nombre");
	  complementario = info.GetValue("Complementario", typeof(Objetivo)) as Objetivo;
	  waypoint_posicion = (Vector3)info.GetValue("Waypoint_Asociado", typeof(Vector3));

	  objetivo_mb.objetivo = this;
   }

#pragma warning disable 0114
   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  base.GetObjectData(info, ctxt);

	  info.AddValue("Cumplido", cumplido);
	  info.AddValue("Posicion", posicion);
	  info.AddValue("Nombre", nombre);
	  info.AddValue("Complementario", complementario);
	  info.AddValue("Waypoint_Asociado", waypoint_posicion);
   }
#pragma warning restore 0114
}