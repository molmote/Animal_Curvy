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
    public class GDEmissionLocalizeData : IGDEData
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

        private static string descriptionEngKey = "descriptionEng";
		private string _descriptionEng;
        public string descriptionEng
        {
            get { return _descriptionEng; }
            set {
                if (_descriptionEng != value)
                {
                    _descriptionEng = value;
                    GDEDataManager.SetString(_key+"_"+descriptionEngKey, _descriptionEng);
                }
            }
        }

        private static string summaryEngKey = "summaryEng";
		private string _summaryEng;
        public string summaryEng
        {
            get { return _summaryEng; }
            set {
                if (_summaryEng != value)
                {
                    _summaryEng = value;
                    GDEDataManager.SetString(_key+"_"+summaryEngKey, _summaryEng);
                }
            }
        }

        private static string descriptionKorKey = "descriptionKor";
		private string _descriptionKor;
        public string descriptionKor
        {
            get { return _descriptionKor; }
            set {
                if (_descriptionKor != value)
                {
                    _descriptionKor = value;
                    GDEDataManager.SetString(_key+"_"+descriptionKorKey, _descriptionKor);
                }
            }
        }

        private static string summaryKorKey = "summaryKor";
		private string _summaryKor;
        public string summaryKor
        {
            get { return _summaryKor; }
            set {
                if (_summaryKor != value)
                {
                    _summaryKor = value;
                    GDEDataManager.SetString(_key+"_"+summaryKorKey, _summaryKor);
                }
            }
        }

        private static string descriptionFreKey = "descriptionFre";
		private string _descriptionFre;
        public string descriptionFre
        {
            get { return _descriptionFre; }
            set {
                if (_descriptionFre != value)
                {
                    _descriptionFre = value;
                    GDEDataManager.SetString(_key+"_"+descriptionFreKey, _descriptionFre);
                }
            }
        }

        private static string summaryFreKey = "summaryFre";
		private string _summaryFre;
        public string summaryFre
        {
            get { return _summaryFre; }
            set {
                if (_summaryFre != value)
                {
                    _summaryFre = value;
                    GDEDataManager.SetString(_key+"_"+summaryFreKey, _summaryFre);
                }
            }
        }

        private static string descriptionPorKey = "descriptionPor";
		private string _descriptionPor;
        public string descriptionPor
        {
            get { return _descriptionPor; }
            set {
                if (_descriptionPor != value)
                {
                    _descriptionPor = value;
                    GDEDataManager.SetString(_key+"_"+descriptionPorKey, _descriptionPor);
                }
            }
        }

        private static string summaryPorKey = "summaryPor";
		private string _summaryPor;
        public string summaryPor
        {
            get { return _summaryPor; }
            set {
                if (_summaryPor != value)
                {
                    _summaryPor = value;
                    GDEDataManager.SetString(_key+"_"+summaryPorKey, _summaryPor);
                }
            }
        }

        private static string descriptionSpaKey = "descriptionSpa";
		private string _descriptionSpa;
        public string descriptionSpa
        {
            get { return _descriptionSpa; }
            set {
                if (_descriptionSpa != value)
                {
                    _descriptionSpa = value;
                    GDEDataManager.SetString(_key+"_"+descriptionSpaKey, _descriptionSpa);
                }
            }
        }

        private static string summarySpaKey = "summarySpa";
		private string _summarySpa;
        public string summarySpa
        {
            get { return _summarySpa; }
            set {
                if (_summarySpa != value)
                {
                    _summarySpa = value;
                    GDEDataManager.SetString(_key+"_"+summarySpaKey, _summarySpa);
                }
            }
        }

        public GDEmissionLocalizeData()
		{
			_key = string.Empty;
		}

		public GDEmissionLocalizeData(string key)
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
                dict.TryGetString(descriptionEngKey, out _descriptionEng);
                dict.TryGetString(summaryEngKey, out _summaryEng);
                dict.TryGetString(descriptionKorKey, out _descriptionKor);
                dict.TryGetString(summaryKorKey, out _summaryKor);
                dict.TryGetString(descriptionFreKey, out _descriptionFre);
                dict.TryGetString(summaryFreKey, out _summaryFre);
                dict.TryGetString(descriptionPorKey, out _descriptionPor);
                dict.TryGetString(summaryPorKey, out _summaryPor);
                dict.TryGetString(descriptionSpaKey, out _descriptionSpa);
                dict.TryGetString(summarySpaKey, out _summarySpa);
                LoadFromSavedData(dataKey);
			}
		}

        public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			
            _index = GDEDataManager.GetInt(_key+"_"+indexKey, _index);
            _descriptionEng = GDEDataManager.GetString(_key+"_"+descriptionEngKey, _descriptionEng);
            _summaryEng = GDEDataManager.GetString(_key+"_"+summaryEngKey, _summaryEng);
            _descriptionKor = GDEDataManager.GetString(_key+"_"+descriptionKorKey, _descriptionKor);
            _summaryKor = GDEDataManager.GetString(_key+"_"+summaryKorKey, _summaryKor);
            _descriptionFre = GDEDataManager.GetString(_key+"_"+descriptionFreKey, _descriptionFre);
            _summaryFre = GDEDataManager.GetString(_key+"_"+summaryFreKey, _summaryFre);
            _descriptionPor = GDEDataManager.GetString(_key+"_"+descriptionPorKey, _descriptionPor);
            _summaryPor = GDEDataManager.GetString(_key+"_"+summaryPorKey, _summaryPor);
            _descriptionSpa = GDEDataManager.GetString(_key+"_"+descriptionSpaKey, _descriptionSpa);
            _summarySpa = GDEDataManager.GetString(_key+"_"+summarySpaKey, _summarySpa);
         }

        public void Reset_index()
        {
            GDEDataManager.ResetToDefault(_key, indexKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetInt(indexKey, out _index);
        }

        public void Reset_descriptionEng()
        {
            GDEDataManager.ResetToDefault(_key, descriptionEngKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(descriptionEngKey, out _descriptionEng);
        }

        public void Reset_summaryEng()
        {
            GDEDataManager.ResetToDefault(_key, summaryEngKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(summaryEngKey, out _summaryEng);
        }

        public void Reset_descriptionKor()
        {
            GDEDataManager.ResetToDefault(_key, descriptionKorKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(descriptionKorKey, out _descriptionKor);
        }

        public void Reset_summaryKor()
        {
            GDEDataManager.ResetToDefault(_key, summaryKorKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(summaryKorKey, out _summaryKor);
        }

        public void Reset_descriptionFre()
        {
            GDEDataManager.ResetToDefault(_key, descriptionFreKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(descriptionFreKey, out _descriptionFre);
        }

        public void Reset_summaryFre()
        {
            GDEDataManager.ResetToDefault(_key, summaryFreKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(summaryFreKey, out _summaryFre);
        }

        public void Reset_descriptionPor()
        {
            GDEDataManager.ResetToDefault(_key, descriptionPorKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(descriptionPorKey, out _descriptionPor);
        }

        public void Reset_summaryPor()
        {
            GDEDataManager.ResetToDefault(_key, summaryPorKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(summaryPorKey, out _summaryPor);
        }

        public void Reset_descriptionSpa()
        {
            GDEDataManager.ResetToDefault(_key, descriptionSpaKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(descriptionSpaKey, out _descriptionSpa);
        }

        public void Reset_summarySpa()
        {
            GDEDataManager.ResetToDefault(_key, summarySpaKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(summarySpaKey, out _summarySpa);
        }

        public void ResetAll()
        {
            GDEDataManager.ResetToDefault(_key, indexKey);
            GDEDataManager.ResetToDefault(_key, descriptionEngKey);
            GDEDataManager.ResetToDefault(_key, summaryEngKey);
            GDEDataManager.ResetToDefault(_key, descriptionKorKey);
            GDEDataManager.ResetToDefault(_key, summaryKorKey);
            GDEDataManager.ResetToDefault(_key, descriptionFreKey);
            GDEDataManager.ResetToDefault(_key, summaryFreKey);
            GDEDataManager.ResetToDefault(_key, descriptionPorKey);
            GDEDataManager.ResetToDefault(_key, summaryPorKey);
            GDEDataManager.ResetToDefault(_key, descriptionSpaKey);
            GDEDataManager.ResetToDefault(_key, summarySpaKey);


            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            LoadFromDict(_key, dict);
        }
    }
}
