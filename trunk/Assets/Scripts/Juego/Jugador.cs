using UnityEngine;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PathRuntime;

[Serializable]
public class Jugador : ISerializable {
   public enum TControl {
	  DIRECTO, IA
   }

   public int id;
   public string nombre;
   public char representacion;
   //public Vector2 posicion;
   public TControl control;
   // <tiempo, accion>
   public SortedDictionary<float, Accion> acciones;

   private JugadorMB _jugadormb;
   public JugadorMB jugador_mb {
	  get {
		 if (_jugadormb == null) {
			GameObject jugador_objeto = GameObject.Find(nombre);
			if (jugador_objeto != null) {
			   _jugadormb = jugador_objeto.GetComponent<JugadorMB>();
			}
		 }

		 return _jugadormb;
	  }

	  set {
		 _jugadormb = value;
	  }
   }

   public Vector3 posicion {
	  get {
		 return jugador_mb.transform.position;
	  }
	  set {
		 jugador_mb.transform.position = value;
	  }
   }

   // Operaciones
   public Jugador() {

   }

   public Jugador(int i, string n, char r, Vector3 p, TControl c) {
	  id = i;
	  nombre = n;
	  representacion = r;
	  control = c;
	  acciones = new SortedDictionary<float, Accion>();

	  jugador_mb.jugador = this;
	  posicion = p;
   }

   public override string ToString() {
	  return "Jugado_id: " + id + ", nombre: " + nombre + ", posicion: " + posicion;
   }

   // Acciones
   public Accion registrarAccion(float tiempo, Accion accion) {
	  if (acciones.Count == 0) {
		 acciones.Add(tiempo, accion);
	  }
	  else {
		 float ultimo_tiempo = acciones.Keys.Last<float>();
		 Accion ultima_accion = acciones[ultimo_tiempo];

		 if (ultima_accion.mismaAccion(accion)) {
			acciones.Remove(ultimo_tiempo);
			acciones.Add(tiempo, ultima_accion);
		 }
		 else {
			if (acciones.ContainsKey(tiempo)) {
			   Debug.LogWarning(tiempo);
			   Debug.LogWarning(tiempo + float.Epsilon);
			   Debug.LogWarning(accion);
			   Debug.LogWarning(acciones[tiempo]);
			}
			acciones.Add(tiempo, accion);
		 }
	  }

	  return accion;
   }

   public void removerAccion(float tiempo) {
	  acciones.Remove(tiempo);
   }

   // Waypoint mas cercano a la posicion actual.
   public Waypoint waypointMasCercano() {
	  return Navigation.GetNearestNode(posicion);
   }

   // Waypoint mas cercano conectado al waypoint proporcionado.
   public Waypoint waypointMasCercano(Waypoint waypoint_conectado) {
	  Waypoint menor_waypoint = null;
	  menor_waypoint = Navigation.GetNearestNode(posicion);
	  if (waypoint_conectado.ConnectsTo(menor_waypoint) || waypoint_conectado == menor_waypoint) {
		 return menor_waypoint;
	  }
	  else {
		 float menor_distancia = float.MaxValue;
		 foreach (Connection conexion in waypoint_conectado.Connections) {
			float distancia = Vector3.Distance(posicion, conexion.To.Position);
			if (distancia < menor_distancia) {
			   menor_distancia = distancia;
			   menor_waypoint = conexion.To;
			}
		 }
		 return menor_waypoint;
	  }
   }

   // Waypoint mas cercano, que no pertenece a un objetivo
   public Waypoint waypointMasCercanoNoObjetivo() {
	  Waypoint menor_waypoint = null;
	  float menor_distancia = float.MaxValue;
	  foreach (Waypoint waypoint in Navigation.Waypoints) {
		 float distancia = Vector3.Distance(posicion, waypoint.Position);
		 if (waypoint.tag != "Objetivo" && distancia < menor_distancia) {
			menor_distancia = distancia;
			menor_waypoint = waypoint;
		 }
	  }
	  return menor_waypoint;
   }

   // Waypoint mas cercano conectado al waypoint proporcionado, que no pertenece a un objetivo
   public Waypoint waypointMasCercanoNoObjetivo(Waypoint waypoint_conectado) {
	  Waypoint menor_waypoint = Navigation.GetNearestNode(posicion);
	  if (menor_waypoint.tag != "Objetivo" && (waypoint_conectado.ConnectsTo(menor_waypoint) || waypoint_conectado == menor_waypoint)) {
		 return menor_waypoint;
	  }
	  else {
		 menor_waypoint = waypoint_conectado;
		 float menor_distancia = float.MaxValue;
		 if (menor_waypoint.tag != "Objetivo") {
			menor_distancia = Vector3.Distance(posicion, waypoint_conectado.Position);
		 }
		 foreach (Connection conexion in waypoint_conectado.Connections) {
			float distancia = Vector3.Distance(posicion, conexion.To.Position);
			if (conexion.To.tag != "Objetivo" && distancia < menor_distancia) {
			   menor_distancia = distancia;
			   menor_waypoint = conexion.To;
			}
		 }
		 return menor_waypoint;
	  }
   }

   // Serializacion
   public Jugador(SerializationInfo info, StreamingContext ctxt) {
	  id = info.GetInt16("Id");
	  nombre = info.GetString("Nombre");
	  representacion = info.GetChar("Representacion");
	  posicion = (Vector3)info.GetValue("Posicion", typeof(Vector3));
	  control = (TControl)info.GetInt16("Control");
	  acciones = new SortedDictionary<float, Accion>();

	  jugador_mb.jugador = this;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Id", id);
	  info.AddValue("Nombre", nombre);
	  info.AddValue("Representacion", representacion);
	  info.AddValue("Posicion", posicion);
	  info.AddValue("Control", control);
   }
}
