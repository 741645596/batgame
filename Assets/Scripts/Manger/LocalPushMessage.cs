using UnityEngine;
using System.Collections;

public class LocalPushMessage : MonoBehaviour {
	
	#if UNITY_EDITOR
	#else 
		#if UNITY_ANDROID
		private AndroidJavaObject m_ANObj = null;
		#endif
		#if UNITY_IPHONE
		#endif
	#endif
	
	#if UNITY_EDITOR
	#else 
		#if UNITY_ANDROID
		private bool InitNotificator()
		{
			if (m_ANObj == null)
			{
				try
				{
				m_ANObj = new AndroidJavaObject("com.nd.unityplugin.NDPushService");
				}
				catch
				{
					//this.Text_Message.text = "Init AndroidNotificator Fail";
					return false;
				}
			}
			
			if (m_ANObj == null)
			{
				//this.Text_Message.text = "AndroidNotificator Not Found.";
				return false;
			}
			
			return true;
		}
		#endif
		#if UNITY_IPHONE
		#endif
	#endif
	
	//本地推送
	public  void NotificationMessage(string strProductName,string  strtitle,string message,int nsecond ,bool isRepeatDay)
	{
		#if UNITY_EDITOR
		#else 
		#if UNITY_ANDROID
		if(InitNotificator())
		{
			m_ANObj.CallStatic(
				"ShowNotification",
				strProductName,
				strtitle,
				message,
				nsecond,
				isRepeatDay);
		}
		#endif
		#if UNITY_IPHONE
		System.DateTime newDate = System.DateTime.Now.AddSeconds (nsecond);
		NotificationMessage(message,newDate,isRepeatDay);
		#endif
		#endif
	}
	#if UNITY_EDITOR
	#else 
		#if UNITY_IPHONE
		//本地推送 你可以传入一个固定的推送时间
		public  void NotificationMessage(string message,System.DateTime newDate,bool isRepeatDay)
		{
			//推送时间需要大于当前时间
			if(newDate > System.DateTime.Now)
			{
				LocalNotification localNotification = new LocalNotification();
				localNotification.fireDate =newDate;	
				localNotification.alertBody = message;
				localNotification.applicationIconBadgeNumber = 1;
				localNotification.hasAction = true;
				if(isRepeatDay)
				{
					//是否每天定期循环
					localNotification.repeatCalendar = CalendarIdentifier.ChineseCalendar;
					localNotification.repeatInterval = CalendarUnit.Day;
				}
				localNotification.soundName = LocalNotification.defaultSoundName;
				NotificationServices.ScheduleLocalNotification(localNotification);
			}
		}
		#endif
	#endif
	void Awake()
	{
		#if UNITY_EDITOR
		#else 
			#if UNITY_ANDROID
			if(InitNotificator())
			{
				m_ANObj.CallStatic("StartPushService");
			}
			#endif
		#endif
		//第一次进入游戏的时候清空，有可能用户自己把游戏冲后台杀死，这里强制清空
		CleanNotification();
	}
	
	void OnApplicationPause(bool paused)
	{
		//程序进入后台时
		if(paused)
		{
			#if UNITY_EDITOR
			#else 
				NotificationMessage("BAT","BATTIP","HELLO CLOCK",10,false);
			#endif

		}
		else
		{
			//程序从后台进入前台时
			CleanNotification();
		}
	}
	
	//清空所有本地消息
	void CleanNotification()
	{
		#if UNITY_EDITOR
		#else 
			#if UNITY_ANDROID
			if(InitNotificator())
			{
				m_ANObj.CallStatic("ClearNotification");
				//this.Text_Message.text = "Notification has been cleaned";
			}
			#endif
			#if UNITY_IPHONE
			LocalNotification l = new LocalNotification (); 
			l.applicationIconBadgeNumber = -1; 
			NotificationServices.PresentLocalNotificationNow (l); 
			NotificationServices.CancelAllLocalNotifications (); 
			NotificationServices.ClearLocalNotifications ();
			#endif
		#endif
	}
}
