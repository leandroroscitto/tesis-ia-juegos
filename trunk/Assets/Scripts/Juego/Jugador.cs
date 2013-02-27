using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

[Serializable]
public class Jugador : MonoBehaviour, ISerializable {
   public enum TControl {
	  DIRECTO, IA
   }

   public int id;
   public string nombre;
   public char representacion;
   public Vector2 posicion;
   public TControl control;
   // <turno, accion>
   public Dictionary<int, Accion> acciones;

   public Jugador(int i, string n, char r, Vector2 p, TControl c) {
	  id = i;
	  nombre = n;
	  representacion = r;
	  posicion = p;
	  control = c;
	  acciones = new Dictionary<int, Accion>();
   }

   public Accion RegistrarAccion(int turno, Accion accion) {
	  acciones.Add(turno, accion);
	  return accion;
   }

   public override string ToString() {
	  return "Jugado_id: " + id + ", nombre: " + nombre + ", posicion: " + posicion;
   }

   // Serializacion
   public Jugador(SerializationInfo info, StreamingContext ctxt) {
	  id = info.GetInt16("Id");
	  nombre = info.GetString("Nombre");
	  representacion = info.GetChar("Representacion");
	  posicion = (Vector2)info.GetValue("Posicion", typeof(Vector2));
	  control = (TControl)info.GetInt16("Control");
	  acciones = info.GetValue("Acciones", typeof(Dictionary<int, Accion>)) as Dictionary<int, Accion>;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Id", id);
	  info.AddValue("Nombre", nombre);
	  info.AddValue("Representacion", representacion);
	  info.AddValue("Posicion", posicion);
	  info.AddValue("Control", control);
	  info.AddValue("Acciones", acciones);
   }
}
