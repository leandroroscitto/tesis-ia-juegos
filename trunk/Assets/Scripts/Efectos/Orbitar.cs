using UnityEngine;
using System.Collections;

// Da una animacion de giro y/o orbita a un objeto.
public class Orbitar : MonoBehaviour {
   // Eje de rotacion para el giro.
   public Vector3 eje_rotacion;
   // Velocidad de rotacion para el giro.
   public float velocidad_rotacion;

   // Objeto al cual se orbita.
   public Transform objeto_orbita;
   // Eje de la orbita.
   public Vector3 eje_orbita;
   // Velocidad de la orbita.
   public float velocidad_orbita;

   void Start() {

   }

   void Update() {
	  // Rotacion
	  transform.RotateAround(transform.rotation * eje_rotacion, (velocidad_rotacion / 36f) * Time.deltaTime);

	  // Orbita
	  if (objeto_orbita != null)
		 transform.RotateAround(objeto_orbita.position, objeto_orbita.rotation * eje_orbita, (velocidad_orbita / 36f) * Time.deltaTime);
   }
}
