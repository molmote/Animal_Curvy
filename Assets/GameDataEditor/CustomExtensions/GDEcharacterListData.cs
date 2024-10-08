// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the Game Data Editor.
// 
//      Changes to this file will be lost if the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;

using GameDataEditor;

namespace GameDataEditor
{
    public class GDEcharacterListData : IGDEData
    {
        private static string indexKey = "index";
		private int _index;
        public int index
        {
            get { return _index; }
            set {
                if (_index != value)
                {
                    _index = value;
                    GDEDataManager.SetInt(_key+"_"+indexKey, _index);
                }
            }
        }

        private static string purchaseCostKey = "purchaseCost";
		private int _purchaseCost;
        public int purchaseCost
        {
            get { return _purchaseCost; }
            set {
                if (_purchaseCost != value)
                {
                    _purchaseCost = value;
                    GDEDataManager.SetInt(_key+"_"+purchaseCostKey, _purchaseCost);
                }
            }
        }

        private static string prefabNameKey = "prefabName";
		private string _prefabName;
        public string prefabName
        {
            get { return _prefabName; }
            set {
                if (_prefabName != value)
                {
                    _prefabName = value;
                    GDEDataManager.SetString(_key+"_"+prefabNameKey, _prefabName);
                }
            }
        }

        private static string purchaseTypeKey = "purchaseType";
		private string _purchaseType;
        public string purchaseType
        {
            get { return _purchaseType; }
            set {
                if (_purchaseType != value)
                {
                    _purchaseType = value;
                    GDEDataManager.SetString(_key+"_"+purchaseTypeKey, _purchaseType);
                }
            }
        }

        private static string speedTypeKey = "speedType";
		private string _speedType;
        public string speedType
        {
            get { return _speedType; }
            set {
                if (_speedType != value)
                {
                    _speedType = value;
                    GDEDataManager.SetString(_key+"_"+speedTypeKey, _speedType);
                }
            }
        }

        private static string unlockSoundKey = "unlockSound";
		private string _unlockSound;
        public string unlockSound
        {
            get { return _unlockSound; }
            set {
                if (_unlockSound != value)
                {
                    _unlockSound = value;
                    GDEDataManager.SetString(_key+"_"+unlockSoundKey, _unlockSound);
                }
            }
        }

        private static string dieSoundKey = "dieSound";
		private string _dieSound;
        public string dieSound
        {
            get { return _dieSound; }
            set {
                if (_dieSound != value)
                {
                    _dieSound = value;
                    GDEDataManager.SetString(_key+"_"+dieSoundKey, _dieSound);
                }
            }
        }

        public GDEcharacterListData()
		{
			_key = string.Empty;
		}

		public GDEcharacterListData(string key)
		{
			_key = key;
		}
		
        public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
        {
            _key = dataKey;
			
			if (dict == null)
				LoadFromSavedData(dataKey);
			else
			{
                dict.TryGetInt(indexKey, out _index);
                dict.TryGetInt(purchaseCostKey, out _purchaseCost);
                dict.TryGetString(prefabNameKey, out _prefabName);
                dict.TryGetString(purchaseTypeKey, out _purchaseType);
                dict.TryGetString(speedTypeKey, out _speedType);
                dict.TryGetString(unlockSoundKey, out _unlockSound);
                dict.TryGetString(dieSoundKey, out _dieSound);
                LoadFromSavedData(dataKey);
			}
		}

        public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			
            _index = GDEDataManager.GetInt(_key+"_"+indexKey, _index);
            _purchaseCost = GDEDataManager.GetInt(_key+"_"+purchaseCostKey, _purchaseCost);
            _prefabName = GDEDataManager.GetString(_key+"_"+prefabNameKey, _prefabName);
            _purchaseType = GDEDataManager.GetString(_key+"_"+purchaseTypeKey, _purchaseType);
            _speedType = GDEDataManager.GetString(_key+"_"+speedTypeKey, _speedType);
            _unlockSound = GDEDataManager.GetString(_key+"_"+unlockSoundKey, _unlockSound);
            _dieSound = GDEDataManager.GetString(_key+"_"+dieSoundKey, _dieSound);
         }

        public void Reset_index()
        {
            GDEDataManager.ResetToDefault(_key, indexKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetInt(indexKey, out _index);
        }

        public void Reset_purchaseCost()
        {
            GDEDataManager.ResetToDefault(_key, purchaseCostKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetInt(purchaseCostKey, out _purchaseCost);
        }

        public void Reset_prefabName()
        {
            GDEDataManager.ResetToDefault(_key, prefabNameKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(prefabNameKey, out _prefabName);
        }

        public void Reset_purchaseType()
        {
            GDEDataManager.ResetToDefault(_key, purchaseTypeKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(purchaseTypeKey, out _purchaseType);
        }

        public void Reset_speedType()
        {
            GDEDataManager.ResetToDefault(_key, speedTypeKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(speedTypeKey, out _speedType);
        }

        public void Reset_unlockSound()
        {
            GDEDataManager.ResetToDefault(_key, unlockSoundKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(unlockSoundKey, out _unlockSound);
        }

        public void Reset_dieSound()
        {
            GDEDataManager.ResetToDefault(_key, dieSoundKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(dieSoundKey, out _dieSound);
        }

        public void ResetAll()
        {
            GDEDataManager.ResetToDefault(_key, indexKey);
            GDEDataManager.ResetToDefault(_key, prefabNameKey);
            GDEDataManager.ResetToDefault(_key, purchaseTypeKey);
            GDEDataManager.ResetToDefault(_key, purchaseCostKey);
            GDEDataManager.ResetToDefault(_key, speedTypeKey);
            GDEDataManager.ResetToDefault(_key, unlockSoundKey);
            GDEDataManager.ResetToDefault(_key, dieSoundKey);


            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            LoadFromDict(_key, dict);
        }
    }
}
