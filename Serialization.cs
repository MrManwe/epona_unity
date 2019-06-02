using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using epona;

public static class Serialization
{
    /*public Serialization()
    {
    }*/

	public static void saveLanguage(String fileName, String lang){

		Stream stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Binder = new VersionDeserializationBinder();

		formatter.Serialize(stream, lang);

		stream.Close();
		
	}

	public static String loadLanguage(String fileName){
		String lang = "en"; //Default language
		try
		{
			Stream stream = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.Open);
			BinaryFormatter bformatter = new BinaryFormatter();
			bformatter.Binder = new VersionDeserializationBinder();
			stream.Seek(0, SeekOrigin.Begin);

			lang = (String)bformatter.Deserialize(stream);

			stream.Close();
		}
		catch (FileNotFoundException e){
			//We do not want to report this exception
			Debug.Log("Language file not found" + e.ToString());
		}
		return lang;
	}
}
