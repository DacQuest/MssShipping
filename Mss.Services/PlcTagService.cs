using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DacQuest.DFX.Core.Services;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core;
using DacQuest.DFX.Devices.Tags;
using DacQuest.DFX.Devices;
using Mss.Collections;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Core.DataItems.Collections;
using Mss.Common;

namespace Mss.Services
{
//     public class PlcTagService : XDeviceSetOwnerService
//     {
//         //public static readonly int MyCustomCommand1 = XService.FirstCustomCommand;
//         //public static readonly int MyCustomCommand2 = XService.FirstCustomCommand + 1;
// 
//         private PlcTags _plcTags;
// //         private List<XDevice> _devices = new List<XDevice>();
// 
//         protected override bool OnStart()
//         {
//             XSharedCollection.Open(Constant.PlcTagsName, out _plcTags);
// 
//             try
//             {
// //                 XDeviceSetConfigurationItem deviceSetConfigurationItem
// //                     = XConfigurationManager.GetConfigurationItem<XDeviceSetConfigurationItem>(
// //                         XConfigurationManager.strX_TAG_DEVICE_SETS_SECTION,
// //                         "PlcTagServiceDevices");
// //                 foreach (string deviceName in deviceSetConfigurationItem.DeviceNames)
// // //                 foreach (string deviceName in deviceSetConfigurationItem.DeviceNames)
// //                 {
// //                     if (XDevice.CreateInstance(
// //                         deviceName,
// //                         out XDevice device,
// //                         out XDeviceConfigurationItem deviceConfigurationItem))
// //                     {
// //                         _devices.Add(device);
// //                     }
// //                     else
// //                     {
// //                         XSystemEvent.Publish(
// //                             deviceSetConfigurationItem.Name,
// //                             XSystemEventLevel.Error,
// //                             $"Failed to initialize device '{deviceConfigurationItem.Name}'.");
// //                         return false;
// //                     }
// //                 }
// //                 Thread.Sleep(3000);
//                 foreach (XDevice device in DeviceSet.Devices)
//                 {
//                     foreach (XTagConfigurationItem tagConfiguration in device.TagConfigurationItems)
//                     {
//                         PlcTagItem item = new PlcTagItem
//                         {
//                             DeviceName = device.DeviceName,
//                             TagName = tagConfiguration.Name,
//                             RoleName = tagConfiguration.RoleName,
//                             DisplayName = device.ConfigurationItem.FriendlyName,
//                             PlcTagName = tagConfiguration.HardwareTagName,
//                             DataType = tagConfiguration.TagDataType,
//                             AccessMode = tagConfiguration.AccessMode
//                         };
//                         XTagData tagData = device.ReadTag(tagConfiguration.RoleName, true);
//                         if (tagData.DataType == XValueDataType.Boolean)
//                         {
//                             if (!tagData.TryGetBoolean(out bool booleanValue))
//                             {
//                                 item.BooleanValue = false;
//                             }
//                             else
//                             {
//                                 item.BooleanValue = booleanValue;
//                             }
//                         }
//                         else if (tagData.DataType == XValueDataType.Int16)
//                         {
//                             if (!tagData.TryGetInt16(out short int16Value))
//                             {
//                                 item.Int16Value = 0;
//                             }
//                             else
//                             {
//                                 item.Int16Value = int16Value;
//                             }
//                         }
//                         else if (tagData.DataType == XValueDataType.Int32)
//                         {
//                             if (!tagData.TryGetInt32(out int int32Value))
//                             {
//                                 item.Int32Value = 0;
//                             }
//                             else
//                             {
//                                 item.Int32Value = int32Value;
//                             }
//                         }
//                         else if (tagData.DataType == XValueDataType.String)
//                         {
//                             if (!tagData.TryGetString(out string stringValue))
//                             {
//                                 item.StringValue = string.Empty;
//                             }
//                             else
//                             {
//                                 item.StringValue = stringValue;
//                             }
//                         }
//                         item.Quality = tagData.Quality;
//                         item.LastUpdateOn = DateTime.Now;
//                         _plcTags[tagConfiguration.Name] = item;
//                         device.StartTagDataCapture(
//                             tagConfiguration.RoleName,
//                             _PlcMonitor_TagChanged,
//                             XTagDataCaptureUpdateMode.OnChange);
//                     }
//                     Thread.Sleep(100);
//                 }
//                 _plcTags.DataItemChanged += _PlcMonitor_DataItemChanged;
//                 // There is no need to call SetServiceStatus() when overriding
//                 return true;
//             }
//             catch (Exception x)
//             {
//                 x.PublishSystemEvent(Constant.PlcTagsName);
//                 return false;
//             }
//         }
// 
//         private void _PlcMonitor_DataItemChanged(object sender, XDataItemChangedEventArgs e)
//         {
//             PlcTagItem plcMonitorItem = (PlcTagItem)e.DataItem;
//             if (plcMonitorItem.WriteTag)
//             {
//                 XDevice device = DeviceSet.Devices.Single(p => p.DeviceName == plcMonitorItem.DeviceName);
//                 object newValue = null;
//                 switch (plcMonitorItem.DataType)
//                 {
//                     case XValueDataType.Boolean:
//                         newValue = plcMonitorItem.NewBooleanValue;
//                         break;
//                     case XValueDataType.Int16:
//                         newValue = plcMonitorItem.NewInt16Value;
//                         break;
//                     case XValueDataType.Int32:
//                         newValue = plcMonitorItem.NewInt32Value;
//                         break;
//                     case XValueDataType.String:
//                         newValue = plcMonitorItem.NewStringValue;
//                         break;
// 
//                 }
//                 device.WriteTag(plcMonitorItem.RoleName, newValue);
//             }
//         }
// 
//         private void _PlcMonitor_TagChanged(object sender, XTagDataEventArgs e)
//         {
//             XTagData tagData = e.TagData;
//             _plcTags.Lock();
//             try
//             {
//                 PlcTagItem item = _plcTags[tagData.Name];
//                 if (tagData.DataType == XValueDataType.Boolean)
//                 {
//                     if (tagData.TryGetBoolean(out bool booleanValue))
//                     {
//                         item.BooleanValue = booleanValue;
//                         item.NewBooleanValue = false;
//                     }
//                 }
//                 else if (tagData.DataType == XValueDataType.Int16)
//                 {
//                     if (tagData.TryGetInt16(out short int16Value))
//                     {
//                         item.Int16Value = int16Value;
//                         item.NewInt16Value = 0;
//                     }
//                 }
//                 else if (tagData.DataType == XValueDataType.Int32)
//                 {
//                     if (tagData.TryGetInt32(out int int32Value))
//                     {
//                         item.Int32Value = int32Value;
//                         item.NewInt32Value = 0;
//                     }
//                 }
//                 else if (tagData.DataType == XValueDataType.String)
//                 {
//                     if (tagData.TryGetString(out string stringValue))
//                     {
//                         item.StringValue = stringValue;
//                         item.NewStringValue = string.Empty;
//                     }
//                 }
//                 item.Quality = tagData.Quality;
//                 item.WriteTag = false;
//                 item.LastUpdateOn = DateTime.Now;
//                 _plcTags[tagData.Name] = item;
//             }
//             finally
//             {
//                 _plcTags.Unlock();
//             }
// 
//         }
//         protected override int OnStop()
//         {
//             foreach (XDevice device in DeviceSet.Devices)
//             {
//                 device.StopAllTagDataCapture();
// //                 device.Close();
//             }
//             // There is no need to call SetServiceStatus() when overriding
//             return ExitCode;
//         }
// 
//         //         protected override bool ProcessParameters(XConfigurationParameterSet parameters)
//         //         {
//         //             return true;
//         //         }
// 
//         //protected override void AutoSubscribe()
//         //{
//         //    // base class MUST be called if this override is used
//         //    base.AutoSubscribe();
//         //}
// 
//         //public override bool CanStop => true;
// 
//         //public override bool CanShutdown => true;
// 
//         //public override bool CanPauseAndContinue => false;
// 
//         //public override bool CanHandlePowerEvent => false;
// 
//         //public override bool CanHandleCustomCommand => GetCustomCommands().Count > 0;
// 
//         //protected override bool OnPause()
//         //{
//         //    return true;
//         //}
// 
//         //protected override bool OnContinue()
//         //{
//         //    return true;
//         //}
// 
//         //protected override void OnCustomCommand(int customCommand)
//         //{
//         //    // Remember to call SetServiceStatus() as necessary when overriding
//         //}
// 
//         //protected override bool OnPowerEvent(PowerBroadcastStatus powerBroadcastStatus)
//         //{
//         //    // Remember to call SetServiceStatus() as necessary when overriding
//         //    return true;
//         //}
// 
//         // This is the common method that does the actual cleanup.
//         // Finalize(), Dispose(), and Close() call this method.
//         // Because this class isn't sealed, this method is protected & virtual.
//         // If this class were sealed, this method should be private.
//         //protected override void Dispose(bool disposing)
//         //{
//         //    // Synchronize threads calling Dispose/Close simultaneously.
//         //    lock (this)
//         //    {
//         //        if (disposing)
//         //        {
//         //            // The object is being explicitly disposed/closed, not
//         //            // finalized. It is therefore safe for code in this if
//         //            // statement to access fields that reference other
//         //            // objects because the Finalize method of these other objects
//         //            // has not been called yet.
//         //            DoDispose();
//         //        }
// 
//         //        // The object is being disposed/closed or finalized, do the following:
//         //        // If resource was already released, just return
//         //        // Set flag indicating that this resource has been released
//         //        // Call GC.SuppressFinalize(this) to prevent Finalize from being called
//         //        GC.SuppressFinalize(this);
//         //        DoFinalize();
//         //    }
//         //}
// 
//         //protected override void DoDispose()
//         //{
//         //    // The object is being explicitly disposed/closed, not
//         //    // finalized. It is therefore safe for code in this if
//         //    // statement to access fields that reference other
//         //    // objects because the Finalize method of these other objects
//         //    // has not been called yet.
// 
//         //    // The base class must always be called when overriding this method
// 
//         //}
// 
//         //protected override void DoFinalize()
//         //{
//         //    // The object is being disposed/closed or finalized, do the following:
//         //    // If resource was already released, just return
//         //    // Set flag indicating that this resource has been released
//         //    // and is no longer valid (e.g., IsValid==false)
//
//         //    // The base class must always be called when overriding this method
//         //}
//     }
    public class PlcTagService : XService
    {
        //public static readonly int MyCustomCommand1 = XService.FirstCustomCommand;
        //public static readonly int MyCustomCommand2 = XService.FirstCustomCommand + 1;

