using UnityEngine;
using System.Collections;

public class Pointer : MonoBehaviour {
   public float duracion;
   public Vector3 posicion_base;
   public Vector3 rotacion_base;
   public float velocidad_rotacion;
   public float velocidad_suspencion;
   public float radio_suspencion;

   public Vector3 eje;

   public bool tomar_posicion_actual;

   void Start() {
	  if (tomar_posicion_actual) {
		 posicion_base = transform.position + eje.normalized * radio_suspencion;
		 rotacion_base = transform.eulerAngles;
	  }
   }

   void Update() {
	  duracion -= Time.deltaTime;
	  if (duracion > 0) {
		 transform.eulerAngles = eje.normalized * ((Time.time * 60 * velocidad_rotacion) % 360) + rotacion_base;
		 transform.position = eje.normalized * Mathf.Sin(Time.time * velocidad_suspencion) * radio_suspencion + posicion_base;
	  }
	  else {
		 Destroy(gameObject);
	  }
   }
}
