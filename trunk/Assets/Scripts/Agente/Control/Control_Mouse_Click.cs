using UnityEngine;
using System.Collections;
using PathRuntime;

public class Control_Mouse_Click : MonoBehaviour
{
	//private Vehicle agente;
	private SteerForPathSimplified pathfollow;
	public Navigator navigator;
	public Transform modelo;
	public GameObject puntero;
	private Path camino_actual;
	
	void OnDrawGizmos ()
	{
		if (camino_actual != null) {
			foreach (Connection coneccion in camino_actual.Segments) {
				Gizmos.DrawLine (coneccion.From.Position, coneccion.To.Position);
			}
		}
	}
	
	void Start ()
	{
		//agente = GetComponent<Vehicle> ();
		pathfollow = GetComponent<SteerForPathSimplified> ();
		navigator = GetComponent<Navigator> ();
		
		puntero = GameObject.CreatePrimitive (PrimitiveType.Capsule);
		Destroy (puntero.GetComponent<Collider> ());
		puntero.name = "Puntero";
	}
	
	void Update ()
	{
		if (Input.GetMouseButton (0)) {
			Ray ray = Camera.mainCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100f, 1 << LayerMask.NameToLayer ("NoPasable"))) {
				puntero.transform.position = hit.point;
			}
		}
		
		navigator.targetPosition = puntero.transform.position + Vector3.up * 0.25f;
	}
	
	public void OnNewPath (Path path)
	{
		camino_actual = path;
		
		UnitySteer.Vector3Pathway v3path = new UnitySteer.Vector3Pathway ();
		v3path.AddPoint (path.StartPosition);
		foreach (Connection segment in path.Segments) {
			v3path.AddPoint (segment.From.Position);
		}
		v3path.AddPoint (path.EndPosition);
		
		pathfollow.Path = v3path;
	}
}
