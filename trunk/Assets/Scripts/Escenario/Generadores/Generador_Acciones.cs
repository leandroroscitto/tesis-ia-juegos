using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using PathRuntime;

public class Generador_Acciones : MonoBehaviour {
   // Acciones
   public List<Accion> acciones;

   // Representacion
   public GameObject acciones_objeto;

   // Operaciones
   public void inicializar() {
	  if (acciones_objeto == null) {
		 acciones_objeto = new GameObject("Acciones");
	  }
	  acciones_objeto.transform.parent = GetComponent<Generador_Escenario>().escenario_objeto.transform;
   }

   // Generacion
   public void generar() {

   }

   public void borrar() {
	  while (acciones_objeto.transform.childCount > 0) {
		 DestroyImmediate(acciones_objeto.transform.GetChild(0));
	  }

	  DestroyImmediate(acciones_objeto);
   }
}