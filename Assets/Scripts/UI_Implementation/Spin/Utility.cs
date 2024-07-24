using UnityEngine;
using System.Collections;

using System;
using System.Security.Cryptography;

public class Utility
{
    // time utill
    public static float MilliSecondsToSeconds(float milliSeconds)
    {
        return milliSeconds * 0.001f;
    }

    private static readonly DateTime Jan1St1970 = new System.DateTime (1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);    
    public  static long Millis         { get { return (long)((System.DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }

    private static long timeOffset  = 0;
    private static long occurTime   = 0;
    private static MD5  md5Hash     = MD5.Create();

    public  static int  GetTimeZoneOffset()
    {
        return System.TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now).Hours;
    }

    public static long GetCurrentTime()
    {
        return Millis + timeOffset;
    }
    
    public static long GetCurrentLocalTime()
    {
        return (long)((System.DateTime.Now - Jan1St1970).TotalMilliseconds);
    }

    public static DateTime ParseTimestampToDateTime(long timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dateTime = dateTime.AddMilliseconds(timestamp);
        return dateTime;
    }

    public static DateTime ParseTimestampToLocalDateTime(long timestamp)
    {
        DateTime dateTime = ParseTimestampToDateTime(timestamp);
        
        return dateTime.ToLocalTime();        
    }

    public static DateTime GetCurrentDateTime()
    {
        return ParseTimestampToDateTime(GetCurrentTime());
    }

    public static long GetCurrentTimeSeconds()
    {
        return GetCurrentTime() / 1000;
    }

    public static float GetClampedDeltaTime()
    {
        return Mathf.Min(0.0333f, Time.deltaTime);
    }
}