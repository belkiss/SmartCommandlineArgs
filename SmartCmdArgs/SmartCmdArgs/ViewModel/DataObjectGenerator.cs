using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartCmdArgs.ViewModel
{
    static class DataObjectGenerator
    {
        public static DataObject Generate(IEnumerable<CmdBase> data, bool includeObject)
        {
            var dataObject = new DataObject();

            if (data == null)
                return dataObject;

            var dataList = data.ToList();

            if (includeObject)
                dataObject.SetData(CmdArgsPackage.DataObjectCmdListFormat, dataList);

            dataObject.SetData(CmdArgsPackage.DataObjectCmdJsonFormat, SerializeToJson(dataList));

            return dataObject;
        }

        private static string SerializeToJson(IEnumerable<CmdBase> data)
        {
            var jsonData = DataObjectJsonItem.Convert(data);
            if (jsonData == null)
                return null;

            return JsonConvert.SerializeObject(jsonData);
        }

        public static bool ExtractableDataPresent(IDataObject dataObject)
        {
            return dataObject.GetDataPresent(CmdArgsPackage.DataObjectCmdListFormat)
                   || dataObject.GetDataPresent(CmdArgsPackage.DataObjectCmdJsonFormat)
                   || dataObject.GetDataPresent(DataFormats.Text);
        }

        public static IEnumerable<CmdBase> Extract(IDataObject dataObject, bool includeObject)
        {
            if (dataObject == null)
                return null;

            IEnumerable<CmdBase> result = null;
            if (includeObject && dataObject.GetDataPresent(CmdArgsPackage.DataObjectCmdListFormat))
                result = dataObject.GetData(CmdArgsPackage.DataObjectCmdListFormat) as List<CmdBase>;
            if (result == null && dataObject.GetDataPresent(CmdArgsPackage.DataObjectCmdJsonFormat))
                result = DeserializeFromJson(dataObject.GetData(CmdArgsPackage.DataObjectCmdJsonFormat) as string);
            return result;
        }

        private static IEnumerable<CmdBase> DeserializeFromJson(string jsonSting)
        {
            if (jsonSting == null)
                return null;
            var jsonData = JsonConvert.DeserializeObject<List<DataObjectJsonItem>>(jsonSting);
            if (jsonData == null)
                return null;

            return DataObjectJsonItem.Convert(jsonData);
        }

        private class DataObjectJsonItem
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public bool? Enabled { get; set; } = null;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Value { get; set; } = null;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string WorkingDir { get; set; } = null;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string ProjectConfig { get; set; } = null;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string LaunchProfile { get; set; } = null;

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool ExclusiveMode { get; set; } = false;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public IEnumerable<DataObjectJsonItem> Items { get; set; } = null;


            public static IEnumerable<DataObjectJsonItem> Convert(IEnumerable<CmdBase> data)
            {
                foreach (var cmd in data)
                {
                    if (cmd is CmdArgument arg)
                    {
                        yield return new DataObjectJsonItem {Enabled = arg.IsChecked, Value = arg.Value};
                    }
                    else if (cmd is CmdWorkingDir workingDir)
                    {
                        yield return new DataObjectJsonItem {Enabled = workingDir.IsChecked, WorkingDir = workingDir.Value};
                    }
                    else if (cmd is CmdGroup grp)
                    {
                        yield return new DataObjectJsonItem {Value = grp.Value, ProjectConfig = grp.ProjectConfig, LaunchProfile = grp.LaunchProfile, ExclusiveMode = grp.ExclusiveMode, Items = Convert(grp.Items)};
                    }
                }
            }

            public static IEnumerable<CmdBase> Convert(IEnumerable<DataObjectJsonItem> data)
            {
                foreach (var item in data)
                {
                    if (item.WorkingDir != null)
                    {
                        yield return new CmdWorkingDir(item.WorkingDir, item.Enabled ?? false);
                    }
                    else if (item.Items == null)
                    {
                        yield return new CmdArgument(item.Value, item.Enabled ?? false);
                    }
                    else
                    {
                        yield return new CmdGroup(item.Value, Convert(item.Items), exclusiveMode: item.ExclusiveMode, projConf: item.ProjectConfig, launchProfile: item.LaunchProfile);
                    }
                }
            }
        }
    }
}
