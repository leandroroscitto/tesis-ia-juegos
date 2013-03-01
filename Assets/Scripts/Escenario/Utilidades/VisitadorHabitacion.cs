using UnityEngine;
using System.Collections.Generic;
using Habitacion = Generador_Mapa.Habitacion;

public class VisitadorHabitacion : BSPTree.Visitador {
   public Dictionary<BSPTree, Habitacion> habitaciones;
   public List<BSPTree> nodos_habitaciones;
   private Generador_Mapa generador;
   private Generador_Escenario.Parametros_Mapa parametros;

   public VisitadorHabitacion(Generador_Mapa gen_map, Generador_Escenario.Parametros_Mapa param) {
	  generador = gen_map;
	  parametros = param;
	  habitaciones = new Dictionary<BSPTree, Habitacion>();
	  nodos_habitaciones = new List<BSPTree>();
   }

   public override bool visitarNodo(BSPTree nodo) {
	  if (nodo.esHoja()) {
		 int x, y, w, h;
		 w = Random.Range(parametros.min_hab_tam, nodo.ancho - 1);
		 h = Random.Range(parametros.min_hab_tam, nodo.largo - 1);
		 x = Random.Range(nodo.x, nodo.x + nodo.ancho - w - 1);
		 y = Random.Range(nodo.y, nodo.y + nodo.largo - h - 1);

		 nodo.x = x + 1;
		 nodo.y = y + 1;
		 nodo.ancho = w - 1;
		 nodo.largo = h - 1;

		 Habitacion habitacion = (Habitacion)generador.crearHabitacion(x + 1, y + 1, x + w - 1, y + h - 1, generador.tile_piso);
		 habitaciones.Add(nodo, habitacion);
		 nodos_habitaciones.Add(nodo);
	  }
	  return true;
   }
}