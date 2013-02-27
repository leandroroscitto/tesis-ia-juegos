using UnityEngine;
using System.Collections;

// Permite controlar la camara.
// CONTROLES:
//	  R: Activa/Desactiva la rotacion alrededor del target.
//	  F: Activa/Desactiva el seguimiento del target.
//	  WASD: En el modo libre, permite cambiar la posicion de la camara.
//	  Mouse: Rotacion.
//	  Rueda del mouse / Z|C: Zoom in/out.
public class Camara_Mouse : MonoBehaviour {
   public float velocidadX, velocidadY;
   public float maxAnguloY, minAnguloY;
   public Transform target;
   public bool seguir = true;

   public Vector3 offset;

   public Camera camara;

   private float aceleracion_movimiento = 0;

   private float distancia = 10f;
   public bool rotar = true;
   private Vector2 angulo;

   void Start() {
	  angulo.x = transform.eulerAngles.y;
	  angulo.y = transform.eulerAngles.x;
   }

   void LateUpdate() {
	  if ((Input.GetAxis("Mouse ScrollWheel") < 0) || (Input.GetKey(KeyCode.C)))
		 distancia += 0.5f;
	  else if ((Input.GetAxis("Mouse ScrollWheel") > 0) || (Input.GetKey(KeyCode.Z)))
		 distancia -= 0.5f;

	  if (Input.GetKeyDown(KeyCode.F))
		 seguir = !seguir && (target != null);

	  if (Input.GetKeyDown(KeyCode.R))
		 rotar = !rotar;

	  if ((Input.GetKey(KeyCode.LeftAlt) || (rotar)) && (!Input.GetMouseButton(1))) {
		 angulo.x += Input.GetAxis("Mouse X") * velocidadX * 0.02f;
		 angulo.y -= Input.GetAxis("Mouse Y") * velocidadY * 0.02f;

		 angulo.y = ClampAngle(angulo.y, minAnguloY, maxAnguloY);
	  }

	  transform.rotation = Quaternion.Euler(angulo.y, angulo.x, 0);

	  if (seguir && (target != null)) {
		 camara.transform.localPosition = offset * distancia;
		 transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 25);
	  }
	  else {
		 Vector3 direccion = Vector3.zero;

		 if (Input.GetKey(KeyCode.W))
			direccion += Vector3.forward;
		 if (Input.GetKey(KeyCode.S))
			direccion += Vector3.back;
		 if (Input.GetKey(KeyCode.A))
			direccion += Vector3.left;
		 if (Input.GetKey(KeyCode.D))
			direccion += Vector3.right;

		 if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
			aceleracion_movimiento += Time.deltaTime;
		 else
			aceleracion_movimiento -= Time.deltaTime;

		 aceleracion_movimiento = Mathf.Max(Mathf.Min(aceleracion_movimiento, 2), 0);

		 transform.position = camara.transform.position;
		 camara.transform.localPosition = Vector3.zero;
		 transform.position += transform.rotation * direccion * (aceleracion_movimiento + 1);
	  }
   }

   private float ClampAngle(float angulo, float min, float max) {
	  angulo = angulo % 360;
	  return Mathf.Clamp(angulo, min, max);
   }
}
