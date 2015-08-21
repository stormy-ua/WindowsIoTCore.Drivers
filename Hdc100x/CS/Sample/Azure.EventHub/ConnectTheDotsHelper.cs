using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Foundation.Metadata;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Web.Http;
using HttpStatusCode = Windows.Web.Http.HttpStatusCode;

namespace Hdc100x.Azure.EventHub
{
    /*
        This set of helper classes for publishing event to EventHub 
        was taken from https://github.com/MSOpenTech/connectthedots/tree/master/Devices/DirectlyConnectedDevices/WindowsIoTCorePi2
    */

    public sealed class AppSettings
    {
        // Our settings
        ApplicationDataContainer localSettings;

        // The key names of our settings
        const string SettingsSetKeyname = "settingsset";
        const string ServicebusNamespaceKeyname = "namespace";
        const string EventHubNameKeyname = "eventhubname";
        const string KeyNameKeyname = "keyname";
        const string KeyKeyname = "key";
        const string DisplayNameKeyname = "displayname";
        const string OrganizationKeyname = "organization";
        const string LocationKeyname = "location";

        // The default value of our settings
        const bool SettingsSetDefault = false;
        const string ServicebusNamespaceDefault = "";
        const string EventHubNameDefault = "";
        const string KeyNameDefault = "";
        const string KeyDefault = "";
        const string DisplayNameDefault = "";
        const string OrganizationDefault = "";
        const string LocationDefault = "";

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            // Get the settings for this application.
            localSettings = ApplicationData.Current.LocalSettings;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object val)
        {
            bool valueChanged = false;

            // If the key exists
            if (localSettings.Values.ContainsKey(Key))
            {
                // If the value has changed
                if (localSettings.Values[Key] != val)
                {
                    // Store the new value
                    localSettings.Values[Key] = val;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                localSettings.Values.Add(Key, val);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetValueOrDefault(string Key, bool defaultValue)
        {
            bool value;

            // If the key exists, retrieve the value.
            if (localSettings.Values.ContainsKey(Key))
            {
                value = (bool)localSettings.Values[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        [DefaultOverload]
        public string GetValueOrDefault(string Key, string defaultValue)
        {
            string value;

            // If the key exists, retrieve the value.
            if (localSettings.Values.ContainsKey(Key))
            {
                value = (string)localSettings.Values[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            // keeping the below in case we want to use this code on a Windows Phone 8 device.
            // With universal Windows Apps, this is no longer necessary as settings are saved automatically
            //            settings.Save();
        }


        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public bool SettingsSet
        {
            get
            {
                return GetValueOrDefault(SettingsSetKeyname, SettingsSetDefault);
            }
            set
            {
                if (AddOrUpdateValue(SettingsSetKeyname, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string ServicebusNamespace
        {
            get
            {
                return GetValueOrDefault(ServicebusNamespaceKeyname, ServicebusNamespaceDefault);
            }
            set
            {
                if (AddOrUpdateValue(ServicebusNamespaceKeyname, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string EventHubName
        {
            get
            {
                return GetValueOrDefault(EventHubNameKeyname, EventHubNameDefault);
            }
            set
            {
                if (AddOrUpdateValue(EventHubNameKeyname, value))
                {
                    Save();
                }
            }
        }
        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string KeyName
        {
            get
            {
                return GetValueOrDefault(KeyNameKeyname, KeyNameDefault);
            }
            set
            {
                if (AddOrUpdateValue(KeyNameKeyname, value))
                {
                    Save();
                }
            }
        }
        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string Key
        {
            get
            {
                return GetValueOrDefault(KeyKeyname, KeyDefault);
            }
            set
            {
                if (AddOrUpdateValue(KeyKeyname, value))
                {
                    Save();
                }
            }
        }
        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return GetValueOrDefault(DisplayNameKeyname, DisplayNameDefault);
            }
            set
            {
                if (AddOrUpdateValue(DisplayNameKeyname, value))
                {
                    Save();
                }
            }
        }
        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string Organization
        {
            get
            {
                return GetValueOrDefault(OrganizationKeyname, OrganizationDefault);
            }
            set
            {
                if (AddOrUpdateValue(OrganizationKeyname, value))
                {
                    Save();
                }
            }
        }
        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string Location
        {
            get
            {
                return GetValueOrDefault(LocationKeyname, LocationDefault);
            }
            set
            {
                if (AddOrUpdateValue(LocationKeyname, value))
                {
                    Save();
                }
            }
        }

    }

    public sealed class ConnectTheDotsSensor
    {
        public string guid { get; set; }
        public string displayname { get; set; }
        public string organization { get; set; }
        public string location { get; set; }
        public string measurename { get; set; }
        public string unitofmeasure { get; set; }
        public string timecreated { get; set; }
        public double value { get; set; }

        /// <summary>
        /// Default parameterless constructor needed for serialization of the objects of this class
        /// </summary>
        public ConnectTheDotsSensor()
        {
        }

        /// <summary>
        /// Construtor taking parameters guid, measurename and unitofmeasure
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="measurename"></param>
        /// <param name="unitofmeasure"></param>
        public ConnectTheDotsSensor(string guid, string measurename, string unitofmeasure)
        {
            this.guid = guid;
            this.measurename = measurename;
            this.unitofmeasure = unitofmeasure;
        }

        /// <summary>
        /// ToJson function is used to convert sensor data into a JSON string to be sent to Azure Event Hub
        /// </summary>
        /// <returns>JSon String containing all info for sensor data</returns>
        public string ToJson()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ConnectTheDotsSensor));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, this);
            string json = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);

            return json;
        }
    }

    public sealed class ConnectTheDotsHelper
    {
        // App Settings variables
        private AppSettings localSettings = new AppSettings();

        private IList<ConnectTheDotsSensor> sensors;

        // Http connection string, SAS tokem and client
        Uri uri;
        private string sas;
        HttpClient httpClient = new HttpClient();
        bool EventHubConnectionInitialized = false;

        public ConnectTheDotsHelper(string serviceBusNamespace,
            string eventHubName,
            string keyName,
            string key,
            string displayName,
            string organization,
            string location,
            IList<ConnectTheDotsSensor> sensorList)
        {
            localSettings.ServicebusNamespace = serviceBusNamespace;
            localSettings.EventHubName = eventHubName;
            localSettings.KeyName = keyName;
            localSettings.Key = key;
            localSettings.DisplayName = displayName;
            localSettings.Organization = organization;
            localSettings.Location = location;

            sensors = sensorList;

            SaveSettings();
        }

        /// <summary>
        /// Validate the settings 
        /// </summary>
        /// <returns></returns>
        bool ValidateSettings()
        {
            if ((localSettings.ServicebusNamespace == "") ||
                (localSettings.EventHubName == "") ||
                (localSettings.KeyName == "") ||
                (localSettings.Key == "") ||
                (localSettings.DisplayName == "") ||
                (localSettings.Organization == "") ||
                (localSettings.Location == ""))
            {
                this.localSettings.SettingsSet = false;
                return false;
            }

            this.localSettings.SettingsSet = true;
            return true;

        }

        /// <summary>
        /// Apply new settings to sensors collection
        /// </summary>
        public bool SaveSettings()
        {
            if (ValidateSettings())
            {
                ApplySettingsToSensors();
                this.InitEventHubConnection();
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        ///  Apply settings to sensors collection
        /// </summary>
        public void ApplySettingsToSensors()
        {
            foreach (ConnectTheDotsSensor sensor in sensors)
            {
                sensor.displayname = this.localSettings.DisplayName;
                sensor.location = this.localSettings.Location;
                sensor.organization = this.localSettings.Organization;
            }
        }

        private void SendAllSensorData()
        {
            foreach (ConnectTheDotsSensor sensor in sensors)
            {
                sensor.timecreated = DateTime.UtcNow.ToString("o");
                sendMessage(sensor.ToJson());
            }
        }

        public void SendSensorData(ConnectTheDotsSensor sensor)
        {
            sensor.timecreated = DateTime.UtcNow.ToString("o");
            sendMessage(sensor.ToJson());
        }

        /// <summary>
        /// Send message to Azure Event Hub using HTTP/REST API
        /// </summary>
        /// <param name="message"></param>
        public async void sendMessage(string message)
        {
            if (this.EventHubConnectionInitialized)
            {
                try
                {
                    HttpStringContent content = new HttpStringContent(message, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                    HttpResponseMessage postResult = await httpClient.PostAsync(uri, content);

                    if (postResult.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Debug.WriteLine("Authorization failure: {0}", postResult.ReasonPhrase);
                        httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("SharedAccessSignature", SASTokenHelper());
                        postResult = await httpClient.PostAsync(uri, content);
                    }

                    if (postResult.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Message Sent: {0}", content);
                    }
                    else
                    {
                        Debug.WriteLine("Failed sending message: {0}", postResult.ReasonPhrase);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when sending message:" + e.Message);
                }
            }
        }

        /// <summary>
        /// Helper function to get SAS token for connecting to Azure Event Hub
        /// </summary>
        /// <returns></returns>
        private string SASTokenHelper()
        {
            int expiry = (int)DateTime.UtcNow.AddMinutes(20).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string stringToSign = WebUtility.UrlEncode(this.uri.ToString()) + "\n" + expiry.ToString();
            string signature = HmacSha256(this.localSettings.Key.ToString(), stringToSign);
            string token = String.Format("sr={0}&sig={1}&se={2}&skn={3}", WebUtility.UrlEncode(this.uri.ToString()), WebUtility.UrlEncode(signature), expiry, this.localSettings.KeyName.ToString());
            return token;
        }

        /// <summary>
        /// Because Windows.Security.Cryptography.Core.MacAlgorithmNames.HmacSha256 doesn't
        /// exist in WP8.1 context we need to do another implementation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string HmacSha256(string key, string val)
        {
            var keyStrm = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var valueStrm = CryptographicBuffer.ConvertStringToBinary(val, BinaryStringEncoding.Utf8);

            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(keyStrm);
            hash.Append(valueStrm);

            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }

        /// <summary>
        /// Initialize Event Hub connection
        /// </summary>
        public bool InitEventHubConnection()
        {
            try
            {
                this.uri = new Uri("https://" + this.localSettings.ServicebusNamespace +
                              ".servicebus.windows.net/" + this.localSettings.EventHubName +
                              "/publishers/" + this.localSettings.DisplayName + "/messages");
                this.sas = SASTokenHelper();

                this.httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("SharedAccessSignature", sas);
                this.EventHubConnectionInitialized = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
