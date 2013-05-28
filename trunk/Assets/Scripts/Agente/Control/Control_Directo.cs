using UnityEngine;
using System.Collections;

// Permite controlar un agente por medio de teclado o joystick.
public class Control_Directo : MonoBehaviour {
   // Agente
   public Transform modelo;
   private Vehicle agente;
   private SteerForPoint steerForPoint;

   // Movimiento
   public bool invertir_horizontal, invertir_vertical;
   public float rango;
   private float eje_horizontal, eje_vertical;

   // Camara
   public bool movimiento_relativo_camara;
   public Camara_3Persona camara_3persona;
   private float distancia_camara = 1.0f;

   // Accion
   public Transform target_base;
   public Transform target;
   public Strobe flash_light;
   public ParticleSystem flash;
   public ParticleSystem debris;
   public LayerMask no_pasable;

   void OnGUI() {
	  //eje_horizontal = GUI.HorizontalSlider(Rect.MinMaxRect(5, 100, 205, 110), eje_horizontal, -1, 1);
	  //eje_vertical = GUI.VerticalSlider(Rect.MinMaxRect(100, 5, 110, 205), eje_vertical, -1, 1);
   }

   void Start() {
	  agente = GetComponent<Vehicle>();
	  steerForPoint = GetComponent<SteerForPoint>();

	  camara_3persona = GameObject.Find("Camara Principal").GetComponent<Camara_3Persona>();
	  camara_3persona.target = transform;
   }

   void Update() {
	  getMovimiento();
	  getAceleracion();
	  getAccion();

	  setFoco();
	  setMovimiento();
   }

   private void setFoco() {
	  target.position = modelo.position + Vector3.up * modelo.localScale.y / 2 + camara_3persona.camara.transform.rotation * Vector3.forward * 4.5f;
	  target.eulerAngles = camara_3persona.camara.transform.eulerAngles - Vector3.right * 90;
   }

   private void getMovimiento() {
	  eje_horizontal = Input.GetAxis("Horizontal") * rango;
	  eje_vertical = Input.GetAxis("Vertical") * rango;

	  if (invertir_horizontal) {
		 eje_horizontal *= -1;
	  }
	  if (invertir_vertical) {
		 eje_vertical *= -1;
	  }
   }

   private void setMovimiento() {
	  if (movimiento_relativo_camara) {
		 steerForPoint.TargetPoint = agente.Position + Camera.mainCamera.transform.rotation * new Vector3(eje_horizontal, 0, eje_vertical);
	  }
	  else {
		 steerForPoint.TargetPoint = agente.Position + new Vector3(eje_horizontal, 0, eje_vertical);
	  }
   }

   private void getAceleracion() {
	  if (Input.GetKeyDown(KeyCode.LeftShift)) {
		 agente.MaxSpeed *= 2;
		 distancia_camara = 0.5f;
	  }
	  if (Input.GetKeyUp(KeyCode.LeftShift)) {
		 agente.MaxSpeed /= 2;
		 distancia_camara = 1.0f;
	  }
	  camara_3persona.distancia = Mathf.Lerp(camara_3persona.distancia, distancia_camara, Time.deltaTime);
   }

   private void getAccion() {
	  if (Input.GetMouseButton(0)) {
		 flash.Play(true);
		 flash_light.enabled = true;
		 RaycastHit hit;
		 if (Physics.Raycast(flash.transform.position, target.position - target_base.position, out hit, 10, no_pasable.value)) {
			ParticleSystem debris_i = (ParticleSystem)Instantiate(debris, hit.point, Quaternion.LookRotation(hit.normal));
			debris_i.Play();
			Destroy(debris_i.gameObject, debris_i.duration);
		 }
	  }
	  else {
		 flash.Stop(true);
		 flash_light.light.enabled = false;
		 flash_light.enabled = false;
	  }
   }
}
