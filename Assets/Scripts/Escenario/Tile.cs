using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class Tile : ISerializable {
   public enum TTile {
	  PISO = 0,
	  PARED = 1,
	  OBJETIVO = 2
   }
   public static TTile[] tipos_tiles = (TTile[])System.Enum.GetValues(typeof(TTile));
   public TTile tipo;
   public bool transitable;
   public string representacion2D;

   public Tile(TTile tipo_in, bool transitable_in, string rep2d_in) {
	  inicializar(tipo_in, transitable_in, rep2d_in);
   }

   public void inicializar(TTile tipo_in, bool transitable_in, string rep2d_in) {
	  tipo = tipo_in;
	  transitable = transitable_in;
	  representacion2D = rep2d_in;
   }

   // Serializacion
   public Tile(SerializationInfo info, StreamingContext ctxt) {
	  tipo = (TTile)info.GetInt32("Tipo");
	  transitable = info.GetBoolean("Transitable");
	  representacion2D = info.GetString("Representacion2D");
   }

   public void GetObjectData(SerializationInfo info, StreamingContext ctxt) {
	  info.AddValue("Tipo", tipo);
	  info.AddValue("Transitable", transitable);
	  info.AddValue("Representacion2D", representacion2D);
   }
}