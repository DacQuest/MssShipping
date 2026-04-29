using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Devices.Tags;
using DacQuest.DFX.Operations;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Operations
{
    public class LoadDirector : LineOperationBase
    {
        private bool _rejectedByOperator = false;
        private bool _broadcastSkuMismatch = false;

        private LoadDirectorParameterSetWrapper _parameters;

        protected Levels Level => _parameters.Level;

        public override OperationCode OperationCode
        {
            get
            {
                switch (Level)
                {
                    case Levels.Upper:
                        return OperationCode.ULD;
                    case Levels.Lower:
                        return OperationCode.LLD;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        protected bool AutoReleaseNonLoadPalletsInManualMode
            => _parameters.AutoReleaseNonLoadPalletsInManualMode;

        protected string CurrentLoadItemName = "Load Item";
        protected LoadItem CurrentLoadItem { get; set; }


        protected string PalletAcceptedName = "Pallet Accepted";
        protected bool PalletAccepted
        {
            get => GetVariable<bool>(PalletAcceptedName);
            set => SetVariable(PalletAcceptedName, value);
        }

        protected string PalletRejectedName = "Pallet Rejected";
        protected bool PalletRejected
        {
            get => GetVariable<bool>(PalletRejectedName);
            set => SetVariable(PalletRejectedName, value);
        }

        protected string IsLoadPalletName = "Is Load Pallet";
        protected bool IsLoadPallet
        {
            get => GetVariable<bool>(IsLoadPalletName);
            set => SetVariable(IsLoadPalletName, value);
        }

        public bool AutoMode => SystemSettings.LoadDirectorMode == LoadDirectorMode.Auto;

        public bool ManualLabelPrinterAvailable => _parameters.ManualLabelPrinterAvailable;

        protected string PrintLeftLabelName = "Print Front Left Label";
        protected bool PrintLeftLabel
        {
            get => GetVariable<bool>(PrintLeftLabelName);
            set => SetVariable(PrintLeftLabelName, value);
        }

        protected string PrintRightLabelName = "Print Front Right Label";
        protected bool PrintRightLabel
        {
            get => GetVariable<bool>(PrintRightLabelName);
            set => SetVariable(PrintRightLabelName, value);
        }

        protected string PrintRearLabelName = "Print Rear Label";
        protected bool PrintRearLabel
        {
            get => GetVariable<bool>(PrintRearLabelName);
            set => SetVariable(PrintRearLabelName, value);
        }

        //==================================================================================

        #region General Overrides

        protected override void RegisterCustomStates()
        {
            RegisterState(AwaitingOperatorResponseState, "Awaiting Operator Response", AwaitingOperatorResponseStateHandler);
            base.RegisterCustomStates();
        }

        protected override XOperationUIMessageData BuildUIMessage()
        {
            XOperationUIMessageData messageData = new XOperationUIMessageData();
            messageData.SetMessageValue(
                Constant.LD_PalletItemName,
                CurrentPallet);
            messageData.SetMessageValue(
                Constant.LD_LoadItemName,
                CurrentLoadItem);
            messageData.SetMessageValue(
                Constant.LD_IsAutoModeName,
                SystemSettings != null && AutoMode);
            messageData.SetMessageValue(
                Constant.LD_AutoReleaseNonLoadPalletsInManualModeName,
                AutoReleaseNonLoadPalletsInManualMode);
            messageData.SetMessageValue(
                Constant.LD_IsAwaitingOperatorResponseName,
                CurrentState.Name == AwaitingOperatorResponseState);
            messageData.SetMessageValue(
                Constant.LD_BroadcastSkuMismatchName,
                _broadcastSkuMismatch);
            return messageData;
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
            => _parameters = (LoadDirectorParameterSetWrapper)parameters;

        protected override void ResetOperationVariables()
        {
            _broadcastSkuMismatch = false;
            _rejectedByOperator = false;
            IsLoadPallet = false;
            PalletAccepted = false;
            PalletRejected = false;
            CurrentLoadItem = null;

//             PrintLeftLabel = SystemSettings.PrintFrontLeftLabelAt == PrintingOperation.LoadDirector;
//             PrintRightLabel = SystemSettings.PrintFrontRightLabelAt == PrintingOperation.LoadDirector;
//             PrintRearLabel = SystemSettings.PrintRearLabelAt == PrintingOperation.LoadDirector;

            base.ResetOperationVariables();
        }

        protected override void DoStart()
        {
            base.DoStart();

//             PrintLeftLabel = SystemSettings.PrintFrontLeftLabelAt == PrintingOperation.LoadDirector;
//             PrintRightLabel = SystemSettings.PrintFrontRightLabelAt == PrintingOperation.LoadDirector;
//             PrintRearLabel = SystemSettings.PrintRearLabelAt == PrintingOperation.LoadDirector;

            _broadcastSkuMismatch = false;
            _rejectedByOperator = false;
            IsLoadPallet = false;
            PalletAccepted = false;
            PalletRejected = false;
            CurrentLoadItem = null;

            SystemSettings.DataItemChanged += _SystemSettings_DataItemChanged;

        }

        protected override void DoStop()
        {
            SystemSettings.DataItemChanged -= _SystemSettings_DataItemChanged;
            base.DoStop();
        }

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingPalletState)
            {
//                 PrintLeftLabel = SystemSettings.PrintFrontLeftLabelAt == PrintingOperation.LoadDirector;
//                 PrintRightLabel = SystemSettings.PrintFrontRightLabelAt == PrintingOperation.LoadDirector;
//                 PrintRearLabel = SystemSettings.PrintRearLabelAt == PrintingOperation.LoadDirector;

                RunCurrentStateHandler();
            }
        }

        #endregion

        //==================================================================================

        #region ProcessPallet State

        protected override void ProcessPalletStateHandler()
        {
            //             SetExtendedState(
            //                 $"Processing Pallet {PalletID}",

            //                 true);
            int moveCommand = Constant.NoMoveCommand;
            string extendedState = string .Empty;

            if (!PalletAccepted && !PalletRejected && PalletID.ValidFrontPalletID())
            {

                if (DataLayer.ProcessPalletAtLoadDirector(
                    OperationCode,
                    Level,
                    PalletID,
                    out PalletItem palletItem,
                    out LoadItem loadItem,
                    out moveCommand,
                    out extendedState,
                    out string fault))
                {
                    IsLoadPallet = true;
                    CurrentLoadItem = loadItem;
                    CurrentPallet = CurrentLoadItem.Pallet;
                    _rejectedByOperator = false;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(fault))
                    {
                        SetOperationFaulted(fault);
                        return;
                    }
                    IsLoadPallet = false;
                    CurrentLoadItem = null;
                    CurrentPallet = palletItem;
                    _rejectedByOperator = false;

                }

                if (AutoMode)
                {
                    if (IsLoadPallet)
                    {
                        BroadcastItem broadcastItem = CurrentLoadItem.Broadcast;

                        if (broadcastItem.Sku != CurrentPallet.Sku)
                        {
                            XSystemEvent.Publish(
                                "SKU Mismatch",
                                XSystemEventLevel.Notification,
                                $" SKU Mismatch between Broadcast and Load Pallet. Pallet={CurrentPallet.PalletID} SKU={CurrentPallet.Sku}. Broadcast SKU={broadcastItem.Sku} CSN={broadcastItem.Csn}");
                            _broadcastSkuMismatch = true;
                            PalletRejected = true;
                            _rejectedByOperator = false;
                            SendUIMessage();
                        }
                    }
                    else
                    {
                        PalletRejected = true;
                        _rejectedByOperator = false;
                    }
                }
                else
                {
                    if (IsLoadPallet)
                    {
                        BroadcastItem broadcastItem = CurrentLoadItem.Broadcast;

                        if (broadcastItem.Sku != CurrentPallet.Sku)
                        {
                            XSystemEvent.Publish(
                                "SKU Mismatch",
                                XSystemEventLevel.Notification,
                                $" SKU Mismatch between Broadcast and Load Pallet. Pallet={CurrentPallet.PalletID} SKU={CurrentPallet.Sku}. Broadcast SKU={broadcastItem.Sku} CSN={broadcastItem.Csn}");
                            _broadcastSkuMismatch = true;
                            PalletRejected = true;
                            _rejectedByOperator = false;
                            SendUIMessage();
                        }
                    }
                    else
                    {
                        if (AutoReleaseNonLoadPalletsInManualMode)
                        {
                            PalletRejected = true;
                            _rejectedByOperator = false;
                        }
                        else
                        {
                            SetCurrentState(AwaitingOperatorResponseState);
                        }
                    }
                }
                SendUIMessage();
            }
            if (DoProcessPallet(out moveCommand, out extendedState))
            {
                SendMoveCommand(moveCommand);
                SetCurrentState(
                    AwaitingMoveCompletedState,
                    extendedState);
//                 SetCurrentState(
//                     AwaitingMoveCommandConfirmationState,
//                     extendedState);
                }
        }

        protected override bool DoProcessPallet(
            out int moveCommand,
            out string extendedState)
        {
            throw new NotImplementedException("LoadDirector is not yet implemented!");
        }

        #endregion

        //==================================================================================

        #region AwaitingOperatorResponseState

        protected readonly string AwaitingOperatorResponseState = "AwaitingOperatorResponse";

        protected virtual void AwaitingOperatorResponseStateHandler()
        {
            // Nothing to do but wait for operator response
        }

        public override void ProcessUIMessage(XOperationUIMessageData uiMessageData)
        {
            bool requestPalletData =
                uiMessageData.GetMessageValue<bool>(Constant.LD_RequestPalletDataName);
            if (requestPalletData)
            {
                SendUIMessage();
                return;
            }
            string currentStateName = CurrentState.Name;
            if (currentStateName == AwaitingOperatorResponseState
                && uiMessageData.ContainsMessageValue(Constant.LD_OperatorResponseName))
            {
                LoadDirectorOperatorResponse response
                    = uiMessageData.GetMessageValue<LoadDirectorOperatorResponse>(
                        Constant.LD_OperatorResponseName);
                switch (response)
                {
                    case LoadDirectorOperatorResponse.Accept:
                        PalletAccepted = true;
                        break;
                    case LoadDirectorOperatorResponse.Reject:
                        _rejectedByOperator = true;
                        PalletRejected = true;
                        break;
                }
                if (response != LoadDirectorOperatorResponse.None)
                {
                    SetCurrentState(ProcessPalletState);
                }
            }
        }

        #endregion

        //==================================================================================

        #region WaitMoveCompleted State

        protected override bool DoMoveCompleted()
        {
            throw new NotImplementedException("LoadDirector.DoMoveCompleted() is not implemented!");
//             if (CachedMoveCommand == Constant.PurgeMoveCommandToPurgeLane)
//             {
//                 PurgePalletWriter.WritePurgePallet(CurrentPallet);
//                 DataLayer.RemovePitPallet(CurrentPallet.PalletID);
//             }
//             return true;
        }

        #endregion

        //==================================================================================

        #region Label Printing

        private void _ReprintLabelRequest_OnMessage(object sender, XMessageEventArgs e)
        {
            ReprintLabelMessageData md = (ReprintLabelMessageData)e.MessageData;
            Slug slug = null;
            if (md.SlugLetter == SlugLetter.A)
            {
                slug = SlugA;
            }
            else if (md.SlugLetter == SlugLetter.B)
            {
                slug = SlugB;
            }
            else
            {
                XSystemEvent.Publish(
                    ConfigurationItem.Name,
                    XSystemEventLevel.Error,
                    $"Received unknown Slug Letter {md.SlugLetter.ToText()} while attempting to reprint a Shipping Label.");
                return;
            }
            if (!_PrintShippingLabel(
                slug[md.LoadIndex],
                out string fault))
            {
                SetOperationFaulted(fault);
            }
        }

//         private bool _PrintLearLabel(
//             PalletItem palletItem,
//             out string fault)
//         {
//             fault = string.Empty;
// 
//             if (!_parameters.LabelPrintingEnabled)
//             {
//                 return true;
//             }
// 
//             if (!ManualLabelPrinterEnabled)
//             {
//                 XSystemEvent.Publish(
//                     ConfigurationItem.Name,
//                     XSystemEventLevel.Notification,
//                     "Label Printer not connected");
//                 return false;
//             }
// 
//             try
//             {
//                 WriteTag(
//                     Constant.ManualLabelPrinterRoleName,
//                     Constant.LabelPrintCommandRoleName,
//                     ShippingLabelFormatter.FormatLearLabel(palletItem));
// 
//                 fault = string.Empty;
//                 XSystemEvent.Publish(
//                     ConfigurationItem.Name,
//                     XSystemEventLevel.Notification,
//                      $"Printed Lear Label: PalletID={palletItem.PalletIDText} JobID={palletItem.JobIDText} SKU={palletItem.Sku}");
//                 return true;
//             }
//             catch (Exception x)
//             {
//                 x.PublishSystemEvent(ConfigurationItem.Name);
//                 fault = "Exception thrown while printing Lear Pallet Label. See System Events.";
//                 return false;
//             }
//         }

        private bool _PrintShippingLabel(
            LoadItem loadItem,
            out string fault)
        {
            fault = string.Empty;

//             if (!_parameters.LabelPrintingEnabled)
//             {
//                 return true;
//             }

            if (!ManualLabelPrinterAvailable)
            {
                XSystemEvent.Publish(
                    ConfigurationItem.Name,
                    XSystemEventLevel.Notification,
                    "Label Printer not connected");
                return false;
            }

            try
            {
//                 WriteTag(
//                     Constant.ManualLabelPrinterRoleName,
//                     Constant.LabelPrintCommandRoleName,
//                     ShippingLabelFormatter.Format(loadItem, groupIndex));

                fault = string.Empty;
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(ConfigurationItem.Name);
                fault = "Exception thrown while printing Shipping Label. See System Events.";
                return false;
            }
        }

        private void _PrintShippingLabels(LoadItem loadItem)
        {
            VehicleRow vehicleRow = loadItem.Pallet.VehicleRow;
            if (vehicleRow == VehicleRow.Row1)
            {
                if (PrintRightLabel)
                {
                    if (!_PrintShippingLabel(
                        loadItem,
                        out string fault))
                    {
                        SetOperationFaulted(fault);
                        return;
                    }
                }
                if (PrintLeftLabel)
                {
                    if (!_PrintShippingLabel(
                        loadItem,
                        out string fault))
                    {
                        SetOperationFaulted(fault);
                        return;
                    }
                }
            }
            else if (vehicleRow == VehicleRow.Row2)
            {
                if (PrintRearLabel)
                {
                    if (!_PrintShippingLabel(
                        loadItem,
                        out string fault))
                    {
                        SetOperationFaulted(fault);
                        return;
                    }
                }
            }
//             if (PrintRightLabel)
//             {
//                 if (!_PrintShippingLabel(
//                     loadItem,
//                     groupIndex,
//                     out string fault))
//                 {
//                     SetOperationFaulted(fault);
//                 }
//             }
//             if (PrintLeftLabel
//                 && loadItem.Broadcasts[groupIndex].VehicleRow == VehicleRow.Row1)
//             {
//                 if (!_PrintShippingLabel(
//                     loadItem,
//                     groupIndex,
//                     out string fault))
//                 {
//                     SetOperationFaulted(fault);
//                 }
//             }

        }

        #endregion Label Printing

        //==================================================================================
    }
}
