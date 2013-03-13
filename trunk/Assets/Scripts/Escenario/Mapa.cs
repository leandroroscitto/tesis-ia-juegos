using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PathRuntime;
using Random = UnityEngine.Random;

[Serializable]
public class Mapa : ISerializable{
   public static Mapa Mapa_Instancia;

   // Atributos
   public int cant_x, cant_y;
   [HideInInspector]
   public Tile[] tiles;
   public Vector3 tamano_tile;

   // Dimensiones
   public float ancho, largo;
   public float offsetx, offsety;

   // Operaciones
   public Mapa() {
	  Mapa_Instancia = this;
   }

   public Mapa(int cx, int cy, Vector3 tam_tile) {
	  cant_x = cx;
	  cant_y = cy;

	  tamano_tile = tam_tile;

	  tiles = new Tile[cant_x * cant_y];

	  ancho = tamano_tile.x * cant_x;
	  largo = tamano_tile.z * cant_y;
	  offsetx = (ancho - tamano_tile.x) / 2;
	  offsety = (largo - tamano_tile.z) / 2;

	  Mapa_Instancia = this;
   }

   // Utilidades
   public Vector2 posicionRealARepresentacion(Vector3 posicion) {
	  return (Vector2.right * (offsetx - posicion.x) / tamano_tile.x + Vector2.up * (offsety - posicion.z) / tamano_tile.z);
   }

   public Vector3 posicionRepresentacionAReal(Vector2 posicion, float altura) {
	  return (Vector3.right * (offsetx - tamano_tile.x * posicion.x) + Vector3.forward * (offsety - tamano_tile.z * posicion.y) + Vector3.up * altura);
   }

   // Serializacion
   public Mapa(SerializationInfo info, StreamingContext ctxt) {
	  tiles = info.GetValue("Tiles", typeof(Tile[])) as Tile[];
	  cant_x = info.GetInt16("Cant_X");
	  cant_y = info.GetInt16("Cant_Y");
	  tamano_tile = (Vector3)info.GetValue("Tamano_Tile", typeof(Vector3));

	  ancho = tamano_tile.x * cant_x;
	  largo = tamano_tile.z * cant_y;
	  offsetx = (ancho - tamano_tile.x) / 2;
	  offsety = (largo - tamano_tile.z) / 2;

	  Mapa_Instancia = this;
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Tiles", tiles);
	  info.AddValue("Cant_X", cant_x);
	  info.AddValue("Cant_Y", cant_y);
	  info.AddValue("Tamano_Tile", tamano_tile);
   }
}