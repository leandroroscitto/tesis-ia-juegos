using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BSPTree {
   public abstract class Visitador {
	  public abstract bool visitarNodo(BSPTree nodo);
   }

   public enum TDivision {
	  HORIZONTAL,
	  VERTICAL,
	  NINGUNA
   }

   public int x, y, ancho, largo;
   public int nivel;
   public BSPTree padre;
   public BSPTree hijoD, hijoI;
   public TDivision division;
   public int posicion;

   public BSPTree(BSPTree padre_in, bool izquierda) {
	  padre = padre_in;
	  if (padre.division == TDivision.HORIZONTAL) {
		 x = padre.x;
		 ancho = padre.ancho;
		 y = izquierda ? padre.y : padre.posicion;
		 largo = izquierda ? padre.posicion - y : padre.y + padre.largo - padre.posicion;
	  }
	  else {
		 y = padre.y;
		 largo = padre.largo;
		 x = izquierda ? padre.x : padre.posicion;
		 ancho = izquierda ? padre.posicion - x : padre.x + padre.ancho - padre.posicion;
	  }
	  nivel = padre.nivel + 1;
   }

   public BSPTree(int x_in, int y_in, int ancho_in, int largo_in) {
	  padre = hijoD = hijoI = null;
	  x = x_in;
	  y = y_in;
	  ancho = ancho_in;
	  largo = largo_in;
	  nivel = 0;
	  division = TDivision.NINGUNA;
	  posicion = -1;
   }

   public void dividirArbol(int posicion_in, TDivision division_in) {
	  division = division_in;
	  posicion = posicion_in;

	  hijoD = new BSPTree(this, true);
	  hijoI = new BSPTree(this, false);
   }

   public void dividirArbolRecursivo(int max_prof, int min_ancho, int min_largo, float maxproph, float maxpropv) {
	  if (max_prof == 0 || (ancho < 2 * min_ancho && largo < 2 * min_largo)) {
		 return;
	  }

	  bool horizontal;
	  if (largo < 2 * min_largo || ancho > largo * maxproph) {
		 horizontal = false;
	  }
	  else if (ancho < 2 * min_ancho || largo > ancho * maxpropv) {
		 horizontal = true;
	  }
	  else {
		 horizontal = (Random.Range(0, 2) == 1);
	  }

	  TDivision div;
	  int pos;
	  if (horizontal) {
		 div = TDivision.HORIZONTAL;
		 pos = Random.Range(y + min_largo, y + largo - min_largo + 1);
	  }
	  else {
		 div = TDivision.VERTICAL;
		 pos = Random.Range(x + min_ancho, x + ancho - min_ancho + 1);
	  }

	  dividirArbol(pos, div);
	  hijoI.dividirArbolRecursivo(max_prof - 1, min_ancho, min_largo, maxproph, maxpropv);
	  hijoD.dividirArbolRecursivo(max_prof - 1, min_ancho, min_largo, maxproph, maxpropv);
   }

   public bool esHijoI() {
	  if (padre != null) {
		 return (padre.hijoI == this);
	  } else {
		 return false;
	  }
   }

   public bool esHijoD() {
	  if (padre != null) {
		 return (padre.hijoD == this);
	  }
	  else {
		 return false;
	  }
   }

   public bool esHoja() {
	  return ((hijoD == null) && (hijoI == null));
   }

   public BSPTree buscarHoja(bool por_derecha) {
	  if (esHoja()) {
		 return this;
	  }
	  else {
		 if (por_derecha) {
			return hijoD.buscarHoja(true);
		 }
		 else {
			return hijoI.buscarHoja(false);
		 }
	  }
   }

   public BSPTree hermano() {
	  if (padre != null) {
		 if (padre.hijoD == this) {
			return padre.hijoI;
		 }
		 else {
			return padre.hijoD;
		 }
	  }
	  else {
		 return null;
	  }
   }

   public bool recorrerOrdenPorNivelInverso(Visitador visitador) {
	  Stack<BSPTree> pila1 = new Stack<BSPTree>();
	  Stack<BSPTree> pila2 = new Stack<BSPTree>();
	  pila1.Push(this);
	  while (pila1.Count > 0) {
		 BSPTree nodo = pila1.Pop();
		 pila2.Push(nodo);
		 if (nodo.hijoD != null)
			pila1.Push(nodo.hijoD);
		 if (nodo.hijoI != null)
			pila1.Push(nodo.hijoI);
	  }
	  while (pila2.Count > 0) {
		 BSPTree nodo = pila2.Pop();
		 if (!visitador.visitarNodo(nodo))
			return false;
	  }
	  return true;
   }

   public void crearTXT(string nombre_salida) {
	  System.IO.StreamWriter salida = new System.IO.StreamWriter(nombre_salida);

	  char[] esc = new char[ancho * largo];
	  for (int i = 0; i < esc.Length; i++) {
		 esc[i] = ' ';
	  }

	  Queue<BSPTree> cola = new Queue<BSPTree>();
	  cola.Enqueue(this);

	  char car = 'A';
	  while (cola.Count > 0) {
		 BSPTree nodo = cola.Dequeue();
		 if (nodo.esHoja()) {
			for (int i = nodo.x; i < nodo.x + nodo.ancho; i++) {
			   for (int j = nodo.y; j < nodo.y + nodo.largo; j++) {
				  esc[i + j * ancho] = car;
			   }
			}
			car++;
		 }
		 if (nodo.hijoD != null) {
			cola.Enqueue(nodo.hijoD);
		 }
		 if (nodo.hijoI != null) {
			cola.Enqueue(nodo.hijoI);
		 }
	  }

	  salida.Write(' ');
	  salida.Write(' ');
	  for (int i = 0; i < ancho; i++) {
		 salida.Write(' ');
		 salida.Write(i % 10);
	  }
	  salida.WriteLine();
	  salida.Write(' ');
	  salida.Write(0);

	  for (int i = 0; i < esc.Length; i++) {
		 salida.Write(' ');
		 salida.Write(esc[i]);
		 if ((i + 1) % ancho == 0) {
			salida.WriteLine();
			salida.Write(' ');
			salida.Write((i + 1) / ancho % 10);
		 }
	  }

	  salida.Close();
   }
}