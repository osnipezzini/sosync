using DeviceId;
using Hardware.Info;
using Microsoft.Deployment.WindowsInstaller;
using Newtonsoft.Json;
using SOCore.Exceptions;
using SOCore.Models;
using SOCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SOSync.CustomAction
{
    public class StatusCode
    {
        public const string OK = "1";
        public const string ERROR = "5";
    }
    public class CustomActions
    {
        const string STATUSCODE = nameof(STATUSCODE);
        const string ERRORMSG = nameof(ERRORMSG);

        [CustomAction]
        public static ActionResult CustomAction1(Session session)
        {
            try
            {
                var licence = new LicenceInstaller();
                licence.Register(session).Wait();
                session[STATUSCODE] = StatusCode.OK;
            }
            catch (Exception ex)
            {
                session[STATUSCODE] = StatusCode.ERROR;
                if (!string.IsNullOrEmpty(ex.InnerException.Message))
                    session[ERRORMSG] = ex.InnerException.Message;
                else
                    session[ERRORMSG] = ex.Message;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult GetRegister(Session session)
        {
            session["SERIAL"] = LicenceInstaller.GetSerial(session["APPID"]);
            return ActionResult.Success;
        }
    }

    public class LicenceInstaller
    {
        const string API_URL = "https://api.sotech.xyz/";
        const string API_URL_BKP = "http://endpoint.sotech.xyz:8005/";
        public LicenceInstaller()
        {
        }

        public async Task RegisterDeviceAsync(string document, string password, string serial, string appId, string appName)
        {
            if (string.IsNullOrEmpty(document))
                throw new ArgumentNullException("Obrigatório informar o documento.");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("Obrigatório informar a senha.");

            //var publicIp = SOHelper.GetPublicIPAddress();
            var registerDevice = new LicenseDeviceRequest
            {
                AppId = appId,
                AppName = appName,
                Serial = serial,
                Document = document,
                MachineName = Environment.MachineName,
                Password = password
            };
            var jsonContent = JsonConvert.SerializeObject(registerDevice);
            var mainServer = string.Empty;
            var statusToTryAnotherServer = new HttpStatusCode[]
            {
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway
            };
            try
            {
                var ip = new Uri(API_URL);
                await new TcpClient().ConnectAsync(ip.Host, ip.Port);
                mainServer = API_URL;
            }
            catch (Exception)
            {
                mainServer = API_URL_BKP;
            }
        doRegister:
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage request = new HttpResponseMessage();
            try
            {
                HttpClient httpClient = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                request = await httpClient.PostAsync($"{mainServer}api/licenses/registerdevice", content);
                var response = await request.Content.ReadAsStringAsync();
                if (request.StatusCode == HttpStatusCode.OK)
                {
                    var pathInstallation = Path.Combine(SOHelper.AppDataFolder, "SOSync/licence.lic");
                    var diretory = Path.GetDirectoryName(pathInstallation);

                    if (!Directory.Exists(diretory))
                        Directory.CreateDirectory(diretory);

                    File.WriteAllText(pathInstallation, response);
                }
                else if (request.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new LicenseNotFoundException(response);
                }

                else if (statusToTryAnotherServer.Contains(request.StatusCode))
                {
                    mainServer = API_URL_BKP;
                    goto doRegister;
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is WebException)
                {
                    mainServer = API_URL_BKP;
                    goto doRegister;
                }
                else if (ex is LicenseNotFoundException lnfe)
                {
                    throw new LicenseNotFoundException(lnfe.Message + "Não foi possivel realizar o licenciamento! Verifique o CPF/CNPJ ou Senha e tente novamente.\n Erro 404");
                }
                else
                    throw new LicenseSaveException("Ocorreu um erro na requisição!", ex);
            }
        }

        public static string GetSerial(string appId = null)
        {
            var hardwareInfo = new HardwareInfo();
            int maxTries = 0;
        refreshHardwareInfo:
            if (!hardwareInfo.DriveList.Any())
                hardwareInfo.RefreshDriveList();
            if (!hardwareInfo.CpuList.Any())
                hardwareInfo.RefreshCPUList(includePercentProcessorTime: false);
            if (!hardwareInfo.MotherboardList.Any())
                hardwareInfo.RefreshMotherboardList();
            if (!hardwareInfo.BiosList.Any())
                hardwareInfo.RefreshBIOSList();
            while (!hardwareInfo.DriveList.Any() && !hardwareInfo.CpuList.Any() && !hardwareInfo.MotherboardList.Any() && maxTries < 20)
            {
                maxTries++;
                Thread.Sleep(250);
                goto refreshHardwareInfo;
            }
            //Combine the IDs and get bytes
            var hwInfo = new List<string>();
            try
            {
                string deviceId = new DeviceIdBuilder()
                            .AddMachineName()
                            .AddOsVersion()
                            .OnWindows(windows => windows
                                .AddMachineGuid())
                            .ToString();
                hwInfo.Add(deviceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (!string.IsNullOrEmpty(appId))
                hwInfo.Add(appId);
            else if (!string.IsNullOrEmpty(SOHelper.AppId))
                hwInfo.Add(SOHelper.AppId);
            try
            {
                hwInfo.AddRange(hardwareInfo.DriveList
                .Where(x => x.Index == 0 || x.Index == 1)
                .Where(x => !string.IsNullOrEmpty(x.SerialNumber))
                .Select(x => x.SerialNumber));

                hwInfo.AddRange(hardwareInfo.DriveList
                    .Where(x => x.Index == 0 || x.Index == 1)
                    .Select(x => x.FirmwareRevision));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }

            if (hardwareInfo.CpuList.Any())
                hwInfo.AddRange(hardwareInfo.CpuList.Select(x => x.ProcessorId));
            if (hardwareInfo.MotherboardList.Any())
                hwInfo.AddRange(hardwareInfo.MotherboardList.Select(x => x.SerialNumber));
            if (hardwareInfo.BiosList.Any())
                hwInfo.AddRange(hardwareInfo.BiosList.Select(x => x.SerialNumber));

            var _id = string.Concat(hwInfo);
            var _byteIds = Encoding.UTF8.GetBytes(_id);

            //Use MD5 to get the fixed length checksum of the ID string
            var _md5 = MD5.Create();
            var _checksum = _md5.ComputeHash(_byteIds);

            //Convert checksum into 4 ulong parts and use BASE36 to encode both
            var _part1Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 0));
            var _part2Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 4));
            var _part3Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 8));
            var _part4Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 12));

            //Concat these 4 part into one string
            var serial = string.Format("{0}-{1}-{2}-{3}", _part1Id, _part2Id, _part3Id, _part4Id);
            return serial;
        }

        public Task Register(Session session)
        {
            try
            {
                return RegisterDeviceAsync(session["CNPJ"], session["PASSWORD"], session["SERIAL"], session["APPID"], session["APP_NAME"]);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
