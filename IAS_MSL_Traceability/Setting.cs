using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IAS_MSL_Traceability
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
	internal sealed class Setting : ApplicationSettingsBase
	{
		private static Setting defaultInstance = (Setting)SettingsBase.Synchronized((SettingsBase)new Setting());

		public static Setting Default
		{
			get
			{
				return Setting.defaultInstance;
			}
		}
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("false")]
		public bool page1Visible
		{
			get
			{
				return (bool)this[nameof(page1Visible)];
			}
			set
			{
				this[nameof(page1Visible)] = (object)value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("false")]
		public bool page2Visible
		{
			get
			{
				return (bool)this[nameof(page2Visible)];
			}
			set
			{
				this[nameof(page2Visible)] = (object)value;
			}
		}
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("false")]
		public bool page3Visible
		{
			get
			{
				return (bool)this[nameof(page3Visible)];
			}
			set
			{
				this[nameof(page3Visible)] = (object)value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string printerName
		{
			get
			{
				return (string)this[nameof(printerName)];
			}
			set
			{
				this[nameof(printerName)] = (object)value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("5,15")]
		public string printerPos
		{
			get
			{
				return (string)this[nameof(printerPos)];
			}
			set
			{
				this[nameof(printerPos)] = (object)value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("admin123**")]
		public string password
		{
			get
			{
				return (string)this[nameof(password)];
			}
			set
			{
				this[nameof(password)] = (object)value;
			}
		}
	}
}
