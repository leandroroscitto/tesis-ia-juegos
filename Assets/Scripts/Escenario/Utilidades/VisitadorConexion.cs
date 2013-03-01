using UnityEngine;
using Habitacion = Generador_Mapa.Habitacion;
using Conexion = Generador_Mapa.Conexion;
using System.Collections.Generic;

public class VisitadorConexion : BSPTree.Visitador {
   private Generador_Mapa generador;
   private int conexiones_extras;
   private VisitadorHabitacion visitador_hab;

   public VisitadorConexion(Generador_Mapa gen_map, int con_extras, VisitadorHabitacion vhab) {
	  generador = gen_map;
	  conexiones_extras = con_extras;
	  visitador_hab = vhab;
   }

   public override bool visitarNodo(BSPTree nodo) {
	  if (!nodo.esHoja()) {
		 setConexion(nodo.hijoI.buscarHoja(true), nodo.hijoD.buscarHoja(false));
	  }
	  else {
		 BSPTree nodo_conexion;
		 int conexiones_realizadas = 0;
		 while (conexiones_realizadas < Mathf.Min(conexiones_extras, visitador_hab.nodos_habitaciones.Count)) {
			nodo_conexion = visitador_hab.nodos_habitaciones[Random.Range(0, visitador_hab.nodos_habitaciones.Count)];
			if (nodo_conexion != nodo) {
			   setConexion(nodo, nodo_conexion);
			}
			conexiones_realizadas++;
		 }
	  }
	  return true;
   }

   public void setConexion(BSPTree nodo1, BSPTree nodo2) {
	  Habitacion habitacion1, habitacion2;
	  Conexion conexion1, conexion2;

	  habitacion1 = null;
	  habitacion2 = null;
	  visitador_hab.habitaciones.TryGetValue(nodo1, out habitacion1);
	  visitador_hab.habitaciones.TryGetValue(nodo2, out habitacion2);
	  if (habitacion1 != null) {
		 if (habitacion2 != null) {
			int conexion_x1 = habitacion1.x1 + habitacion1.ancho / 2;
			int conexion_y1 = habitacion1.y1 + habitacion1.largo / 2;
			int conexion_x2 = habitacion2.x1 + habitacion2.ancho / 2;
			int conexion_y2 = habitacion2.y1 + habitacion2.largo / 2;

			conexion1 = (Conexion)generador.crearConexion(conexion_x2, conexion_y2, conexion_x1, conexion_y2, generador.tile_piso, true);
			conexion2 = (Conexion)generador.crearConexion(conexion_x1, conexion_y2, conexion_x1, conexion_y1, generador.tile_piso, true);

			if (conexion2 != null) {
			   habitacion1.conexiones.Add(conexion2);
			}
			if (conexion1 != null) {
			   habitacion2.conexiones.Add(conexion1);
			}
		 }
	  }
   }
}