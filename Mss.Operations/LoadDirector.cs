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

        protected bool AutoReleaseNonLoadPallets
            => _parameters.AutoReleaseNonLoadPallets;

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

        protected string IsStackName = "Is Stack";
        protected bool IsStack
        {
            get => GetVariable<bool>(IsStackName);
            set => SetVariable(IsStackName, value);
        }

        protected string RejectedByOperatorName = "Rejected By Operator";
        protected bool RejectedByOperator
        {
            get => GetVariable<bool>(RejectedByOperatorName);
            set => SetVariable(RejectedByOperatorName, value);
        }

        //         public bool AutoMode => SystemSettings.LoadDirectorMode == LoadDirectorMode.Auto;
        //         public bool ManualMode => true;

        //         public bool ManualLabelPrinterAvailable => _parameters.ManualLabelPrinterAvailable;

        //         protected string PrintLeftLabelName = "Print Front Left Label";
        //         protected bool PrintLeftLabel
        //         {
        //             get => GetVariable<bool>(PrintLeftLabelName);
        //             set => SetVariable(PrintLeftLabelName, value);
        //         }

        //         protected string PrintRightLabelName = "Print Front Right Label";
        //         protected bool PrintRightLabel
        //         {
        //             get => GetVariable<bool>(PrintRightLabelName);
        //             set => SetVariable(PrintRightLabelName, value);
        //         }

        //         protected string PrintRearLabelName = "Print Rear Label";
        //         protected bool PrintRearLabel
        //         {
        //             get => GetVariable<bool>(PrintRearLabelName);
        //             set => SetVariable(PrintRearLabelName, value);
        //         }

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
                Constant.LD_LoadItemName,
                CurrentLoadItem);
            messageData.SetMessageValue(
                Constant.LD_PalletItemName,
                CurrentPallet);
            messageData.SetMessageValue(
                Constant.LD_IsStackName,
                IsStack);
            messageData.SetMessageValue(
                Constant.LD_AutoReleaseNonLoadPalletsName,
                AutoReleaseNonLoadPallets);
            messageData.SetMessageValue(
                Constant.LD_IsAwaitingOperatorResponseName,
                CurrentState.Name == AwaitingOperatorResponseState);
            return messageData;
        }

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
            => _parameters = (LoadDirectorParameterSetWrapper)parameters;

        protected override void ResetOperationVariables()
        {
            RejectedByOperator = false;
            IsLoadPallet = false;
            IsStack = false;
            PalletAccepted = false;
            PalletRejected = false;
            CurrentLoadItem = null;

            base.ResetOperationVariables();
        }

        protected override void DoStart()
        {
            base.DoStart();

            RejectedByOperator = false;
            IsLoadPallet = false;
            IsStack = false;
            PalletAccepted = false;
            PalletRejected = false;
            CurrentLoadItem = null;

            SystemSettings.DataItemChanged += _SystemSettings_DataItemChanged;

            if (Level == Levels.Lower)
            {
                Subscribe(
                    PrintLabelMessageData.PrintLoadLabelRequest,
                    _PrintLoadLabelRequest_OnMessage,
                    XMessageScopes.All);
            }

        }

        protected override void AutoSubscribe()
        {
            Subscribe(
                PrintLabelMessageData.ReprintShippingLabelRequest,
                _ReprintShippingLabelRequest_OnMessage,
                XMessageScopes.All);

//             if (Level == Levels.Lower)
//             {
//                 Subscribe(
//                     PrintLabelMessageData.PrintLoadLabelRequest,
//                     _PrintLoadLabelRequest_OnMessage,
//                     XMessageScopes.All);
//             }

        }

        protected override void DoStop()
        {
            if (Level == Levels.Lower)
            {
                Unsubscribe(
                    PrintLabelMessageData.PrintLoadLabelRequest,
                    XMessageScopes.All);
            }

            SystemSettings.DataItemChanged -= _SystemSettings_DataItemChanged;

            base.DoStop();
        }

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingPalletState)
            {
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

            int moveCommand;
            string extendedState;

            if (PalletID.ValidPalletID() && !PalletAccepted && !PalletRejected)
            {
                if (DataLayer.ProcessPalletAtLoadDirector(
                    OperationCode,
                    Level,
                    PalletID,
                    out PalletItem palletItem,
                    out LoadItem loadItem,
                    out string fault))
                {
                    IsLoadPallet = true;
                    CurrentLoadItem = loadItem;
                    CurrentPallet = CurrentLoadItem.Pallet;
                    RejectedByOperator = false;
                    if (!_PrintShippingLabels(CurrentLoadItem, out fault))
                    {
                        SetOperationFaulted(fault);
                        return;
                    }
//                     if (CurrentLoadItem.IsLoadLabelLocation
//                          && !_PrintLoadLabel(
//                              Constant.DefaultTrailerID,
//                              CurrentLoadItem.SlugLetter,
//                              out fault))
//                     {
//                         SetOperationFaulted(fault);
//                         return;
//                     }
                    SetCurrentState(AwaitingOperatorResponseState);
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
                    RejectedByOperator = false;
                    IsStack = CurrentPallet.IsStack;
                    if (AutoReleaseNonLoadPallets || IsStack)
                    {
                        PalletRejected = true;
                        RejectedByOperator = false;
                    }
                    else
                    {
                        SetCurrentState(AwaitingOperatorResponseState);
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
            moveCommand = Constant.NoMoveCommand;
            extendedState = string.Empty;
            if (PalletRejected)
            {
                if (IsLoadPallet)
                {
                    DataLayer.RollBackLoadPick(PalletID, false);
                    DataLayer.SetPitPallet(
                        Level,
                        CurrentPallet,
                        PitCode.Purge);
                    if (RejectedByOperator)
                    {
                        XSystemEvent.Publish(
                            $"{Level.ToText()} Load Director",
                            XSystemEventLevel.Notification,
                            $"Pallet {PalletID} was rejected by the operator on the {Level} Level");
                    }
                }
                moveCommand = Constant.LoadDirectorMoveCommandRelease;
                extendedState = IsStack
                    ? Level == Levels.Upper
                        ? $"({Constant.PalletTypeStack})  Releasing Stack {PalletID} to Pallet Return"
                        : $"({Constant.PalletTypeStack})  Releasing Stack {PalletID} to Purge"
                    : IsLoadPallet
                        ? $"({Constant.PalletTypeRejected})  Releasing Rejected Load Pallet {PalletID}"
                        : $"({Constant.PalletTypePurge})  Releasing Purge Pallet {PalletID}";
            }
            else if (PalletAccepted)
            {
                if (!DataLayer.TrySetPresequenced(PalletID, out string fault))
                {
                    SetOperationFaulted(fault);
                    return false;
                }
                moveCommand = Constant.LoadDirectorMoveCommandRelease;
                extendedState = $"({Constant.PalletTypeLoad})  Releasing Load Pallet {PalletID}";
            }
            return moveCommand != Constant.NoMoveCommand;
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
            if (CurrentState.Name == AwaitingOperatorResponseState
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
                        if (IsLoadPallet)
                        {
                            RejectedByOperator = true;
                        }
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

        #region AwaitingMoveCompletedState

        protected override bool DoMoveCompleted()
        {
//             if (IsLoadPallet)
//             {
//                 if (PalletAccepted)
//                 {
//                     
//                 }
//                 else if (PalletRejected)
//                 {
//                     
//                 }
//                 else
//                 {
//                     return false;
//                 }
//             }
            return true;
        }

        #endregion

        //==================================================================================

        #region Label Printing

        private void _PrintLoadLabelRequest_OnMessage(object sender, XMessageEventArgs e)
        {
            PrintLabelMessageData md = (PrintLabelMessageData)e.MessageData;

            if (!_PrintLoadLabel(
                md.SlugLetter,
                md.TrailerNumber,
                md.SmallestRotation,
                md.LargestRotation,
                md.PalletCount,
                out string fault))
            {
                SetOperationFaulted(fault);
            }
        }
        private void _ReprintShippingLabelRequest_OnMessage(object sender, XMessageEventArgs e)
        {
            PrintLabelMessageData md = (PrintLabelMessageData)e.MessageData;

            if (md.SlugLevel != Level)
            {
                return; // wrong level
            }

            Slug slug;
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
            if (!_PrintShippingLabels(
                slug[md.LoadIndex],
                out string fault))
            {
                SetOperationFaulted(fault);
            }
        }

        private bool _PrintShippingLabels(
            LoadItem loadItem,
            out string fault)
        {

            return loadItem.Pallet.VehicleRow == VehicleRow.Row2
                ? _PrintShippingLabel(loadItem, Constant.RearSeatLabelCode, out fault)
                : _parameters.PrintRightLabelFirst
                    ? (_PrintShippingLabel(loadItem, Constant.RightSeatLabelCode, out fault)
                        && _PrintShippingLabel(loadItem, Constant.LeftSeatLabelCode, out fault))
                    : (_PrintShippingLabel(loadItem, Constant.LeftSeatLabelCode, out fault)
                        && _PrintShippingLabel(loadItem, Constant.RightSeatLabelCode, out fault));

        }

        private bool _PrintShippingLabel(
            LoadItem loadItem,
            string vehicleLocation,
            out string fault)
        {
            try
            {
                WriteTag(
                    Constant.ManualLabelPrinterRoleName,
                    Constant.LabelPrintCommandRoleName,
                    ShippingLabelFormatter.Format(loadItem, vehicleLocation));

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

        private bool _PrintLoadLabel(
            SlugLetter slugLetter,
            string trailerNumber,
            string smallestRotation,
            string largestRotation,
            int palletCount,
            out string fault)
        {
            try
            {
                WriteTag(
                    Constant.ManualLabelPrinterRoleName,
                    Constant.LabelPrintCommandRoleName,
                    LoadLabelFormatter.Format(
                        slugLetter,
                        trailerNumber,
                        smallestRotation,
                        largestRotation,
                        palletCount));

                fault = string.Empty;
                return true;
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(ConfigurationItem.Name);
                fault = "Exception thrown while printing Load Label. See System Events.";
                return false;
            }
        }

        //         private void _PrintShippingLabels(LoadItem loadItem)
        //         {
        //             VehicleRow vehicleRow = loadItem.Pallet.VehicleRow;
        //             if (vehicleRow == VehicleRow.Row1)
        //             {
        //                 if (PrintRightLabel)
        //                 {
        //                     if (!_PrintShippingLabel(
        //                         loadItem,
        //                         out string fault))
        //                     {
        //                         SetOperationFaulted(fault);
        //                         return;
        //                     }
        //                 }
        //                 if (PrintLeftLabel)
        //                 {
        //                     if (!_PrintShippingLabel(
        //                         loadItem,
        //                         out string fault))
        //                     {
        //                         SetOperationFaulted(fault);
        //                         return;
        //                     }
        //                 }
        //             }
        //             else if (vehicleRow == VehicleRow.Row2)
        //             {
        //                 if (PrintRearLabel)
        //                 {
        //                     if (!_PrintShippingLabel(
        //                         loadItem,
        //                         out string fault))
        //                     {
        //                         SetOperationFaulted(fault);
        //                         return;
        //                     }
        //                 }
        //             }
        // //             if (PrintRightLabel)
        // //             {
        // //                 if (!_PrintShippingLabel(
        // //                     loadItem,
        // //                     groupIndex,
        // //                     out string fault))
        // //                 {
        // //                     SetOperationFaulted(fault);
        // //                 }
        // //             }
        // //             if (PrintLeftLabel
        // //                 && loadItem.Broadcasts[groupIndex].VehicleRow == VehicleRow.Row1)
        // //             {
        // //                 if (!_PrintShippingLabel(
        // //                     loadItem,
        // //                     groupIndex,
        // //                     out string fault))
        // //                 {
        // //                     SetOperationFaulted(fault);
        // //                 }
        // //             }
        // 
        //         }

        #endregion Label Printing

        //==================================================================================
    }
}
