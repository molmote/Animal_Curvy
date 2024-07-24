using UnityEngine;
using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace GameDataEditor
{
    public class ExcelDataHelper
    {
        public readonly string Path;

		public static string IgnoreToken = "GDE_IGNORE";
		public static string FieldNameToken = "GDE_FIELD_NAMES";
		public static string FieldTypeToken = "GDE_FIELD_TYPES";

        private IExcelDataReader _reader;
        public IExcelDataReader ExcelReader
        {
            get
            {
                if (_reader == null)
                    _reader = GetReader();

                return _reader;
            }
            private set 
            {
                _reader = value;
            }
        }

        private DataSet _dataSet;
        public DataSet WorkbookData
        {
            get
            {
                try
                {
                    if (_dataSet == null)
                     _dataSet = ExcelReader.AsDataSet();
                }
                catch(Exception ex)
                {
                    Debug.LogError("Error loading spreadsheet. Only text formatted cells are supported!");
					Debug.LogException(ex);
                }

                return _dataSet;
            }
            private set
            {
                _dataSet = value;
            }
        }
        
        public ExcelDataHelper(string filePath)
        {
            Path = filePath;
        }

        private IExcelDataReader GetReader()
        {
            FileStream stream = File.OpenRead(Path);            
            IExcelDataReader reader = null;
            try
            {
                if(Path.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if(Path.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                return reader;
            }
            catch (Exception)
            {
                if (stream != null)
                    stream.Close();

                throw;
            }
        }

        public List<string> GetSheetNames()
        {
            if (WorkbookData == null)
                return new List<string>();

            IEnumerable<string> sheets = from DataTable sheet in WorkbookData.Tables select sheet.TableName;
            return sheets.ToList();
        }

        public List<DataRow> GetSheetData(string sheetName)
        {
			if (WorkbookData == null)
                return new List<DataRow>();

            var sheetData = WorkbookData.Tables[sheetName];
            IEnumerable<DataRow> sheetRows = from DataRow a in sheetData.Rows select a;
			return sheetRows.ToList();
        }

        public void CloseWorkbook()
        {
            if (_dataSet != null)
                _dataSet.Clear();

            if (_reader != null)
                _reader.Close();

            WorkbookData = null;
            ExcelReader = null;
        }
    }
}
