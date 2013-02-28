using UnityEngine;
using System.Collections;

public class Controlador_Animaciones : MonoBehaviour {
   public GameObject agent_model;
   public bool mantener_rotacion;
   public AudioClip footstep;

   private AudioSource agente_audio_source;
   private Vehicle agente;

   void Start() {
	  agente = GetComponent<Vehicle>();
	  agente_audio_source = gameObject.AddComponent<AudioSource>();
	  agente_audio_source.playOnAwake = false;
	  agente_audio_source.loop = true;
	  agente_audio_source.volume = 0.05f;
	  agente_audio_source.pitch = 0.48f;
	  agente_audio_source.spread = 10f;
   }

   void Update() {
	  if (mantener_rotacion) {
		 Vector3 eulers = Camera.mainCamera.transform.eulerAngles;
		 agent_model.transform.eulerAngles = new Vector3(0, eulers.y, 0);
	  }
	  if (agente.Speed > agente.MaxSpeed / 4) {
		 float velocidad_adelante = Mathf.Max(0, Vector3.Dot(agente.Velocity, agent_model.transform.rotation * Vector3.forward));
		 float velocidad_atras = Mathf.Max(0, Vector3.Dot(agente.Velocity, agent_model.transform.rotation * Vector3.back));
		 float velocidad_izquierda = Mathf.Max(0, Vector3.Dot(agente.Velocity, agent_model.transform.rotation * Vector3.left));
		 float velocidad_derecha = Mathf.Max(0, Vector3.Dot(agente.Velocity, agent_model.transform.rotation * Vector3.right));

		 agente_audio_source.clip = footstep;
		 if (!agente_audio_source.isPlaying) {
			agente_audio_source.Play();
		 }

		 agent_model.animation.Stop("idle");

		 agent_model.animation.Blend("run_forward", velocidad_adelante);
		 agent_model.animation.Blend("run_backward", velocidad_atras);
		 agent_model.animation.Blend("run_left", velocidad_izquierda);
		 agent_model.animation.Blend("run_right", velocidad_derecha);
		 agent_model.animation["run_forward"].speed = velocidad_adelante / (2 * agente.MaxSpeed);
		 agent_model.animation["run_backward"].speed = velocidad_atras / (2 * agente.MaxSpeed);
		 agent_model.animation["run_left"].speed = velocidad_izquierda / (2 * agente.MaxSpeed);
		 agent_model.animation["run_right"].speed = velocidad_derecha / (2 * agente.MaxSpeed);
	  }
	  else if (agente.Speed > 0) {
		 agent_model.animation.Blend("idle");
	  }
	  else {
		 agente_audio_source.Stop();

		 agent_model.animation.Stop("run_forward");
		 agent_model.animation.Stop("run_backward");
		 agent_model.animation.Stop("run_left");
		 agent_model.animation.Stop("run_right");
		 agent_model.animation.Play("idle", AnimationPlayMode.Queue);
	  }
   }

   public void Atacar() {
	  if (!agent_model.animation.IsPlaying("attack")) {
		 agent_model.animation.Play("attack", PlayMode.StopAll);
		 //agent_model.animation.Blend ("attack", 1.0f, 2.0f);	
	  }
   }
}
