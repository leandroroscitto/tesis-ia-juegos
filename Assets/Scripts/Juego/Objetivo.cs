using UnityEngine;
using System;
using System.Runtime.Serialization;
using PathRuntime;
using System.Collections.Generic;

[Serializable, RequireComponent(typeof(Radar))]
public class Objetivo : MonoBehaviour, Objetivo_MDP, ISerializable {
   public int id;
   public bool cumplido;
   public Vector2 posicion;
   public string nombre;
   public Objetivo complementario;
   public Waypoint waypoint_asociado;

   public Radar radar;

   public Objetivo() {

   }

   public void inicializar(string nombre_in, Vector2 p, Waypoint wayp_asociado_in) {
	  nombre = nombre_in;
	  posicion = p;
	  waypoint_asociado = wayp_asociado_in;

	  radar = GetComponent<Radar>();
	  radar.DetectionRadius = 2.5f;
	  radar.DetectDisabledVehicles = true;
	  radar.LayersChecked = 1 << LayerMask.NameToLayer("Jugador");
   }

   public void agregarComplementario(Objetivo obj) {
	  complementario = obj;
   }

   public int GetID() {
	  return id;
   }

   public override string ToString() {
	  return "Objetivo_id: " + id + ", cumplido: " + cumplido + ", complementario_id: " + complementario.id;
   }

   // Serializacion
   public Objetivo(SerializationInfo info, StreamingContext ctxt) {
	  id = info.GetInt16("Id");
	  cumplido = info.GetBoolean("Cumplido");
	  posicion = (Vector2)info.GetValue("Posicion", typeof(Vector2));
	  nombre = info.GetString("Nombre");
	  complementario = info.GetValue("Complementario", typeof(Objetivo)) as Objetivo;
	  waypoint_asociado = info.GetValue("Waypoint_Asociado", typeof(Waypoint)) as Waypoint;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Id", id);
	  info.AddValue("Cumplido", cumplido);
	  info.AddValue("Posicion", posicion);
	  info.AddValue("Nombre", nombre);
	  info.AddValue("Complementario", complementario);
	  info.AddValue("Waypoint_Asociado", waypoint_asociado);
   }
}