using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using PathRuntime;

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

   public object SetObjectData(object obj, SerializationInfo info, StreamingContext ctxt, ISurrogateSelector selector) {
	  Vector2 vector2 = (Vector2)obj;
	  vector2.x = (float)info.GetValue("Vector2X", typeof(float));
	  vector2.y = (float)info.GetValue("Vector2Y", typeof(float));
	  return vector2;
   }
}

public sealed class HashSetSurrogate<T> : ISerializationSurrogate {
   public void GetObjectData(object obj, SerializationInfo info, StreamingContext ctxt) {
	  HashSet<T> hashset = (HashSet<T>)obj;

	  T[] obj_aux = new T[hashset.Count];
	  int i = 0;
	  foreach (T objeto in hashset) {
		 obj_aux[i] = objeto;
	  }
	  info.AddValue("Elementos", obj_aux);
   }

   public object SetObjectData(object obj, SerializationInfo info, StreamingContext ctxt, ISurrogateSelector selector) {
	  //HashSet<T> hashset = (HashSet<T>)obj;
	  HashSet<T> hashset = new HashSet<T>();

	  T[] obj_aux = (T[])info.GetValue("Elementos", typeof(T[]));
	  hashset.UnionWith(obj_aux);
	  return hashset;
   }
}

public class Serializador {
   public Serializador() {

   }

   public void Serializar(string nombre_archivo, Objeto_Serializable objeto) {
	  //Stream stream = File.Open(nombre_archivo + ".zip", FileMode.Create);
	  BinaryFormatter bFormatter = new BinaryFormatter();

	  Ionic.Zip.ZipOutputStream zipstream = new Ionic.Zip.ZipOutputStream(nombre_archivo + ".zip");
	  zipstream.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
	  zipstream.CompressionMethod = Ionic.Zip.CompressionMethod.BZip2;
	  zipstream.PutNextEntry("data_comprimida");

	  bFormatter.SurrogateSelector = cargarSurrogates();
	  try {
		 bFormatter.Serialize(zipstream, objeto);
	  }
	  catch (System.Exception excp) {
		 zipstream.Close();
		 //stream.Close();
		 throw excp;
	  }

	  zipstream.Close();
	  //stream.Close();
   }

   public Objeto_Serializable Deserializar(string nombre_archivo) {
	  Objeto_Serializable objeto = new Objeto_Serializable();
	  //Stream stream = File.Open(nombre_archivo+".zip", FileMode.Open);

	  Ionic.Zip.ZipInputStream zipstream = new Ionic.Zip.ZipInputStream(nombre_archivo + ".zip");
	  zipstream.GetNextEntry();

	  BinaryFormatter bFormatter = new BinaryFormatter();

	  bFormatter.SurrogateSelector = cargarSurrogates();
	  try {
		 objeto = bFormatter.Deserialize(zipstream) as Objeto_Serializable;
	  }
	  catch (System.Exception excp) {
		 zipstream.Close();
		 //stream.Close();
		 throw excp;
	  }

	  zipstream.Close();
	  //stream.Close();
	  return objeto;
   }

   private SurrogateSelector cargarSurrogates() {
	  SurrogateSelector surrogate_sel = new SurrogateSelector();
	  surrogate_sel.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
	  surrogate_sel.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), new Vector2Surrogate());
	  surrogate_sel.AddSurrogate(typeof(HashSet<int>), new StreamingContext(StreamingContextStates.All), new HashSetSurrogate<int>());
	  return surrogate_sel;
   }
}
