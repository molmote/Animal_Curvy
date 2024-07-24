using UnityEngine;
using System.Collections;

namespace Waggle
{

public class TimeFormatUtility 
{
	public static string TimeFormat(long milliseconds)
	{
		long minutes = milliseconds / 60 % 60; 
		long hour 	 = milliseconds / 60 / 60;
		long seconds = milliseconds;

		if (hour <= 0 && minutes <= 0)
		{
			return string.Format("{0:00}", seconds);
		}
		else
		{
			return string.Format("{0:00}:{1:00}", hour, minutes);	
		}
	}
}

}