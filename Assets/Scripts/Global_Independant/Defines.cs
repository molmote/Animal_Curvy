using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using System;
using System.Reflection;

public enum LockStatus : int
{
    LOCKED      = -1,
    NEW         = 1,
    UNLOCKED    = 0
}

public enum AchievementName : int
{    
    START_BRAIN_EXERCISE = 0,   // make sure to match with json index
    PIECE_OF_CAKE,
    //...
    MASTER,                     //49
    ACHIEVEMENT_ONLY_SIZE,             
    BRONZE_REWARD_ICON = 50,
    SILVER_REWARD_ICON,
    GOLDEN_REWARD_ICON,
    SIZE,             
    DEFAULT_ICON,             
    ADDITIONAL_SIZE = 60
};

public class POPUP_EVENT
{
    public const string POPUP_CLOSE     = "closed" ;
    public const string POPUP_ACCEPT    = "accept" ;
    public const string POPUP_DECLINE   = "decline" ;

    public const string POPUP_RESTART   = "restart" ;
    public const string POPUP_REVIVE    = "revive" ;
};

public class MyMath
{
    public static Vector2 GetVector(Vector2 source, float dist, float angleInDegree)
    {
        float angle = Mathf.Deg2Rad * angleInDegree;
        float x = source.x + Mathf.Cos(angle) * dist;
        float y = source.y + Mathf.Sin(angle) * dist;
        return new Vector2(x,y);
    }

    public static float Get180Angle(Vector2 toVector2)
    {
        Vector2 fromVector2 = new Vector2(1, 0);
        //Vector2 toVector2 = new Vector2(-1, 0);

        float angle = Vector2.Angle(fromVector2, toVector2);
        MyLogger.Blue("MyMath Get360Angle", "from " + fromVector2 +" to "+toVector2);
        MyLogger.Blue("MyMath Get360Angle", "angle before : "+angle);
        Vector3 cross = Vector3.Cross(fromVector2, toVector2);

        if (cross.z > 0)
        {

        }
        else
        {
            angle = -angle;
        }

        MyLogger.Blue("MyMath Get360Angle", "angle transferred : "+angle);

        return angle;
    }

    public static float Get360AngleCrossed(Vector2 toVector2)
    {
        Vector2 fromVector2 = new Vector2(1, 0);
        //Vector2 toVector2 = new Vector2(-1, 0);

        float angle = Vector2.Angle(fromVector2, toVector2);
        MyLogger.Blue("MyMath Get360Angle", "toVector2 : "+toVector2);
        MyLogger.Blue("MyMath Get360Angle", "angle before : "+angle);
        Vector3 cross = Vector3.Cross(fromVector2, toVector2);

        if (cross.z > 0)
            angle = 360 - angle;

        MyLogger.Blue("MyMath Get360Angle", "angle crossed : "+angle);

        return angle;
    }

    public static float GetRandomAngle( Range2 range, Range2 exception )
    {        
        if ( exception.min < range.min || exception.max > range.max )
        {
            return UnityEngine.Random.Range(range.min, range.max);
        }
        float random1 = UnityEngine.Random.Range(range.min, exception.min);
        float random2 = UnityEngine.Random.Range(exception.max, range.max);

        float span1 = Mathf.Abs(range.min - exception.min);
        float d = span1 + Mathf.Abs(range.max-exception.max);
        float det = UnityEngine.Random.Range(0,d);
        if ( det < span1 )
        {
            MyLogger.Blue("GET_RANDOM_ANGLE", string.Format("small span({0})~({1}) : {2}",range.min, exception.min,random1) );
            return random1;
        }
        MyLogger.Blue("GET_RANDOM_ANGLE", string.Format("bigger span({0})~({1}) : {2}",exception.max, range.max,random2) );
        return random2;
    }

}

[System.Serializable]
public class Range2 
{
    public float min;
    public float max;
    public Range2(float min, float max)
    {
        if ( max > min )
        {
            this.min = min;
            this.max = max;
        }
        else
        {
            this.min = max;
            this.max = min;
        }
    }
    public Range2(Vector2 range)
    {
        if ( range.y > range.x )
        {
            this.min = range.x;
            this.max = range.y;
        }
        else
        {
            this.min = range.y;
            this.max = range.x;
        }
    }

    public  float GetRandom()
    {
        return UnityEngine.Random.Range(this.min, this.max);
    }
}

[System.Serializable]
public class MyTimer
{
    [SerializeField]     float timer;
    [SerializeField]     float timeSpan;
    [SerializeField]     bool  start;

    public MyTimer ( float _timeSpan, bool _start = true )
    {
        this.timer      = 0;
        this.timeSpan   = _timeSpan;
        this.start      = _start;
    }

    public  bool UpdateAndCheckIfEnded()
    {
        this.Update();
        return this.IsEnded();
    }

    public  void Update()
    {
        if ( this.start )
        this.timer += Time.deltaTime;
    }

    public  bool IsEnded()
    {
        if ( this.timer > this.timeSpan )
        {
            this.start = false;
            return true;
        }
        return false;
    }

    public  void StartAgain()
    {
        this.timer = 0;
        this.start = true;
    }
}

public class MyCompare
{
    public static bool IsInRange(float comp1, float comp2, float val)
    {
        float min, max;
        if ( comp1 > comp2 )
        {
            max = comp1;
            min = comp2;
        }
        else
        {
            max = comp2;
            min = comp1;
        }
        if ( val >= min && val <= max )
        {
            return true;
        }
        return false;
        //if ( val > comp1 && val < comp2 )  

    }
}

public class MyRandom<T>
{
    public  static T Range(T [] strList)
    {
        int i = UnityEngine.Random.Range(0,strList.Length);
        return strList[i];
    }

