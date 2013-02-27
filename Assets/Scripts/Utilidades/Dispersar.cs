using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Dado un objeto, instancia copias en posiciones al azar de acuerdo a los parametros de entrada.
public class Dispersar : MonoBehaviour {
   // Objeto a instanciar.
   public GameObject objeto;

   // Label que se le aplica a las copias.
   public string label;
   // Layer a la que pertenencen las copias.
   public string layer;
   // Cantidad de copias a crear.
   public int cantidad;
   // Distancia minima en la cual se ubicara una copia.
   public float distancia_minima;
   // Distancia maxima en la cual se ubicara una copia.
   public float distancia_maxima;
   // Tamano minimo de las copias.
   public float tamano_min;
   // Tamano maximo de las copias.
   public float tamano_max;
   // Objeto que sirve de contenedor.
   public Transform padre;

   // Auxiliar usado para detectar cambios.
   private int cantidad_anterior;
   // Lista de objetos.
   private List<GameObject> objetos;

   // Escala de la distribucion. Permite modificar el aspecto de la esfera de dispercion.
   public Vector3 escala;

   void Start() {
	  cantidad_anterior = 0;
	  objetos = new List<GameObject>(cantidad);
   }

   void Update() {
	  Vector3 orbita;
	  Quaternion rotacion;

	  // Agrega nuevas instancias si faltan.
	  for (int i = 0; i < (cantidad - cantidad_anterior); i++) {
		 rotacion = Random.rotation;

		 orbita = Random.onUnitSphere * Random.Range(distancia_minima, distancia_maxima);

		 orbita = Vector3.Scale(orbita, escala);

		 GameObject instancia = (GameObject)Instantiate(objeto, transform.position + orbita, rotacion);
		 if (padre != null)
			instancia.transform.parent = padre;

		 instancia.SetActive(true);

		 instancia.tag = (label != "") ? label : "Untagged";
		 instancia.layer = (layer != "") ? LayerMask.NameToLayer(layer) : LayerMask.NameToLayer("Default");

		 instancia.name = objeto.name + "_" + i.ToString();
		 instancia.transform.localScale = Vector3.one * Random.Range(tamano_min, tamano_max);

		 objetos.Add(instancia);
	  }

	  // Elimina instancias si sobran.
	  for (int i = 0; i < (cantidad_anterior - cantidad); i++) {
		 GameObject instancia = objetos[objetos.Count - 1];
		 objetos.Remove(instancia);
		 Destroy(instancia);
	  }

	  cantidad_anterior = cantidad;
   }
}
