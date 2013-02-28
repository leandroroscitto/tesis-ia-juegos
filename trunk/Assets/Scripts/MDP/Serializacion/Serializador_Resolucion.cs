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


public class Serializador_Resolucion {
   public Serializador_Resolucion() {

   }

   public void SerializarResolucion(string nombre_archivo, Resolucion_Serializable resolucion) {
	  Stream stream = File.Open(nombre_archivo, FileMode.Create);
	  BinaryFormatter bFormatter = new BinaryFormatter();

	  SurrogateSelector surrogate_sel = new SurrogateSelector();
	  surrogate_sel.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
	  bFormatter.SurrogateSelector = surrogate_sel;

	  bFormatter.Serialize(stream, resolucion);
	  stream.Close();
   }

   public Resolucion_Serializable DeserializarResolucion(string nombre_archivo) {
	  Resolucion_Serializable resolucion;
	  Stream stream = File.Open(nombre_archivo, FileMode.Open);
	  BinaryFormatter bFormatter = new BinaryFormatter();

	  SurrogateSelector surrogate_sel = new SurrogateSelector();
	  surrogate_sel.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
	  bFormatter.SurrogateSelector = surrogate_sel;

	  resolucion = bFormatter.Deserialize(stream) as Resolucion_Serializable;
	  stream.Close();
	  return resolucion;
   }
}
