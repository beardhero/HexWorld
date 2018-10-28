using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;
using System;

public class BinaryHandler {

/**
* Helper function to serialize and write data to disk.
*/
  public static void WriteData<T>(T data, string path) {
    Stream stream = File.Open(path, FileMode.Create);
    BinaryFormatter bformatter = new BinaryFormatter();
    bformatter.Binder = new VersionDeserializationBinder();
    bformatter.Serialize(stream, data);
    stream.Close();
  }

  /**
   * Helper function to read and unserialize data from disk.
   */
  public static T ReadData<T>(string path) where T : new()
  {
    // Declare the hashtable reference.
    T output  = new T();

    // Open the file containing the data that you want to deserialize.
    FileStream ffs = new FileStream(path, FileMode.Open);

    try 
    {
      BinaryFormatter formatter = new BinaryFormatter();

      // Deserialize the hashtable from the file and 
      // assign the reference to the local variable.
      output = (T) formatter.Deserialize(ffs);
    }
    catch (SerializationException e) 
    {
      Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
      throw;
    }
    finally 
    {
      ffs.Close();
    }

    return output;
  }


  public sealed class VersionDeserializationBinder : SerializationBinder
  {
      public override Type BindToType( string assemblyName, string typeName )
      {
          if ( !string.IsNullOrEmpty( assemblyName ) && !string.IsNullOrEmpty( typeName ) )
          {
              Type typeToDeserialize = null;

              assemblyName = Assembly.GetExecutingAssembly().FullName;

              // The following line of code returns the type.
              typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) );

              return typeToDeserialize;
          }

          return null;
      }
  }
}
