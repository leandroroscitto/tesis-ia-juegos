using UnityEngine;
using System.Collections;

// Permite controlar la camara. (DESACTUALIZADO)
// CONTROLES: 
//	  R: Activa/Desactiva la rotacion alrededor del target.
//	  F: Activa/Desactiva el seguimiento del target.
//	  WASD: En el modo libre, permite cambiar la posicion de la camara.
//	  Mouse: Rotacion.
//	  Rueda del mouse / Z|C: Zoom in/out.
public class Camara_3Persona : MonoBehaviour
{
	// Argumentos
	public float velocidadX = 800;
	public float velocidadY = 400;
	public float maxAnguloY = 85;
	public float minAnguloY = -85;
	public Vector3 offset = Vector3.zero;
	public float distancia = 1f;
	
	// Objeto al cual sigue la camara
	public Transform target;
	
	// Camara
	public Camera camara;
	
	// Parametros
	private Vector2 angulo;

	void Start ()
	{
		angulo.x = transform.eulerAngles.y;
		angulo.y = transform.eulerAngles.x;
	}

	void Update ()
	{
		if ((Input.GetAxis ("Mouse ScrollWheel") < 0) || (Input.GetKey (KeyCode.C)))
			distancia += 0.5f;
		else if ((Input.GetAxis ("Mouse ScrollWheel") > 0) || (Input.GetKey (KeyCode.Z)))
			distancia -= 0.5f;

		angulo.x += Input.GetAxis ("Mouse X") * velocidadX * 0.02f;
		angulo.y -= Input.GetAxis ("Mouse Y") * velocidadY * 0.02f;
		angulo.y = ClampAngle (angulo.y, minAnguloY, maxAnguloY);
		transform.rotation = Quaternion.Euler(angulo.y, angulo.x, 0);

		if (target != null) {
			camara.transform.localPosition = offset * distancia;
			transform.position = Vector3.Lerp (transform.position, target.position, Time.deltaTime * 25);
		}
	}

	private float ClampAngle (float angulo, float min, float max)
	{
		angulo = angulo % 360;
		return Mathf.Clamp (angulo, min, max);
	}
}
