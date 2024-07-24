using UnityEngine;
using System.Text;
using System.Collections;

public class MyLogger : MonoBehaviour
{

    public static bool logEnabled = true;

    public static void Assert(string tag, bool condition = false)
    {
        if (!logEnabled)
            return;

        if (!condition)
        {
            Debug.LogError("<color=red>[Assert] 			" + tag + " </color>");
        }
    }

    public static void Red(string tag, string log)
    {
        if (!logEnabled)
            return;

        Debug.Log("<color=red>[" + tag + "]  	   	" + log + "</color>");
    }

    public static void RedAbsolute(string tag, string log)
    {
#if USE_ABSOLUTE
		Debug.Log("<color=red>["+tag+"]  	   	" + log + "</color>");
#endif
    }

    public static void Blue(string tag, string log)
    {
        if (!logEnabled)
            return;

        Debug.Log("<color=aqua>[" + tag + "]  	   	" + log + "</color>");
    }

    public static void Green(string tag, string log)
    {
        if (!logEnabled)
            return;

        Debug.Log("<color=green>[" + tag + "]  	   	" + log + "</color>");
    }


    public static void Yellow(string tag, string log)
    {
        if (!logEnabled)
            return;

        Debug.Log("<color=yellow>[" + tag + "]  	   	" + log + "</color>");
    }


    public static void LogObject(string tag, object obj)
    {
        if (!logEnabled)
            return;

        Log(tag, Newtonsoft.Json.JsonConvert.SerializeObject(obj));
    }
    public static void RedObject(string tag, object obj)
    {
        if (!logEnabled)
            return;

        Red(tag, Newtonsoft.Json.JsonConvert.SerializeObject(obj));
    }
    public static void BlueObject(string tag, object obj)
    {
        if (!logEnabled)
            return;

        Blue(tag, Newtonsoft.Json.JsonConvert.SerializeObject(obj));
    }
    public static void GreenObject(string tag, object obj)
    {
        if (!logEnabled)
            return;

        Green(tag, Newtonsoft.Json.JsonConvert.SerializeObject(obj));
    }


    //!<<----------------------------------------------------------------------
    public static void Log(string tag, string log)
    {
        if (!logEnabled)
            return;

        Debug.Log("[" + tag + "]  	   	" + log);
    }
    //!<<----------------------------------------------------------------------	
    public static void Log(string tag, object log)
    {
        if (!logEnabled)
            return;

        Log(tag, log.ToString());
    }
    //!<<----------------------------------------------------------------------
    public static void Log(object log)
    {
        if (!logEnabled)
            return;

        Debug.Log(log);
    }
    //!<<----------------------------------------------------------------------
    public static void Log(string log)
    {
        if (!logEnabled)
            return;

        Debug.Log(log);
    }
    //!<<----------------------------------------------------------------------
    public static void LogWarning(string log)
    {
        if (!logEnabled)
            return;

        Debug.LogWarning(log);
    }
    //!<<----------------------------------------------------------------------	
    public static void LogWarning(object msg, UnityEngine.Object obj)
    {
        if (!logEnabled)
            return;

        Debug.LogWarning(msg, obj);
    }
    //!<<----------------------------------------------------------------------
    public static void LogWarning(string tag, string log)
    {
        if (!logEnabled)
            return;

        Debug.LogWarning("[" + tag + "]     " + log);
    }
    //!<<----------------------------------------------------------------------
    public static void LogError(string log)
    {
        if (!logEnabled)
            return;

        Debug.LogError(log);
    }
    //!<<----------------------------------------------------------------------
    public static void LogError(string tag, string log)
    {
        if (!logEnabled)
            return;

        Debug.LogError("[" + tag + "]     " + log);
    }

    public static void GA_CreateHitTracking(string tag, string log, bool fatal)
    {
        if (true)
        {
            //GoogleAnalytics.Client.CreateHit(GoogleAnalyticsHitType.EXCEPTION);
            //GoogleAnalytics.Client.SetExceptionDescription(tag + log);
           // GoogleAnalytics.Client.SetIsFatalException(fatal);

            //GoogleAnalytics.Client.Send();
        }
    }

    //!<<----------------------------------------------------------------------
    // helper to log Arraylists and Hashtables
    public static void logObject(object result)
    {
        if (!logEnabled)
            return;

        if (result.GetType() == typeof(ArrayList))
            MyLogger.logArraylist((ArrayList)result);
        else if (result.GetType() == typeof(Hashtable))
            MyLogger.logHashtable((Hashtable)result);
        else
            Debug.Log("result is not a hashtable or arraylist");
    }


    public static void logArraylist(ArrayList result)
    {
        if (!logEnabled)
            return;

        StringBuilder builder = new StringBuilder();

        // we start off with an ArrayList of Hashtables
        foreach (Hashtable item in result)
        {
            MyLogger.addHashtableToString(builder, item);
            builder.Append("\n--------------------\n");
        }

        Debug.Log(builder.ToString());
    }


    public static void logHashtable(Hashtable result)
    {
        if (!logEnabled)
            return;

        StringBuilder builder = new StringBuilder();
        addHashtableToString(builder, result);

        MyLogger.Log(builder.ToString());
    }


    // simple helper to add a hashtable to a StringBuilder to make reading the output easier
    public static void addHashtableToString(StringBuilder builder, Hashtable item)
    {
        if (!logEnabled)
            return;

        foreach (DictionaryEntry entry in item)
        {
            if (entry.Value is Hashtable)
            {
                builder.AppendFormat("{0}: ", entry.Key);
                addHashtableToString(builder, (Hashtable)entry.Value);
            }
            else if (entry.Value is ArrayList)
            {
                builder.AppendFormat("{0}: ", entry.Key);
                addArraylistToString(builder, (ArrayList)entry.Value);
            }
            else
            {
                builder.AppendFormat("{0}: {1}\n", entry.Key, entry.Value);
            }
        }
    }


    public static void addArraylistToString(StringBuilder builder, ArrayList result)
    {
        if (!logEnabled)
            return;

        // we start off with an ArrayList of Hashtables
        foreach (object item in result)
        {
            if (item is Hashtable)
                MyLogger.addHashtableToString(builder, (Hashtable)item);
            else if (item is ArrayList)
                MyLogger.addArraylistToString(builder, (ArrayList)item);
            builder.Append("\n--------------------\n");
        }

        MyLogger.Log(builder.ToString());
    }

    public static void Test(string log)
    {
        Debug.Log(log);
    }

    public static void Test(object log)
    {
        Debug.Log(log.ToString());
    }
}