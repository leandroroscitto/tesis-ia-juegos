using System;
using System.Collections.Generic;

[System.Serializable]
public class HeapBinario<T> {
   // Arreglo auxiliar para almacenar la posicion de cada elemento en el heap.
   private Dictionary<int, int> posicion;
   // Heap implementado mediante un arreglo de elementos.
   private T[] heap;
   // Cantidad actual de elementos del heap.
   public int tamano;
   // Cantidad maxima de elementos que puede almacenar el heap.
   public int max_tamano;

   // Operacion de comparacion entre elementos.
   public delegate int comparar(T e1, T e2);
   public comparar comparador;

   public delegate int clave(T e1);
   public clave getClave;

   public HeapBinario(int maxtam, comparar comp, clave cv) {
	  max_tamano = maxtam;
	  tamano = 0;
	  heap = new T[max_tamano];
	  posicion = new Dictionary<int, int>(max_tamano);
	  getClave = cv;
	  comparador = comp;
   }

   public HeapBinario(T[] entrada, comparar comp, clave cv) {
	  max_tamano = entrada.Length;
	  tamano = max_tamano;
	  heap = new T[max_tamano];
	  posicion = new Dictionary<int, int>(max_tamano);
	  getClave = cv;
	  for (int i = 0; i < max_tamano; i++) {
		 heap[i] = entrada[i];
		 posicion.Add(getClave(entrada[i]), i);
	  }
	  comparador = comp;
	  heapify();
   }

   public void insertar(T elemento) {
	  if (tamano < max_tamano) {
		 heap[tamano] = elemento;
		 tamano++;
		 posicion.Add(getClave(elemento), tamano - 1);
		 percolate(tamano - 1);
	  }
	  else {
		 throw new Exception("No es posible insertar mas elementos, el heap esta lleno." + " (" + heap.Length + ")");
	  }
   }

   public void modificar(int indice, T nuevo_elemento) {
	  heap[indice] = nuevo_elemento;
	  heapify();
   }

   public T suprimirMinimo() {
	  if (tamano > 0) {
		 T aux = heap[0];
		 heap[0] = heap[tamano - 1];
		 tamano--;
		 posicion.Remove(getClave(aux));
		 posicion[getClave(heap[0])] = 0;
		 siftDown(0, tamano - 1);
		 return aux;
	  }
	  else {
		 throw new Exception("No es posible elimiar el minimo, el heap esta vacio.");
	  }
   }

   public T getMinimo() {
	  if (tamano > 0) {
		 return heap[0];
	  }
	  else {
		 throw new Exception("No es posible devolver el minimo, el heap esta vacio.");
	  }
   }

   private void percolate(int inicio) {
	  int i = inicio;
	  bool corte = false;
	  while ((i > 0) && (!corte)) {
		 if (comparador(heap[((i - 1) / 2)], heap[i]) > 0) {
			intercambiar(ref heap, ((i - 1) / 2), i);
			i = ((i - 1) / 2);
		 }
		 else {
			corte = true;
		 }
	  }
   }

   private void siftDown(int inicio, int fin) {
	  int raiz, hijo;

	  raiz = inicio;
	  while ((raiz * 2) + 1 <= fin) {
		 hijo = (raiz * 2) + 1;
		 if (hijo < fin) {
			if (comparador(heap[hijo], heap[hijo + 1]) > 0) {
			   hijo++;
			}
		 }
		 if (hijo <= fin) {
			if (comparador(heap[raiz], heap[hijo]) > 0) {
			   intercambiar(ref heap, raiz, hijo);
			}
		 }
		 raiz = hijo;
	  }
   }

   private void heapify() {
	  int i = tamano / 2;
	  while (i >= 0) {
		 siftDown(i, tamano - 1);
		 i--;
	  }
   }

   public T getValor(int i) {
	  if (i < tamano) {
		 return heap[i];
	  }
	  else {
		 throw new Exception("Indice fuera de rango.");
	  }
   }

   public void setValor(int i, T elemento) {
	  if (i < tamano) {
		 heap[i] = elemento;
		 posicion[getClave(elemento)] = i;
	  }
	  else {
		 throw new Exception("Indice fuera de rango.");
	  }
   }

   public bool existeIndice(int clave) {
	  int valor;
	  return posicion.TryGetValue(clave, out valor);
   }

   public int getIndice(int clave) {
	  int valor;
	  if (posicion.TryGetValue(clave, out valor)) {
		 return valor;
	  }
	  else {
		 throw new Exception("No existe elemento con la clave dada.");
	  }
   }

   private void intercambiar(ref T[] elementos, int i1, int i2) {
	  int longitud = elementos.Length;

	  if ((i1 >= 0) && (i1 < longitud) && (i2 < longitud) && (i2 >= 0)) {
		 T aux = elementos[i1];
		 elementos[i1] = elementos[i2];
		 elementos[i2] = aux;
		 posicion[getClave(elementos[i2])] = i2;
		 posicion[getClave(elementos[i1])] = i1;
	  }
	  else {
		 throw new Exception("Los indices estan fuera de rango.");
	  }
   }

   public string imprimirHeapBinario() {
	  string resultado = "";
	  foreach (int i in posicion.Keys) {
		 T elemento = heap[posicion[i]];
		 resultado += "(" + posicion[i] + ")" + elemento.ToString() + ", ";
	  }
	  return resultado;
   }
}
