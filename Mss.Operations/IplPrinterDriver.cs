using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using DacQuest.DFX.Devices;
using DacQuest.DFX.Devices.Drivers;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Core.Strings;

namespace Mss.Operations
{
    public class IplPrinterDriver : XDeviceDriver
    {
        public static readonly string SingleFileExtension = "iplx";
        public static readonly string FormatFileExtension = "iplf";
        public static readonly string DataFileExtension = "ipld";

        private class IplDefinitionInfo
        {
            public string FormatDefinition { get; set; }
            public string DataDefinition { get; set; }
        }

        IplPrinterDriverParameterSetWrapper _parameters;
        Dictionary<string, IplDefinitionInfo> _definitions = new Dictionary<string, IplDefinitionInfo>();

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = parameters as IplPrinterDriverParameterSetWrapper;
        }

        protected override bool InitializeHardware(bool connectionReady)
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                _parameters.LabelDefinitionPath);
            foreach (string labelName in _parameters.LabelDefinitionNames)
            {
                if (_parameters.SingleDefinitionFile)
                {
                    _LoadSingleDefinitionFile(path, labelName);
                }
                else
                {
                    _LoadDefinitionFiles(path, labelName);
                }
            }
            return true;
        }

        public override void WriteHardwareTag(string hardwareTagName, object tagValue)
        {
            XLabelDataPairs dataPairs = tagValue as XLabelDataPairs;
            if (!dataPairs.GetLabelDefinitionName(out string labelDefinitionName))
            {
                XSystemEvent.Publish(
                    OwnerDevice.DeviceName,
                    XSystemEventLevel.Error,
                    "Label Data missing LabelDefinitionName value");
                return;
            }
            IplDefinitionInfo info;
            if (!_definitions.TryGetValue(labelDefinitionName, out info))
            {
                XSystemEvent.Publish(
                    OwnerDevice.DeviceName,
                    XSystemEventLevel.Error,
                    string.Format(
                        "Label Definition not found: {0}",
                        labelDefinitionName));
                return;
            }
            StringBuilder definition = new StringBuilder(info.DataDefinition);
            foreach (KeyValuePair<string, string> pair in dataPairs)
            {
                _ReplacePlaceholder(definition, pair.Key, pair.Value);
            }
            _ConvertToBytesAndWriteToPrinter(definition.ToString());
        }

        private void _LoadSingleDefinitionFile(string path, string labelName)
        {
            string filePath = Path.Combine(path, labelName);
            filePath = Path.ChangeExtension(filePath, SingleFileExtension);
            if (!File.Exists(filePath))
            {
                XSystemEvent.Publish(
                    OwnerDevice.DeviceName,
                    XSystemEventLevel.Error,
                    string.Format(
                        "Label Definition File Not Found: {0}",
                        filePath));
                return;
            }
            if (_LoadDefinitionFile(filePath, out string file))
            {
                IplDefinitionInfo info = new IplDefinitionInfo
                {
                    DataDefinition = file
                };
                _definitions[labelName] = info;
            }
        }

        private void _LoadDefinitionFiles(string path, string labelName)
        {
            string formatFilePath = Path.Combine(path, labelName);
            formatFilePath = Path.ChangeExtension(formatFilePath, FormatFileExtension);
            if (!File.Exists(formatFilePath))
            {
                XSystemEvent.Publish(
                    OwnerDevice.DeviceName,
                    XSystemEventLevel.Error,
                    string.Format(
                        "Label Definition File Not Found: {0}",
                        formatFilePath));
                return;
            }
            string dataFilePath = Path.Combine(path, labelName);
            dataFilePath = Path.ChangeExtension(dataFilePath, DataFileExtension);
            if (!File.Exists(dataFilePath))
            {
                XSystemEvent.Publish(
                    OwnerDevice.DeviceName,
                    XSystemEventLevel.Error,
                    string.Format(
                        "Label Definition File Not Found: {0}",
                        dataFilePath));
                return;
            }
            if (_LoadDefinitionFile(dataFilePath, out string dataFile))
            {
                if (_LoadDefinitionFile(formatFilePath, out string formatFile))
                {
                    IplDefinitionInfo info = new IplDefinitionInfo
                    {
                        FormatDefinition = formatFile,
                        DataDefinition = dataFile
                    };
                    _definitions[labelName] = info;
                    _ConvertToBytesAndWriteToPrinter(formatFile);
                }
            }

        }

        private bool _LoadDefinitionFile(string filePath, out string file)
        {
            file = string.Empty;
            StreamReader reader = new StreamReader(filePath);
            try
            {
                file = reader.ReadToEnd();
            }
            catch
            {
                XSystemEvent.Publish(
                    OwnerDevice.DeviceName,
                    XSystemEventLevel.Error,
                    string.Format(
                        "Failed to read Label Definition File: {0}",
                        filePath));
            }
            finally
            {
                reader.Close();
            }
            return !string.IsNullOrWhiteSpace(file);
        }

        private void _ReplacePlaceholder(
            StringBuilder definition,
            string valueName,
            string value)
        {
            definition.Replace(
                string.Format(
                    "{0}{1}{2}",
                    _parameters.FormatPlaceholderStartDelimiter,
                    valueName,
                    _parameters.FormatPlaceholderEndDelimiter),
                value);
        }

        private void _ConvertToBytesAndWriteToPrinter(string data)
        {
            DeviceConnection.Write(Encoding.ASCII.GetBytes(data));
        }
    }
}