        private PlcTags _plcTags;
        private List<XDevice> _devices = new List<XDevice>();

        protected override bool OnStart()
        {
            XSharedCollection.Open(Constant.PlcTagsName, out _plcTags);

            try
            {
                XDeviceSetConfigurationItem deviceSetConfigurationItem
                    = XConfigurationManager.GetConfigurationItem<XDeviceSetConfigurationItem>(
                        XConfigurationManager.strX_TAG_DEVICE_SETS_SECTION,
                        "PlcTagServiceDevices");
                foreach (string deviceName in deviceSetConfigurationItem.DeviceNames)
                {
                    if (!XConfigurationManager.TryGetConfigurationItem(
                        XConfigurationManager.strX_TAG_DEVICES_SECTION,
                        deviceName,
                        out XDeviceConfigurationItem deviceConfigurationItem))
                    {
                        XSystemEvent.Publish(
                            ConfigurationItem.Name,
                            XSystemEventLevel.Error,
                            $"Failed to find configuration for Device {deviceName}");
                        return false;
                    }

                    if (XDevice.CreateInstance(
                        deviceConfigurationItem,
                        out XDevice device))
                    {
                        _devices.Add(device);
                    }
                    else
                    {
                        XSystemEvent.Publish(
                            deviceSetConfigurationItem.Name,
                            XSystemEventLevel.Error,
                            $"Failed to initialize device '{deviceConfigurationItem.Name}'.");
                        return false;
                    }
                }
                Thread.Sleep(3000);
                foreach (XDevice device in _devices)
                {
                    foreach (XTagConfigurationItem tagConfiguration in device.TagConfigurationItems)
                    {
                        PlcTagItem item = new PlcTagItem
                        {
                            DeviceName = device.DeviceName,
                            TagName = tagConfiguration.Name,
                            RoleName = tagConfiguration.RoleName,
                            DisplayName = device.ConfigurationItem.FriendlyName,
                            PlcTagName = tagConfiguration.HardwareTagName,
                            DataType = tagConfiguration.TagDataType,
                            AccessMode = tagConfiguration.AccessMode
                        };
                        XTagData tagData = device.ReadTag(tagConfiguration.RoleName, true);
                        if (tagData.DataType == XValueDataType.Boolean)
                        {
                            if (!tagData.TryGetBoolean(out bool booleanValue))
                            {
                                item.BooleanValue = false;
                            }
                            else
                            {
                                item.BooleanValue = booleanValue;
                            }
                        }
                        else if (tagData.DataType == XValueDataType.Int16)
                        {
                            if (!tagData.TryGetInt16(out short int16Value))
                            {
                                item.Int16Value = 0;
                            }
                            else
                            {
                                item.Int16Value = int16Value;
                            }
                        }
                        else if (tagData.DataType == XValueDataType.Int32)
                        {
                            if (!tagData.TryGetInt32(out int int32Value))
                            {
                                item.Int32Value = 0;
                            }
                            else
                            {
                                item.Int32Value = int32Value;
                            }
                        }
                        else if (tagData.DataType == XValueDataType.String)
                        {
                            if (!tagData.TryGetString(out string stringValue))
                            {
                                item.StringValue = string.Empty;
                            }
                            else
                            {
                                item.StringValue = stringValue;
                            }
                        }
                        item.Quality = tagData.Quality;
                        item.LastUpdatedOn = DateTime.Now;
                        _plcTags[tagConfiguration.Name] = item;
                        device.StartTagDataCapture(
                            tagConfiguration.RoleName,
                            _PlcMonitor_TagChanged,
                            XTagDataCaptureUpdateMode.OnChange);
                    }
                    Thread.Sleep(100);
                }
                _plcTags.DataItemChanged += _PlcMonitor_DataItemChanged;
                // There is no need to call SetServiceStatus() when overriding
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(Constant.PlcTagsName);
                return false;
            }
        }

