#if XBOX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using EasyStorage;

using CloudberryKingdom;

namespace EasyStorage
{
	/// <summary>
	/// The languages supported by EasyStorage.
	/// </summary>
	public enum Language
	{
		German,
		Spanish,
		French,
		Italian,
		Japanese,
		English
	}

	/// <summary>
	/// Used to access settings for EasyStorage.
	/// </summary>
	public static class EasyStorageSettings
	{
		/// <summary>
		/// Resets the SaveDevice strings to their default values.
		/// </summary>
		public static void ResetSaveDeviceStrings()
		{
			if (!Localization.IsLoaded()) return;

            SaveDevice.Err_Ok = Localization.WordString(Localization.Words.Err_Ok);
            SaveDevice.Err_YesSelectNewDevice = Localization.WordString(Localization.Words.Err_YesSelectNewDevice);
            SaveDevice.Err_NoContinueWithoutDevice = Localization.WordString(Localization.Words.Err_NoContinueWithoutDevice);
            SaveDevice.Err_ReselectStorageDevice = Localization.WordString(Localization.Words.Err_ReselectStorageDevice);
            SaveDevice.Err_StorageDeviceRequired = Localization.WordString(Localization.Words.Err_StorageDeviceRequired);
            SaveDevice.Err_ForceDisconnectedReselectionMessage = Localization.WordString(Localization.Words.Err_ForceDisconnectedReselectionMessage);
            SaveDevice.Err_PromptForDisconnectedMessage = Localization.WordString(Localization.Words.Err_PromptForDisconnectedMessage);
            SaveDevice.Err_ForceCancelledReselectionMessage = Localization.WordString(Localization.Words.Err_ForceCancelledReselectionMessage);
            SaveDevice.Err_PromptForCancelledMessage = Localization.WordString(Localization.Words.Err_PromptForCancelledMessage);
		}
	}
}
#endif