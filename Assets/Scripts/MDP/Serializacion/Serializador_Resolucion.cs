using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Serializador_Resolucion {
   public Serializador_Resolucion() {

   }

   public void SerializarResolucion(string nombre_archivo, Resolucion_Serializable resolucion) {
	  Stream stream = File.Open(nombre_archivo, FileMode.Create);
	  BinaryFormatter bFormatter = new BinaryFormatter();
	  bFormatter.Serialize(stream, resolucion);
	  stream.Close();
   }

   public Resolucion_Serializable DeserializarResolucion(string nombre_archivo) {
	  Resolucion_Serializable resolucion;
	  Stream stream = File.Open(nombre_archivo, FileMode.Open);
	  BinaryFormatter bFormatter = new BinaryFormatter();
	  resolucion = bFormatter.Deserialize(stream) as Resolucion_Serializable;
	  stream.Close();
	  return resolucion;
   }
}