        private void _PlcMonitor_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            PlcTagItem plcMonitorItem = (PlcTagItem)e.DataItem;
            if (plcMonitorItem.WriteTag)
            {
                XDevice device = _devices.Single(p => p.DeviceName == plcMonitorItem.DeviceName);
                object newValue = null;
                switch (plcMonitorItem.DataType)
                {
                    case XValueDataType.Boolean:
                        newValue = plcMonitorItem.NewBooleanValue;
                        break;
                    case XValueDataType.Int16:
                        newValue = plcMonitorItem.NewInt16Value;
                        break;
                    case XValueDataType.Int32:
                        newValue = plcMonitorItem.NewInt32Value;
                        break;
                    case XValueDataType.String:
                        newValue = plcMonitorItem.NewStringValue;
                        break;

                }
                device.WriteTag(plcMonitorItem.RoleName, newValue);
            }
        }

        private void _PlcMonitor_TagChanged(object sender, XTagDataEventArgs e)
        {
            XTagData tagData = e.TagData;
            _plcTags.Lock();
            try
            {
                PlcTagItem item = _plcTags[tagData.Name];
                if (tagData.DataType == XValueDataType.Boolean)
                {
                    if (tagData.TryGetBoolean(out bool booleanValue))
                    {
                        item.BooleanValue = booleanValue;
                        item.NewBooleanValue = false;
                    }
                }
                else if (tagData.DataType == XValueDataType.Int16)
                {
                    if (tagData.TryGetInt16(out short int16Value))
                    {
                        item.Int16Value = int16Value;
                        item.NewInt16Value = 0;
                    }
                }
                else if (tagData.DataType == XValueDataType.Int32)
                {
                    if (tagData.TryGetInt32(out int int32Value))
                    {
                        item.Int32Value = int32Value;
                        item.NewInt32Value = 0;
                    }
                }
                else if (tagData.DataType == XValueDataType.String)
                {
                    if (tagData.TryGetString(out string stringValue))
                    {
                        item.StringValue = stringValue;
                        item.NewStringValue = string.Empty;
                    }
                }
                item.Quality = tagData.Quality;
                item.WriteTag = false;
                item.LastUpdatedOn = DateTime.Now;
                _plcTags[tagData.Name] = item;
            }
            finally
            {
                _plcTags.Unlock();
            }

        }
        protected override int OnStop()
        {
            foreach (XDevice device in _devices)
            {
                device.StopAllTagDataCapture();
                device.Close();
            }
            // There is no need to call SetServiceStatus() when overriding
            return ExitCode;
        }

