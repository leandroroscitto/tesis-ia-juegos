using UnityEngine;
using System.Collections;

// Crea una caja con 6 parades que sirve de limites para el escenario.
// Cada pared tiene un box collider.
public class Crear_Limites : MonoBehaviour {
   // Centro de la caja.
   public Vector3 centro;
   // Tamano de la caja.
   public Vector3 tamano;
   // Tag que se le aplica a cada pared.
   public string tag_nombre;
   // Layer a la que se incluye cada pared.
   public string layer_nombre;

   // Grosor de cada pared.
   public float grosor_default;

   // Determina si se dibuja o no las paredes en ejecucion.
   public bool dibujar;

   private GameObject limites, izquierda, derecha, adelante, atras, arriba, abajo;

   void Start() {
	  limites = new GameObject("Limites");
	  limites.transform.parent = transform;

	  izquierda = GameObject.CreatePrimitive(PrimitiveType.Cube);
	  izquierda.name = "LIzquierdo";
	  derecha = GameObject.CreatePrimitive(PrimitiveType.Cube);
	  derecha.name = "LDerecho";
	  adelante = GameObject.CreatePrimitive(PrimitiveType.Cube);
	  adelante.name = "LAdelante";
	  atras = GameObject.CreatePrimitive(PrimitiveType.Cube);
	  atras.name = "LAtras";
	  arriba = GameObject.CreatePrimitive(PrimitiveType.Cube);
	  arriba.name = "LArriba";
	  abajo = GameObject.CreatePrimitive(PrimitiveType.Cube);
	  abajo.name = "LAbajo";

	  izquierda.transform.parent = derecha.transform.parent = adelante.transform.parent = atras.transform.parent = arriba.transform.parent = abajo.transform.parent = limites.transform;
   }

   void Update() {
	  izquierda.transform.position = centro + Vector3.left * (tamano.x + grosor_default) / 2;
	  derecha.transform.position = centro + Vector3.right * (tamano.x + grosor_default) / 2;
	  atras.transform.position = centro + Vector3.back * (tamano.z + grosor_default) / 2;
	  adelante.transform.position = centro + Vector3.forward * (tamano.z + grosor_default) / 2;
	  arriba.transform.position = centro + Vector3.up * (tamano.y + grosor_default) / 2;
	  abajo.transform.position = centro + Vector3.down * (tamano.y + grosor_default) / 2;

	  izquierda.transform.localScale = derecha.transform.localScale = Vector3.right * grosor_default + Vector3.forward * tamano.z + Vector3.up * tamano.y;
	  atras.transform.localScale = adelante.transform.localScale = Vector3.right * tamano.x + Vector3.forward * grosor_default + Vector3.up * tamano.y;
	  arriba.transform.localScale = abajo.transform.localScale = Vector3.right * tamano.x + Vector3.forward * tamano.z + Vector3.up * grosor_default;

	  izquierda.renderer.enabled = derecha.renderer.enabled = adelante.renderer.enabled = atras.renderer.enabled = arriba.renderer.enabled = abajo.renderer.enabled = dibujar;

	  izquierda.tag = derecha.tag = adelante.tag = atras.tag = arriba.tag = abajo.tag = tag_nombre;
	  izquierda.layer = derecha.layer = adelante.layer = atras.layer = arriba.layer = abajo.layer = LayerMask.NameToLayer(layer_nombre);
   }
}
