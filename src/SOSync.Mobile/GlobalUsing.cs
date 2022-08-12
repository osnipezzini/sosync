global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

global using SOFramework;

global using SOSync.Abstractions.Models;

global using SOSync.Common;

global using SOSync.Mobile.Pages;
global using SOSync.Mobile.ViewModels;

global using System.Collections.ObjectModel;


#if WINDOWS
[assembly: SOCore.SOApplication(AppConstants.Identifier, AppName = AppConstants.AppName,
	ModuleId = AppConstants.WinUIModuleId,
	ModuleName = "WinUI",
	LogName = "SOSync")]
#elif ANDROID
[assembly: SOCore.SOApplication(AppConstants.Identifier, AppName = AppConstants.AppName,
    ModuleId = AppConstants.DroidModuleId,
    ModuleName = "Android",
    LogName = "SOSync")]
#elif IOS
[assembly: SOCore.SOApplication(AppConstants.Identifier, AppName = AppConstants.AppName,
    ModuleId = AppConstants.IOSModuleId,
    ModuleName = "iOS",
    LogName = "SOSync")]
#elif MACCATALYST
[assembly: SOCore.SOApplication(AppConstants.Identifier, AppName = AppConstants.AppName,
    ModuleId = AppConstants.MacModuleId,
    ModuleName = "MacOS",
    LogName = "SOSync")]
#endif