        //         protected override bool ProcessParameters(XConfigurationParameterSet parameters)
        //         {
        //             return true;
        //         }

        //protected override void AutoSubscribe()
        //{
        //    // base class MUST be called if this override is used
        //    base.AutoSubscribe();
        //}

        //public override bool CanStop => true;

        //public override bool CanShutdown => true;

        //public override bool CanPauseAndContinue => false;

        //public override bool CanHandlePowerEvent => false;

        //public override bool CanHandleCustomCommand => GetCustomCommands().Count > 0;

        //protected override bool OnPause()
        //{
        //    return true;
        //}

        //protected override bool OnContinue()
        //{
        //    return true;
        //}

        //protected override void OnCustomCommand(int customCommand)
        //{
        //    // Remember to call SetServiceStatus() as necessary when overriding
        //}

        //protected override bool OnPowerEvent(PowerBroadcastStatus powerBroadcastStatus)
        //{
        //    // Remember to call SetServiceStatus() as necessary when overriding
        //    return true;
        //}

        // This is the common method that does the actual cleanup.
        // Finalize(), Dispose(), and Close() call this method.
        // Because this class isn't sealed, this method is protected & virtual.
        // If this class were sealed, this method should be private.
        //protected override void Dispose(bool disposing)
        //{
        //    // Synchronize threads calling Dispose/Close simultaneously.
        //    lock (this)
        //    {
        //        if (disposing)
        //        {
        //            // The object is being explicitly disposed/closed, not
        //            // finalized. It is therefore safe for code in this if
        //            // statement to access fields that reference other
        //            // objects because the Finalize method of these other objects
        //            // has not been called yet.
        //            DoDispose();
        //        }

        //        // The object is being disposed/closed or finalized, do the following:
        //        // If resource was already released, just return
        //        // Set flag indicating that this resource has been released
        //        // Call GC.SuppressFinalize(this) to prevent Finalize from being called
        //        GC.SuppressFinalize(this);
        //        DoFinalize();
        //    }
        //}

        //protected override void DoDispose()
        //{
        //    // The object is being explicitly disposed/closed, not
        //    // finalized. It is therefore safe for code in this if
        //    // statement to access fields that reference other
        //    // objects because the Finalize method of these other objects
        //    // has not been called yet.

        //    // The base class must always be called when overriding this method

        //}

        //protected override void DoFinalize()
        //{
        //    // The object is being disposed/closed or finalized, do the following:
        //    // If resource was already released, just return
        //    // Set flag indicating that this resource has been released
        //    // and is no longer valid (e.g., IsValid==false)

        //    // The base class must always be called when overriding this method
        //}
    }
}
