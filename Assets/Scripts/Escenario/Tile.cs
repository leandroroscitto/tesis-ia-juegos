using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Tile {
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
}