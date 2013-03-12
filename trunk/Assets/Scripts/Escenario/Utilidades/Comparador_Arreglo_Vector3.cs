using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Comparador_Arreglo_Vector3 : IEqualityComparer<Vector3[]> {

   public bool Equals(Vector3[] x, Vector3[] y) {
	  if (x.Length == y.Length) {
		 for (int i = 0; i < x.Length; i++) {
			if (!x[i].Equals(y[i]))
			   return false;
		 }
		 return true;
	  }
	  else
		 return false;
   }

   public int GetHashCode(Vector3[] obj) {
	  int dimensiones = Mapa.Mapa_Instancia.cant_x * Mapa.Mapa_Instancia.cant_y;
	  int valor = 0;
	  for (int i = 0; i < obj.Length; i++) {
		 valor += obj[i].GetHashCode() * (int)Math.Pow(Math.Floor(Math.Log10(dimensiones)), i);
	  }
	  return valor;
   }
}