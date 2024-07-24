using UnityEngine;
using UnityEditor;
using System;
using Google.GData.Client;

namespace GameDataEditor.Google
{
	public class GDEOAuth
	{
		const string CLIENT_ID = "835206785031-e728g5seco0r583h6sivu0iota14ars4.apps.googleusercontent.com";
		const string CLIENT_SECRET = "WuxBy5qFjoy6XWVvlFTS4sdD";
		const string SCOPE = "https://www.googleapis.com/auth/drive.readonly";
		const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

		const string ACCESS_TOKEN_KEY = "gde_at";
		const string REFRESH_TOKEN_KEY = "gde_rt";
		const string ACCESS_TOKEN_TIMEOUT_KEY = "gde_t";

		const int ACCESS_TOKEN_TIMEOUT = 3600;

		OAuth2Parameters oauth2Params;

		public string AccessToken
		{
			get { return oauth2Params.AccessToken; }
			private set {}
		}

        public GDEOAuth()
		{
			oauth2Params = new OAuth2Parameters();		

			oauth2Params.ClientId = CLIENT_ID;
			oauth2Params.ClientSecret = CLIENT_SECRET;
			oauth2Params.RedirectUri = REDIRECT_URI;
			oauth2Params.Scope = SCOPE;
		}

        public bool HasAuthenticated()
        {
            return EditorPrefs.HasKey(ACCESS_TOKEN_TIMEOUT_KEY);
        }

		public string GetAuthURL()
		{
			return OAuthUtil.CreateOAuth2AuthorizationUrl(oauth2Params);
		}

		public void SetAccessCode(string code)
		{
			if (oauth2Params != null)
			{
				oauth2Params.AccessCode = code;
				OAuthUtil.GetAccessToken(oauth2Params);
				SaveTokens();
			}
		}

		public void Init()
		{
            if (HasAuthenticated())
            {
    			string accessToken = EditorPrefs.GetString(ACCESS_TOKEN_KEY, string.Empty);
    			string refreshToken = EditorPrefs.GetString(REFRESH_TOKEN_KEY, string.Empty);
    			
    			oauth2Params.AccessToken = accessToken;
    			oauth2Params.RefreshToken = refreshToken;

    			string timeString = EditorPrefs.GetString(ACCESS_TOKEN_TIMEOUT_KEY, string.Empty);
                DateTime lastRefreshed = DateTime.MinValue;

                if (!timeString.Equals(string.Empty))
                    DateTime.Parse(timeString);

    			TimeSpan timeSinceRefresh = DateTime.Now.Subtract(lastRefreshed);

    			if (timeSinceRefresh.TotalSeconds >= ACCESS_TOKEN_TIMEOUT)
    				RefreshAccessToken();
            }
		}

        public static void ClearAuth()
        {
            EditorPrefs.DeleteKey(ACCESS_TOKEN_TIMEOUT_KEY);
            EditorPrefs.DeleteKey(ACCESS_TOKEN_KEY);
            EditorPrefs.DeleteKey(REFRESH_TOKEN_KEY);

            Debug.Log("Google OAuth Tokens Cleared");
        }

		void RefreshAccessToken()
		{
			OAuthUtil.RefreshAccessToken(oauth2Params);
			SaveTokens();
		}

		void SaveTokens()
		{
			EditorPrefs.SetString(ACCESS_TOKEN_TIMEOUT_KEY, DateTime.Now.ToString());
			EditorPrefs.SetString(ACCESS_TOKEN_KEY, oauth2Params.AccessToken);
			EditorPrefs.SetString(REFRESH_TOKEN_KEY, oauth2Params.RefreshToken);
		}
	}
}