    public  static T Range(List<T> strList)
    {
        int i = UnityEngine.Random.Range(0,strList.Count);
        return strList[i];
    }
}


public class Defines 
{
    public const int    TARGET_FPS                  = 120;

    public const string STATIC_CONTROLLER           = "StaticController";
    public const string LOADING_UI                  = "5_UI/UI Loading";
    public const string GAME_GENERAL_SETTING        = "0_Game/Game Setting";
    public const string LOCALIZATION_MANAGER        = "UI/Etc/Localization Manager";

    public const string PLAYER_PREFAB_PATH          = "1_Character/";
    public const string MONSTER_PREFAB_PATH         = "2_Monster/";
    public const string UI_PREFAB_PATH              = "5_UI/";
    public const string DATABASE_PATH               = "6_DB/";

    public const int    USER_REVIEW_PLAYCOUNT       = 20;
    public const float  USER_REVIEW_WEIGHT          = 100f;
    public const int    USER_REVIEW_REWARD          = 2000;
    public const int    USER_VIDEO_REWARD           = 500;
    public const int    CHARACTER_UNLOCK_COST       = 500;

    //public const int    POPUP_RATE_BEFORE_REVIEW    = 20;
    //public const int    POPUP_RATE_AFTER_REVIEW     = 20;

    public const int    AD_VIDEO_INTERVAL           = 6;
    public const int    AD_INTERSTITIAL_INTERVAL    = 5;

    public const int    USER_POPUP_INTERVAL         = 5;

    public const int    POPUP_UI_FIRST_LAYER        = 10;

    public const int    AD_REQUEST_TRY_COUNT        = 10;
    public const int    AD_REQUEST_TRY_INTERVAL     = 5;

    public const int    SERVER_REQUEST_TRY_COUNT    = 3;
    public const float SERVER_REQUEST_TIME_OUT      = 10.0f; // sec

    public static long  TimeToMilliseconds(double currentTime)
    {
        return (long)(currentTime * 1000.0);
    }

    public static string MonsterLeftFormat(int left)
    {
        return System.String.Format("Monster Left : {0}", left);
    }

    public static string CurrentStatusPerBaseFormat(int current, int basis)
    {
        return System.String.Format("{0}/{1}", current, basis);
    }

    public static string PercentFormat(float per)
    {
        return Mathf.CeilToInt(per).ToString() + "%";
    }

    public static string TimeTextFormat(int minutes, int seconds, int rest)
    {
        return System.String.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, rest);
    }

    public static string TimeTextFormat(float time)
    {
        float tmp       = time;

        int minutes     = Mathf.FloorToInt(tmp / 60.0f);
        tmp             -= minutes * 60.0f;

        int seconds     = Mathf.FloorToInt(tmp);
        tmp             -= seconds;

        int rest        = Mathf.FloorToInt(tmp*100);

        return TimeTextFormat(minutes, seconds, rest);
    }

    public static string TimeTextFormat2(int hours, int minutes, int seconds)
    {
        return System.String.Format("{0:00}h {1:00}m {2:00}s", hours, minutes, seconds);
    }

	private static string platform = "";
	public static string GetPlatform()
    {
        if (platform == "")
        {
            if      (Application.platform == RuntimePlatform.IPhonePlayer)  		platform = "IOS";                
            else if (Application.platform == RuntimePlatform.Android)       		platform = "Android";                
            else if (Application.platform == RuntimePlatform.OSXEditor)     		platform = "OSXWebPlayer";
            else if (Application.platform == RuntimePlatform.OSXPlayer)     		platform = "OSXWebPlayer";    
            //else if (Application.platform == RuntimePlatform.OSXWebPlayer)  		platform = "OSXWebPlayer";
            //else if (Application.platform == RuntimePlatform.WindowsWebPlayer)     	platform = "WindowsWebPlayer";
            else                                                            		platform = "UNKNOWN";            
        }
        
        return platform;
    }

    public static string GetDateToNonURL()
    {
        return string.Format("{0:MM-dd-yyyy}{0:HH_mm_ss.ffffff}", DateTime.Now);
    }

    public static string GetSaveFileFreeURL()
    {
        string name = SystemInfo.deviceModel + GetDateToNonURL();
        #if UNITY_IPHONE
            name = "SystemInfo.deviceModel";
        #endif
        return name.Replace(" ", "");
    }

    // public const string FACEBOOK_HOME_ID                         = "503287153144438";
    // public const string FACEBOOK_HOME_PROFILE       = "fb://profile/503287153144438";
    public const string FACEBOOK_HOME_URL           = "https://www.facebook.com/ketchappgames?fref=ts";

    public const string TWITTER_HOME_PROFILE        = "twitter:///user?screen_name=ketchappgames";
    public const string TWITTER_HOME_URL            = "twitter:///user?screen_name=ketchappgames";

    // public const string REVIEW_ADDRESS_ANDROID      = "https://db.tt/ujMZ4DzQ";
    // public const string REVIEW_ADDRESS_IOS          = "https://db.tt/btUjaGIg";
    // public const string CONTACT_EMAIL_ADDRESS       = "contact@molamola.co.kr";

}
 
namespace UnityEngine {
 
    public static class ExtensionMethods {
 
        public static void Invoke(this MonoBehaviour behaviour, string method, object options, float delay) {
            behaviour.StartCoroutine(_invoke(behaviour, method, delay, options));
        }
        
        private static IEnumerator _invoke(this MonoBehaviour behaviour, string method, float delay, object options) {
            if (delay > 0f) {
                yield return new WaitForSeconds(delay);
            }
 
            Type instance = behaviour.GetType();
            MethodInfo mthd = instance.GetMethod(method);
            mthd.Invoke(behaviour, new object[]{options});
 
            yield return null;
        }
 
    }
    
}