﻿using SOCore;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]


[assembly: ExportFont("typicons.ttf", Alias = "TypIcon")]
[assembly: Guid(AppConstants.ServiceId)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCopyright("Copyright © SOTech 2020-2022")]
[assembly: SOApplication(AppConstants.Identifier,
    AppName = AppConstants.AppName,
    ModuleId = AppConstants.ServiceId,
    LogName = "mobile")]