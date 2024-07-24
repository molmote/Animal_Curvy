using UnityEngine;
using System.Collections;
using System.IO;

using System;

//  using UnionAssets.FLE;
// TODO : make this static
public class DataSaver
{
	private static DataSaver _instance = new DataSaver();
	public static DataSaver instance 
	{
		get
		{
			return _instance; 
		}
	}

    bool startLoadFromCloud = false;

    string      cloudNameBeingLoaded;
    Texture2D   screenShot;

	public static void Save(string jsonString, string path)
	{
        byte []byteArray = null;
        try 
        {
            Debug.Log("Save Data: " + jsonString);
            
            byteArray           = GetBytes(jsonString);
        }
        catch(System.Exception e)
        {
            // MyLogger.GA_CreateHitTracking("DataSaver-Save", e.ToString(), false);
            MyLogger.LogError("DEBUG", "DataSaver Save ERROR: ");
            return;
        }

        FileIOController.instance.WriteBytes(path, byteArray, true);
	}

    //<<-----------------------------------------------------------------------
    public static T Load<T>(string path) //where T : object
    {
        //string path             = "userInfo.txt";       
        try 
        {
            string json                 = FileIOController.instance.ReadText(path);

            MyLogger.LogObject("UserInfoEntity-json",json);
            
            if ( json != null )
            {
                // T user = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                // T user = MiniJSON.Json.Deserialize<T>(json);
                return default(T); 
            }
        }
        catch(System.Exception e)
        {
            return default(T) ;
        }
        return default(T) ;
    }

	private static byte[] GetBytes(string str)
	{
	    byte[] bytes 	= System.Text.Encoding.UTF8.GetBytes(str);
	    return bytes;
	}

	public static string GetString(byte[] bytes)
	{
	    return System.Text.Encoding.UTF8.GetString(bytes);
	}
}
