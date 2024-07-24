using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor.MiniJSON;

namespace GameDataEditor.Google
{
	public class GDEDriveHelper
	{
		const string FILE_QUERY = "https://www.googleapis.com/drive/v2/files?fields=items(exportLinks,title,downloadUrl,mimeType)";
		const string ACCESS_TOKEN = "access_token=";

		GDEOAuth oauth;

		List<string> _spreadSheetNames;
		public string[] SpreadSheetNames
		{
			get 
			{ 
				if (_spreadSheetNames == null)
					_spreadSheetNames = new List<string>(){""};

				return _spreadSheetNames.ToArray(); 
			}
			private set {}
		}

        public string DownloadPath
        {
            get { return Application.dataPath + "/GameDataEditor/Editor/ExcelImport/imported.xlsx";}
            private set {}
        }

		Dictionary<string, string> _spreadSheetLinks;

		public GDEDriveHelper ()
		{
			oauth = new GDEOAuth();
			oauth.Init();

			_spreadSheetLinks = new Dictionary<string, string>();
			_spreadSheetNames = new List<string>();
		}

        public bool HasAuthenticated()
        {
            return oauth.HasAuthenticated();
        }

		public void SetAccessCode(string code)
		{
			oauth.SetAccessCode(code);
		}

		public void RequestAuthFromUser()
		{
			string authURL = oauth.GetAuthURL();
			Application.OpenURL(authURL);
		}

		public void DownloadSpreadSheet(string fileName)
		{
			string fileUrl;
			_spreadSheetLinks.TryGetValue(fileName, out fileUrl);

            DoDownload(fileUrl);
		}

		public void GetSpreadsheetList()
		{
			DoFileListQuery();
		}

		void DoFileListQuery()
		{
			oauth.Init();

			string url = FILE_QUERY + "&" + ACCESS_TOKEN + oauth.AccessToken;

			WWW req = new WWW(url);
            while(!req.isDone);

			if (req.error == null || req.error.Equals(string.Empty))
			{
				Dictionary<string, object> response = Json.Deserialize(req.text) as Dictionary<string, object>;
				List<object> items = response["items"] as List<object>;

                _spreadSheetLinks.Clear();
				_spreadSheetNames.Clear();

				foreach(var item in items)
				{
					Dictionary<string, object> itemData = item as Dictionary<string, object>;

					string mimeType = itemData["mimeType"].ToString();
					string fileName = itemData["title"].ToString();
					string fileUrl = string.Empty;

					if (!mimeType.Contains("spreadsheet"))
						continue;

					if (itemData.ContainsKey("exportLinks"))
					{
						Dictionary<string, object> exportLinks = itemData["exportLinks"] as Dictionary<string, object>;
						foreach(var pair in exportLinks)
						{
							if(pair.Value.ToString().Contains("xlsx"))
								fileUrl = pair.Value.ToString();
						}
					}
					else if(itemData.ContainsKey("downloadUrl"))
						fileUrl = itemData["downloadUrl"].ToString();
					else
					{
						fileUrl = string.Empty;
					}

					try
					{
						if (!_spreadSheetNames.Contains(fileName))
							_spreadSheetNames.Add(fileName);
						_spreadSheetLinks.Add(fileName, fileUrl);
					}
					catch(Exception ex)
					{
						Debug.LogWarning(ex.ToString());
					}
				}
			}
			else
			{
				Debug.Log(req.error);
			}
		}


		void DoDownload(string fileUrl)
		{
			oauth.Init();
			
			fileUrl = fileUrl + "&" + ACCESS_TOKEN + oauth.AccessToken;
			
			WWW req = new WWW(fileUrl);
            while(!req.isDone);

			if (req.error == null || req.error.Equals(string.Empty))
				File.WriteAllBytes(DownloadPath, req.bytes);
			else
				Debug.Log(req.error);
		}
	}
}

