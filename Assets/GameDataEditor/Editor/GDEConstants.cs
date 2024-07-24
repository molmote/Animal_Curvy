using System;

namespace GameDataEditor
{
    public class GDEConstants {
        #region Header Strings
        public static string GameDataHeader = "Create Game Data";
        public static string DefineDataHeader = "Define Game Data";
        public static string CreateNewItemHeader = "Create a New Item";
        public static string ItemListHeader = "Item List";
        public static string SearchHeader = "Search";
        public static string CreateNewSchemaHeader = "Create a New Schema";
        public static string SchemaListHeader = "Schema List";
        public static string NewFieldHeader = "Add a new field";
        #endregion

        #region Button Strings
        public static string SaveBtn = "Save";
        public static string SaveNeededBtn = "Save Needed";
        public static string LoadBtn = "Load";
        public static string ClearSearchBtn = "Clear Search";
        public static string CreateNewItemBtn = "Create New Item";
        public static string DeleteBtn = "Delete";
        public static string ResizeBtn = "Resize";
        public static string AddFieldBtn = "Add Field";
        public static string AddCustomFieldBtn = "Add Custom Field";
        public static string CreateNewSchemaBtn = "Create New Schema";
        public static string RenameBtn = "Rename";
        public static string CancelBtn = "Cancel";
        public static string DeleteSchemaBtn = "Delete Schema";
        #endregion

        #region Label Strings
        public static string FilterBySchemaLbl = "Show Items Containing Schema:";
        public static string SchemaLbl = "Schema:";
        public static string ItemNameLbl = "Item Name:";
        public static string ExpandAllLbl = "Expand All";
        public static string CollapseAllLbl = "Collapse All";
        public static string ValueLbl = "Value:";
        public static string ValuesLbl = "Values:";
        public static string SizeLbl = "Size:";
        public static string SchemaNameLbl = "Schema Name:";
        public static string DefaultValueLbl = "Default Value:";
        public static string DefaultValuesLbl = "Default Values:";
        public static string DefaultSizeLbl = "Default Size:";
        public static string BasicFieldTypeLbl = "Basic Field Type:";
        public static string CustomFieldTypeLbl = "Custom Field Type:";
        public static string FieldNameLbl = "Field Name:";
        public static string IsListLbl = "Is List:";
		public static string Is2DListLbl = "Is 2D List:";
		public static string EncryptionTitle = "Encryption Complete!";
		public static string EncryptionWarning = "Be sure to move your plain text gde data file OUT of the resources folder!"+Environment.NewLine+Environment.NewLine+
			"Plain Text GDE Data File Path:";
		public static string EncryptionFileLabel = "Encrypted GDE Data File Path:";
		#endregion

        #region Error Strings
        public static string ErrorLbl = "Error!";
        public static string OkLbl = "Ok";
        public static string ErrorCreatingItem = "Error creating item!";
        public static string NoOrInvalidSchema = "No schema or invalid schema selected.";
        public static string SchemaNotFound = "Schema data not found";
        public static string InvalidCustomFieldType = "Invalid custom field type selected.";
        public static string ErrorCreatingField = "Error creating field!";
        public static string ErrorCreatingSchema = "Error creating Schema!";
        public static string SureDeleteSchema = "Are you sure you want to delete this schema?";
        public static string DirectoryNotFound = "Could not find part of the path: {0}";
        #endregion

		#region Window Constants
		public const float MinLabelWidth = 200f;
		public const int Indent = 20;
		public const float LineHeight = 20f;
		public const float TopBuffer = 2f;
		public const float LeftBuffer = 2f;
		public const float RightBuffer = 2f;
		public const float VectorFieldBuffer = 0.75f;
		public const float MinTextAreaWidth = 100f;
		public const float MinTextAreaHeight = LineHeight;
		public const double DoubleClickTime = 0.5;
		public const double AutoSaveTime = 30;
		public const float PreferencesMinWidth = 640f;
		public const float PreferencesMinHeight = 280f;
		#endregion

		#region Preference Keys
		public const string CreateDataColorKey = "gde_createdatacolor";
		public const string DefineDataColorKey = "gde_definedatacolor";
		public const string HighlightColorKey = "gde_highlightcolor";
		public const string AutoSaveKey = "gde_autosave";
		public const string DataFileKey = "gde_datafile";
		#endregion
		
		#region Default Preference Settings
		public const string CreateDataColor = "#013859";
		public const string CreateDataColorPro = "#36ccdb";
		public const string DefineDataColor = "#185e65";
		public const string DefineDataColorPro = "#0488d7";
		public const string HighlightColor = "#f15c25";
		public const bool AutoSaveDefault = false;
		public const string DefaultDataFilePath = "/GameDataEditor/Resources/";
		public const string DataFile = "gde_data.txt";
		#endregion

		#region Link Strings
		public const string RateMeText = "Click To Rate!";
		public const string ForumLinkText = "Suggest Features in the Forum";
		public const string RateMeURL = "http://u3d.as/7YN";
		public const string ForumURL = "http://forum.unity3d.com/threads/game-data-editor-the-visual-data-editor-released.250379/";
		public const string DocURL = "http://gamedataeditor.com/docs/gde-quickstart.html";
		public const string Contact = "mailto:celeste%40stayathomedevs.com?subject=Question%20about%20GDE&cc=steve%40stayathomedevs.com";
		public const string Twitter = "https://twitter.com/celestipoo";
		public const string BorderTexturePath = "Assets/GameDataEditor/Editor/Textures/boarder.png";
		public const string WarningIconTexturePath = "Assets/GameDataEditor/Editor/Textures/warning.png";
		#endregion
		
		#region Import Workbook Keys
		public const string WorkbookFilePathKey = "gde_workbookpath";
		#endregion
    }
}