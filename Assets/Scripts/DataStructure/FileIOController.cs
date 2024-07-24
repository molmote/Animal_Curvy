using UnityEngine;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FileIOController 
{
	private static FileIOController _instance = new FileIOController();
	public  static FileIOController instance
	{
		get
		{
			return _instance;
		}
	}

	//!<<----------------------------------------------------------------------
	public byte[] ReadBytes(string path)
	{
		try 
		{
			string fullpath = Application.persistentDataPath+"/"+path;
			byte[] buffer 	= null;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
	        FileStream fs 	= new FileStream(fullpath, FileMode.Open, FileAccess.Read);
	        BinaryReader br = new BinaryReader(fs);
	        long numBytes 	= new FileInfo(fullpath).Length;
	        buffer 			= br.ReadBytes((int) numBytes);
	        fs.Close();
#endif
	        return buffer;
		}
		catch(System.Exception e)
		{
			MyLogger.Red("Error",e.ToString());
			MyLogger.LogError("DEBUG", "ReadText ERROR: " + Application.persistentDataPath+"/"+path);
		}

		return null;
	}

	//!<<----------------------------------------------------------------------
	public void WriteBytes(string filename, byte[] byteArray, bool resourcesDB )
	{
		try
	    {	       
	       // Open file for reading
	       System.IO.FileStream stream = 
	          new System.IO.FileStream(Application.persistentDataPath+"/"+filename, System.IO.FileMode.Create,
	                                   System.IO.FileAccess.Write);
	       // Writes a block of bytes to this stream using data from
	       // a byte array.
	       // MyLogger.LogObject("WriteBytes", "PREWRITE" + byteArray);
	       stream.Write(byteArray, 0, byteArray.Length);
	       // MyLogger.LogObject("WriteBytes", "AFTERWRITE" + byteArray);
	       // close file stream
	       stream.Close();
	       stream = null;
	    }
	    catch (System.Exception exception)
	    {
	       // Error
	       MyLogger.Log("DEBUG", "Exception caught in process: " + exception.ToString());
	    }
	}


	//!<<----------------------------------------------------------------------
	public string ReadText(string path)
	{
		try 
		{
			string readText = "";

			MyLogger.Log("DEBUG", "PATH: " + Application.persistentDataPath + "/" + path);

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
			readText = File.ReadAllText(Application.persistentDataPath + "/" + path);
#endif

			return readText;
		}
		catch(System.Exception e)
		{
			MyLogger.LogError("DEBUG", "ReadText ERROR!");
		}
		return null;
	}

	//!<<----------------------------------------------------------------------
	public string LoadText(string path, bool ignoreException = false)
	{
		try 
		{
			//MyLogger.Log("DEBUG", "PATH: " + Application.dataPath + "/" + path);

			TextAsset textAsset = Resources.Load(path) as TextAsset;

			return textAsset.text;
		}
		catch(System.Exception e)
		{
			if( ignoreException == false )
			MyLogger.LogError("DEBUG", "ReadText ERROR!" + e);
		}
		return null;
	}



	public string LoadCSV(string csvFilePath)
	{
		string csvString 	= this.LoadText(csvFilePath);

		string [] lines 	= csvString.Split(new char[] { '\n', '\r' });//, StringSplitOptions.RemoveEmptyEntries);
		string [] cols 		= lines[0].Split(',');
		/*IEnumerable<Dictionary<string,string>> csv = lines.Skip(1)
		               .Select(l => l.Split(',')
		                             .Select((s, i) => new {s,i})
		                             .ToDictionary(x=>cols[x.i],x=>x.s));*/
  		MyLogger.Log(lines[0]);
  		MyLogger.Log(lines[1]);
  		MyLogger.Log(lines[2]);

  		//MyLogger.LogObject(cols);
		List<Dictionary<string,string>> csv = new List<Dictionary<string,string>>();
		for ( int i = 0 ; i < lines.Length ; i++ )
		{
			for ( int j = 0 ; j < cols[i].Length ; j++ )
			{

			}
		}

        // string jsonString 	= Newtonsoft.Json.JsonConvert.SerializeObject(csv);//, Formatting.None, null);

        // return jsonString;
        return null;
	}
}
