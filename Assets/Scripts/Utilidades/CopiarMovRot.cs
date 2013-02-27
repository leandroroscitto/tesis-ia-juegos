using UnityEngine;
using System.Collections;

// Copia la posicion y/o rotacion de un target al objeto que contiene este script.
public class CopiarMovRot : MonoBehaviour {
   public Transform target;
   public bool copiarPosicion;
   public bool copiarRotacion;

   void Start() {

   }

   void Update() {
	  if (copiarPosicion)
		 transform.position = target.position;

	  if (copiarRotacion)
		 transform.rotation = target.rotation;
   }
}
