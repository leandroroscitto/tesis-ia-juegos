using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public sealed class Vector3Surrogate : ISerializationSurrogate {
   public void GetObjectData(object obj, SerializationInfo info, StreamingContext ctxt) {
	  Vector3 vector3 = (Vector3)obj;
	  info.AddValue("Vector3X", vector3.x);
	  info.AddValue("Vector3Y", vector3.y);
	  info.AddValue("Vector3Z", vector3.z);
   }

   public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
	  Vector3 vector3 = (Vector3)obj;
	  vector3.x = (float)info.GetValue("Vector3X", typeof(float));
	  vector3.y = (float)info.GetValue("Vector3Y", typeof(float));
	  vector3.z = (float)info.GetValue("Vector3Z", typeof(float));
	  return vector3;
   }
}

public sealed class Vector2Surrogate : ISerializationSurrogate {
   public void GetObjectData(object obj, SerializationInfo info, StreamingContext ctxt) {
	  Vector2 vector2 = (Vector2)obj;
	  info.AddValue("Vector2X", vector2.x);
	  info.AddValue("Vector2Y", vector2.y);
   }

   public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
	  Vector2 vector2 = (Vector2)obj;
	  vector2.x = (float)info.GetValue("Vector2X", typeof(float));
	  vector2.y = (float)info.GetValue("Vector2Y", typeof(float));
	  return vector2;
   }
}

public class Serializador {
   public Serializador() {

   }

   public void SerializarResolucion(string nombre_archivo, Objeto_Serializable objeto) {
	  Stream stream = File.Open(nombre_archivo, FileMode.Create);
	  BinaryFormatter bFormatter = new BinaryFormatter();

	  bFormatter.SurrogateSelector = cargarSurrogates();

	  bFormatter.Serialize(stream, objeto);
	  stream.Close();
   }

   public Objeto_Serializable DeserializarResolucion(string nombre_archivo) {
	  Objeto_Serializable objeto;
	  Stream stream = File.Open(nombre_archivo, FileMode.Open);
	  BinaryFormatter bFormatter = new BinaryFormatter();

	  bFormatter.SurrogateSelector = cargarSurrogates();

	  objeto = bFormatter.Deserialize(stream) as Objeto_Serializable;
	  stream.Close();
	  return objeto;
   }

   public SurrogateSelector cargarSurrogates() {
	  SurrogateSelector surrogate_sel = new SurrogateSelector();
	  surrogate_sel.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
	  surrogate_sel.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), new Vector2Surrogate());
	  return surrogate_sel;
   }
}
