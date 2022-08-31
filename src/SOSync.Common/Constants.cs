namespace SOSync.Common;

public class AppConstants
{
    public const string AppName = "SOSync";
    public const string Secret = "5DCBD60C99244E5F85EF263F6243B349";
    public const string Identifier = "24057664-5DA8-41BD-8DCC-98D04AA1A430";
    public const string DroidModuleId = "02982a16-546a-4cfe-8088-c7a20b8fe504";
    public const string WinUIModuleId = "19727e28-af03-417a-8744-f946288ed51b";
    public const string IOSModuleId = "b5f921b3-b64e-467e-932d-4235f1cc3677";
    public const string MacModuleId = "4b6467b9-67aa-4afd-91bd-e84e58959380";
    public const string ServiceId = "74e0678f-6500-49bc-b8f2-d58b302e5174";

    public const string AppInsightsKey = "InstrumentationKey=635cbeac-3974-455c-a644-f9e01df903df;" +
        "IngestionEndpoint=https://brazilsouth-1.in.applicationinsights.azure.com/;" +
        "LiveEndpoint=https://brazilsouth.livediagnostics.monitor.azure.com/";
    public const string AppCenterKey = "ios={28601304-361e-4ebd-8bac-fa5338f8a626};" +
                  "uwp={76519bc5-3cb8-492e-897e-99820cf528df};" +
                  "android={6b88b0e9-97df-4518-8bcc-55644a55f3fc};" +
                  "macos={7740243f-cc48-41c4-86e6-1a03b3953ccc};";

    public class Generic
    {
        public const string Android = "{ \"notification\": { \"SPFidelidade\" : \"PushDemo\", \"body\" : \"$(alertMessage)\"}, \"data\" : { \"action\" : \"$(alertAction)\" } }";
        public const string iOS = "{ \"aps\" : {\"alert\" : \"$(alertMessage)\"}, \"action\" : \"$(alertAction)\" }";
    }

    public class Silent
    {
        public const string Android = "{ \"data\" : {\"message\" : \"$(alertMessage)\", \"action\" : \"$(alertAction)\"} }";
        public const string iOS = "{ \"aps\" : {\"content-available\" : 1, \"apns-priority\": 5, \"sound\" : \"\", \"badge\" : 0}, \"message\" : \"$(alertMessage)\", \"action\" : \"$(alertAction)\" }";
    }
}
