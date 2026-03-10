using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Devices;
using DacQuest.DFX.Devices.Tags;
using DacQuest.DFX.Operations;
using Mss.Collections;
using Mss.Common;
using Mss.Data;

namespace Mss.Operations
{
    public class Crane : XSimpleOperation
    {
        public delegate bool PickFunc(
            out int getCommand,
            out string extendedState,
            out object extra);
        protected class PickFunction
        {
            public PickFunction(CraneFunction craneFunction, PickFunc pickFunc)
            {
                CraneFunction = craneFunction;
                PickFunc = pickFunc;
            }
            public CraneFunction CraneFunction
            {
                get; private set;
            }
            public PickFunc PickFunc
            {
                get; private set;
            }
        }
        protected List<PickFunction> PickFunctionPriority { get; private set; } = new List<PickFunction>();

        private CraneParameterSetWrapper _parameters;
        private bool _startup = true;
        private Levels _previousStoreLevel = Levels.None;
        private Levels _purgeLevel = Levels.None;
        private Levels _stackPickLevel = Levels.None;

        public OperationCode OperationCode
        {
            get
            {
                switch (_parameters.CraneNumber)
                {
                    case CraneNumber.Crane1:
                        return OperationCode.CR1;
                    case CraneNumber.Crane2:
                        return OperationCode.CR2;
                    case CraneNumber.Crane3:
                        return OperationCode.CR3;
                    case CraneNumber.Crane4:
                        return OperationCode.CR4;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

        public CraneNumber CraneNumber => _parameters.CraneNumber;

        protected string CraneModeName = "Crane Mode";
        protected CraneMode CraneMode
        {
            get => GetVariable<CraneMode>(CraneModeName);
            set => SetVariable(CraneModeName, value);
        }

        protected string PalletIDName = "Pallet ID";
        protected string PalletID
        {
            get => GetVariable<string>(PalletIDName);
            set => SetVariable(PalletIDName, value);
        }

        protected string PalletOnCraneName = "Pallet On Crane";
        protected bool PalletOnCrane
        {
            get => GetVariable<bool>(PalletOnCraneName);
            set => SetVariable(PalletOnCraneName, value);
        }

        protected string PalletAtLowerInboundName = "Pallet At Lower Inbound";
        protected bool PalletAtLowerInbound
        {
            get => GetVariable<bool>(PalletAtLowerInboundName);
            set => SetVariable(PalletAtLowerInboundName, value);
        }

        protected string PalletAtUpperInboundName = "Pallet At Upper Inbound";
        protected bool PalletAtUpperInbound
        {
            get => GetVariable<bool>(PalletAtUpperInboundName);
            set => SetVariable(PalletAtUpperInboundName, value);
        }

        protected string LowerOutboundClearName = "Lower Outbound Clear";
        protected bool LowerOutboundClear
        {
            get => GetVariable<bool>(LowerOutboundClearName);
            set => SetVariable(LowerOutboundClearName, value);
        }

        protected string UpperOutboundClearName = "Upper Outbound Clear";
        protected bool UpperOutboundClear
        {
            get => GetVariable<bool>(UpperOutboundClearName);
            set => SetVariable(UpperOutboundClearName, value);
        }

        protected string CurrentCraneFunctionName = "Current Crane Function";
        protected CraneFunction CurrentCraneFunction
        {
            get => GetVariable<CraneFunction>(CurrentCraneFunctionName);
            set => SetVariable(CurrentCraneFunctionName, value);
        }

        protected string LastCraneFunctionName = "Last Crane Function";
        protected CraneFunction LastCraneFunction
        {
            get => GetVariable<CraneFunction>(LastCraneFunctionName);
            set => SetVariable(LastCraneFunctionName, value);
        }

        protected readonly string GetCommandName = "Get Command";
        protected int GetCommand
        {
            get => GetVariable<int>(GetCommandName);
            set => SetVariable(GetCommandName, value);
        }

        protected readonly string PutCommandName = "Put Command";
        protected int PutCommand
        {
            get => GetVariable<int>(PutCommandName);
            set => SetVariable(PutCommandName, value);
        }

        protected readonly string CraneCommandName = "Current Crane Command";
        protected int CraneCommand
        {
            get => GetVariable<int>(CraneCommandName);
            set => SetVariable(CraneCommandName, value);
        }

        protected readonly string SemiAutoGetLocationName = "Semi Auto Get Location";
        protected int SemiAutoGetLocation
        {
            get => GetVariable<int>(SemiAutoGetLocationName);
            set => SetVariable(SemiAutoGetLocationName, value);
        }

        protected readonly string SemiAutoPutLocationName = "Semi Auto Put Location";
        protected int SemiAutoPutLocation
        {
            get => GetVariable<int>(SemiAutoPutLocationName);
            set => SetVariable(SemiAutoPutLocationName, value);
        }

        protected string CurrentPalletName = "Current Pallet";
        protected PalletItem CurrentPallet { get; set; }

        protected string CurrentLoadItemName = "Current Load Item";
        protected LoadItem CurrentLoadItem { get; set; }

        private Storage _storage;
        private LowerPit _lowerPit;
        private UpperPit _upperPit;
        private SystemSettings _systemSettings;
        private Broadcast _broadcast;
        private LoadA _loadA;
        private LoadB _loadB;

        protected DataLayer DataLayer
        {
            get; private set;
        }

        protected bool AutoMode => CraneMode == CraneMode.Auto;
        protected bool SemiAutoMode => CraneMode == CraneMode.SemiAuto;
        protected bool ManualMode => CraneMode == CraneMode.Manual;

        public int InboundLocation(Levels level)
        {
            return level == Levels.Lower
                ? LowerInboundLocation
                : UpperInboundLocation;
        }

        public int OutboundLocation(Levels level)
        {
            return level == Levels.Lower
                ? LowerOutboundLocation
                : UpperOutboundLocation;
        }

        public int LowerInboundLocation
        {
            get
            {
                switch (CraneNumber)
                {
                    case CraneNumber.Crane1:
                        return Constant.Crane1LowerInboundLocation;
                    case CraneNumber.Crane2:
                        return Constant.Crane2LowerInboundLocation;
                    case CraneNumber.Crane3:
                        return Constant.Crane3LowerInboundLocation;
                    case CraneNumber.Crane4:
                        return Constant.Crane4LowerInboundLocation;
                }
                return 0;
            }
        }

        public int UpperInboundLocation
        {
            get
            {
                switch (CraneNumber)
                {
                    case CraneNumber.Crane1:
                        return Constant.Crane1UpperInboundLocation;
                    case CraneNumber.Crane2:
                        return Constant.Crane2UpperInboundLocation;
                    case CraneNumber.Crane3:
                        return Constant.Crane3UpperInboundLocation;
                    case CraneNumber.Crane4:
                        return Constant.Crane4UpperInboundLocation;
                }
                return 0;
            }
        }

        public int LowerOutboundLocation
        {
            get
            {
                switch (CraneNumber)
                {
                    case CraneNumber.Crane1:
                        return Constant.Crane1LowerOutboundLocation;
                    case CraneNumber.Crane2:
                        return Constant.Crane2LowerOutboundLocation;
                    case CraneNumber.Crane3:
                        return Constant.Crane3LowerOutboundLocation;
                    case CraneNumber.Crane4:
                        return Constant.Crane4LowerOutboundLocation;
                }
                return 0;
            }
        }

        public int UpperOutboundLocation
        {
            get
            {
                switch (CraneNumber)
                {
                    case CraneNumber.Crane1:
                        return Constant.Crane1UpperOutboundLocation;
                    case CraneNumber.Crane2:
                        return Constant.Crane2UpperOutboundLocation;
                    case CraneNumber.Crane3:
                        return Constant.Crane3UpperOutboundLocation;
                    case CraneNumber.Crane4:
                        return Constant.Crane4UpperOutboundLocation;
                }
                return 0;
            }
        }

        public override void FaultForUnresponsiveDependencyService(string unresponsiveDependencyServiceName)
            => TelemetrySetCraneFaulted($"Dependency {unresponsiveDependencyServiceName} not responsive.");

//         protected override void FaultOnSharedCollectionOpenFailure(string sharedCollectionName)
//             => TelemetrySetCraneFaulted($"Failed to open {sharedCollectionName}.");

        protected string TelemetryEnabledName = "Telemetry Enabled";
        protected bool TelemetryEnabled
        {
            get => GetVariable<bool>(TelemetryEnabledName);
            set => SetVariable(TelemetryEnabledName, value);
        }

        protected void TelemetrySetCurrentState(string newState)
        {
            PublishCraneStateTelemetry(newState);
            SetCurrentState(newState);
        }

        protected void TelemetrySetCurrentState(string newState, int delayMilliseconds)
        {
            PublishCraneStateTelemetry(newState);
            SetCurrentState(newState, delayMilliseconds);
        }

        protected void TelemetrySetCurrentState(string newState, string extendedState)
        {
            PublishCraneStateTelemetry(newState);
            SetCurrentState(newState, extendedState);
        }

        protected void TelemetrySetCurrentState(
            string newState,
            string extendedState,
            int delayMilliseconds)
        {
            PublishCraneStateTelemetry(newState);
            SetCurrentState(newState, extendedState, delayMilliseconds);
        }

        protected void PublishCraneStateTelemetry(string newState)
        {
            if (TelemetryEnabled)
            {
                XSystemEvent.Publish(
                    ConfigurationItem.Name,
                    XSystemEventLevel.Telemetry,
                    $"*** Crane State: {newState}");
            }
        }

        protected void PublishTelemetryOut(string name, object newValue)
        {
            if (TelemetryEnabled)
            {
                XSystemEvent.Publish(
                    ConfigurationItem.Name,
                    XSystemEventLevel.Telemetry,
                    $"==> {name}: {newValue}");
            }
        }

        protected void PublishTelemetryIn(string name, object newValue)
        {
            if (TelemetryEnabled)
            {
                XSystemEvent.Publish(
                    ConfigurationItem.Name,
                    XSystemEventLevel.Telemetry,
                    $"<== {name}: {newValue}");
            }
        }

        protected void WritePlc(string tagRoleName, object value)
            => WriteTag(Constant.PlcRoleName, tagRoleName, value);

        protected XTagData ReadPlc(string tagRoleName)
            => ReadTag(Constant.PlcRoleName, tagRoleName, true);

        protected void StartPlcTagCapture(
            string tagRoleName,
            XTagDataEventHandler handler,
            XTagDataCaptureUpdateMode updateMode)
                => StartTagDataCapture(Constant.PlcRoleName, tagRoleName, handler, updateMode);

        protected void RegisterPickMethods(CraneFunction[] pickFunctionPriority)
        {
            foreach (CraneFunction craneFunction in pickFunctionPriority)
            {
                switch (craneFunction)
                {
                    case CraneFunction.Audit:
                        PickFunctionPriority.Add(new PickFunction(craneFunction, TryAuditPick));
                        break;
                    case CraneFunction.LoadPick:
                        PickFunctionPriority.Add(new PickFunction(craneFunction, TryLoadPick));
                        break;
                    case CraneFunction.PurgePick:
                        PickFunctionPriority.Add(new PickFunction(craneFunction, TryPurgePick));
                        break;
                    case CraneFunction.StackPick:
                            PickFunctionPriority.Add(new PickFunction(craneFunction, TryStackPick));
                        break;
                    default:
                        break;
                }
            }
        }

        //==================================================================================

        #region General Overrides

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = (CraneParameterSetWrapper)parameters;

            RegisterPickMethods(_parameters.PickFunctionPriority);
        }

        protected override void RegisterCustomStates()
        {
            base.RegisterCustomStates();
            RegisterState(MonitoringSemiAutoState, "Monitoring Semi-Auto", MonitoringSemiAutoStateHandler);
            RegisterState(AwaitingCraneAutoState, "Awaiting Crane Auto", AwaitingCraneAutoStateHandler, true);
            RegisterState(RecoveringOnboardPalletState, "Recovering On-board Pallet", RecoveringOnboardPalletStateHandler);
            RegisterState(AwaitingGetTaskState, "Awaiting Get Task", AwaitingGetTaskStateHandler, true);
            RegisterState(AwaitingGetCompletedState, "Awaiting Get Completed", AwaitingGetCompletedStateHandler);
            RegisterState(AwaitingPalletReadyOnStartupState, "Awaiting Pallet Ready on Crane", AwaitingPalletReadyOnStartupStateHandler);
            RegisterState(AssigningPutTaskState, "Assigning Put Task", AssigningPutTaskStateHandler);
            RegisterState(AwaitingPutCompletedState, "Awaiting Put Completed", AwaitingPutCompletedStateHandler);
        }

        protected override void ResetOperationVariables()
        {
            // Always call this base class when overriding
            base.ResetOperationVariables();

            _purgeLevel = Levels.None;
            _stackPickLevel = Levels.None;

            GetCommand = Constant.NoCraneCommand;
            PutCommand = Constant.NoCraneCommand;
            CurrentPallet = null;
            CurrentLoadItem = null;

            // These are not normal Operation Variables
            // but this is a good place to reset them

            if (CurrentCraneFunction != CraneFunction.None)
            {
                LastCraneFunction = CurrentCraneFunction;
            }
            CurrentCraneFunction = CraneFunction.None;
            WaitingStateName = string.Empty;
        }

        protected override void DoStart()
        {
            _startup = true;
            _purgeLevel = Levels.None;
            _stackPickLevel = Levels.None;

            CraneMode = CraneMode.Manual;
            PalletID = Constant.NoPalletID;
            PalletOnCrane = false;
            PalletAtLowerInbound = false;
            PalletAtUpperInbound = false;
            LowerOutboundClear = false;
            UpperOutboundClear = false;
            CurrentCraneFunction = CraneFunction.None;
            LastCraneFunction = CraneFunction.None;
            GetCommand = Constant.NoCraneCommand;
            PutCommand = Constant.NoCraneCommand;
            CraneCommand = Constant.NoCraneCommand;

            CurrentPallet = null;
            CurrentLoadItem = null;
            WaitingStateName = string.Empty;
            SemiAutoGetLocation = Constant.NoSemiAutoLocation;
            SemiAutoPutLocation = Constant.NoSemiAutoLocation;

            DataLayer = DataLayer.Factory(
                out _storage,
                out _lowerPit,
                out _upperPit,
                out _systemSettings,
                out _broadcast,
                out _loadA,
                out _loadB);

            _systemSettings.SetCraneMode(CraneNumber, CraneMode);

            TelemetryEnabled = _systemSettings.CanGenerateCraneTelemetry(CraneNumber);

            ClearCraneFaultInPlc();

            XTagData tagData = ReadPlc(Constant.PalletIDRoleName);
            if (tagData.TryGetTagValue(out string palletID)
                && palletID.ValidPalletID())
            {
                PalletID = palletID;
            }
            StartPlcTagCapture(
                Constant.PalletIDRoleName,
                _PalletID_TagValueChanged,
                XTagDataCaptureUpdateMode.OnChange);

            tagData = ReadPlc(Constant.PalletOnCraneRoleName);
            if (tagData.TryGetTagValue(out bool palletOnCrane))
            {
                PalletOnCrane = palletOnCrane;
            }
            StartPlcTagCapture(
               Constant.PalletOnCraneRoleName,
               _PalletOnCrane_TagValueChanged,
               XTagDataCaptureUpdateMode.OnChange);

            tagData = ReadPlc(Constant.PalletAtLowerInboundRoleName);
            if (tagData.TryGetTagValue(out bool palletAtInbound))
            {
                PalletAtLowerInbound = palletAtInbound;
            }
            StartPlcTagCapture(
               Constant.PalletAtLowerInboundRoleName,
               _PalletAtLowerInbound_TagValueChanged,
               XTagDataCaptureUpdateMode.OnRefresh);

            tagData = ReadPlc(Constant.PalletAtUpperInboundRoleName);
            if (tagData.TryGetTagValue(out palletAtInbound))
            {
                PalletAtUpperInbound = palletAtInbound;
            }
            StartPlcTagCapture(
               Constant.PalletAtUpperInboundRoleName,
               _PalletAtUpperInbound_TagValueChanged,
               XTagDataCaptureUpdateMode.OnRefresh);

            tagData = ReadPlc(Constant.LowerOutboundClearRoleName);
            if (tagData.TryGetTagValue(out bool clear))
            {
                LowerOutboundClear = clear;
            }
            StartPlcTagCapture(
               Constant.LowerOutboundClearRoleName,
               _LowerOutboundClear_TagValueChanged,
               XTagDataCaptureUpdateMode.OnChange);

            tagData = ReadPlc(Constant.UpperOutboundClearRoleName);
            if (tagData.TryGetTagValue(out clear))
            {
                UpperOutboundClear = clear;
            }
            StartPlcTagCapture(
               Constant.UpperOutboundClearRoleName,
               _UpperOutboundClear_TagValueChanged,
               XTagDataCaptureUpdateMode.OnChange);

            StartPlcTagCapture(
                Constant.CraneCommandRoleName,
                _CraneCommand_TagValueChanged,
                XTagDataCaptureUpdateMode.OnChange);

            tagData = ReadPlc(Constant.CraneModeRoleName);
            if (tagData.TryGetTagValue(out int mode))
            {
                CraneMode = (CraneMode)mode;
                _systemSettings.SetCraneMode(CraneNumber, CraneMode);
            }
            StartPlcTagCapture(
                Constant.CraneModeRoleName,
                _CraneMode_TagValueChanged,
                XTagDataCaptureUpdateMode.OnChange);

            StartPlcTagCapture(
                Constant.CraneSemiAutoGetLocationRoleName,
                _SemiAutoGetLocation_TagValueChanged,
                XTagDataCaptureUpdateMode.OnChange);

            StartPlcTagCapture(
                Constant.CraneSemiAutoPutLocationRoleName,
                _SemiAutoPutLocation_TagValueChanged,
                XTagDataCaptureUpdateMode.OnChange);

            _storage.DataItemChanged += _Storage_DataItemChanged;
            _lowerPit.DataItemChanged += _LowerPit_DataItemChanged;
            _upperPit.DataItemChanged += _UpperPit_DataItemChanged;
            _systemSettings.DataItemChanged += _SystemSettings_DataItemChanged;
            _broadcast.DataItemChanged += _Broadcast_DataItemChanged;
            _loadA.DataItemChanged += _LoadA_DataItemChanged;
            _loadB.DataItemChanged += _LoadB_DataItemChanged;

            // Always call this base class when overriding
            base.DoStart();
        }

        protected override void DoStop()
        {
            StopAllTagDataCapture(Constant.PlcRoleName);
            _systemSettings.SetCraneMode(CraneNumber, CraneMode.Manual);
            base.DoStop();
        }

        protected override void BuildStateDetails()
        {
            base.BuildStateDetails();
            if (CurrentPallet != null)
            {
                SetStateDetail(
                    CurrentPalletName,
                    CurrentPallet.GetPalletStateDetails(Constant.OperationDetailsLeadingSpaceCount),
                    false);
            }
            else
            {
                RemoveStateDetail(CurrentPalletName, false);
            }
            if (CurrentLoadItem != null)
            {
                SetStateDetail(
                    CurrentLoadItemName,
                    CurrentLoadItem.GetLoadItemStateDetails(Constant.OperationDetailsLeadingSpaceCount),
                    false);
            }
            else
            {
                RemoveStateDetail(CurrentLoadItemName, false);
            }
//             if (CurrentPendingRequirementsItem != null)
//             {
//                 SetStateDetail(
//                     CurrentPendingRequirementsItemName,
//                     CurrentPendingRequirementsItem.GetPendingRequirementsStateDetails(Constant.OperationDetailsLeadingSpaceCount),
//                     false);
//             }
//             else
//             {
//                 RemoveStateDetail(CurrentPendingRequirementsItemName, false);
//             }
        }

        protected override void DoRewind()
        {
            SetExtendedState(string.Empty, true);

            if (!AutoMode)
            {
                if (SemiAutoMode)
                {
                    TelemetrySetCurrentState(MonitoringSemiAutoState);
                }
                else // ManualMode
                {
                    WaitingStateName = string.Empty;
                    TelemetrySetCurrentState(AwaitingCraneAutoState);
                }
                return;
            }

            if (_startup)
            {
                XTagData tagData = ReadPlc(Constant.PalletOnCraneRoleName);
                if (tagData != null && tagData.Quality == XTagQuality.Good)
                {
                    if (tagData.TryGetTagValue(out bool palletOnCrane))
                    {
                        PalletOnCrane = palletOnCrane;
                    }
                }
                tagData = ReadPlc(Constant.PalletIDRoleName);
                if (tagData != null && tagData.Quality == XTagQuality.Good)
                {
                    if (tagData.TryGetTagValue(out string palletID))
                    {
                        PalletID = palletID;
                    }
                }
                if (PalletPartlyReadyOnCrane)
                {
                    TelemetrySetCurrentState(AwaitingPalletReadyOnStartupState);
                    return;
                }
                else if (PalletReadyOnCrane)
                {
                    TelemetrySetCurrentState(RecoveringOnboardPalletState);
                    return;
                }
            }
            _startup = false;
            TelemetrySetCurrentState(AwaitingGetTaskState);
        }

        #endregion

        //==================================================================================

        protected bool CheckForFault(string fault)
        {
            if (fault.IsNullOrWhiteSpace())
            {
                return false;
            }
            TelemetrySetCraneFaulted(fault);
            return true;
        }
        protected void ClearCraneFault()
        {
            PublishTelemetryOut($"Clear Fault Condition ({CraneCommand})", 0);
            CraneCommand = Constant.NoCraneCommand;
            WritePlc(
                Constant.CraneCommandRoleName,
                Constant.NoCraneCommand);
        }

        protected void TelemetrySetCraneFaulted(string fault)
        {
            SetFaulted(fault);
            PublishCraneStateTelemetry("Faulted!");
            WritePlc(
                Constant.SoftwareFaultRoleName,
                true);
            PublishTelemetryOut($"Set Software Fault Condition", true.ToString());
        }

        protected void ClearCraneFaultInPlc()
        {
            WritePlc(
                Constant.SoftwareFaultRoleName,
                false);
            PublishTelemetryOut($"Clear Software Fault Condition", false.ToString());
        }

        protected bool FetchPitPallet(
            string palletID,
            out PalletItem palletItem,
            out string fault)
        {
            if (!DataLayer.TryGetPitPallet(palletID, out palletItem))
            {
                return MesInterface.TryFetchPalletItem(palletID, out palletItem, out fault);
            }
//             DataLayer.RemovePitPallet(palletID);
            fault = null;
            return true;
        }

        protected bool QueryMesPallet(
            string palletID,
            out PalletItem palletItem,
            out string fault)
        {
            return MesInterface.TryFetchPalletItem(palletID, out palletItem, out fault);
        }

        //==================================================================================

        #region AwaitingPalletReadyOnStartupState

        protected readonly string AwaitingPalletReadyOnStartupState = "AwaitingPalletReady";

        protected virtual void AwaitingPalletReadyOnStartupStateHandler()
        {
            if (PalletReadyOnCrane)
            {
                TelemetrySetCurrentState(RecoveringOnboardPalletState);
            }
        }

        protected bool PalletReadyOnCrane => ValidPalletID && PalletOnCrane;

        protected bool PalletPartlyReadyOnCrane => (ValidPalletID && !PalletOnCrane)
            || (!ValidPalletID && PalletOnCrane);

        protected bool ValidPalletID => PalletID.ValidPalletID();

//         protected bool NoReadPalletID => PalletID == Constant.NoRead;

//         protected bool ValidInboundPalletID => InboundPalletID.Valid() && !LostInboundPalletID;

//         protected bool LostInboundPalletID => PalletID == Constant.LostPalletID;

        //         protected void DoPalletReady()
        //         {
        //             // override for each Crane Operation derived class
        //         }

        #endregion

        //==================================================================================

        #region RecoveringOnboardPalletState

        protected readonly string RecoveringOnboardPalletState = "RecoveringOnboardPallet";

        protected virtual void RecoveringOnboardPalletStateHandler()
        {
            _startup = false;

            if (!QueryMesPallet(PalletID, out PalletItem palletItem, out string fault))
            {
                TelemetrySetCraneFaulted(fault);
            }

            if (palletItem.IsStack)
            {
                DataLayer.ClearStorageBinByPalletID(PalletID);
                DataLayer.RemovePitPallet(PalletID);
                CurrentPallet = palletItem;
                CurrentCraneFunction = CraneFunction.Store;
            }
            else if (DataLayer.TryGetRecoveryLoadItem(
                PalletID,
                out LoadItem loadItem))
            {
                DataLayer.ClearStorageBinByPalletID(PalletID);
                ClearCraneFault();
                CurrentLoadItem = loadItem;
                CurrentPallet = loadItem.Pallet;
                CurrentCraneFunction = CraneFunction.LoadPick;
            }
            else if (DataLayer.TryCleanUpPitPalletOnCrane(
                PalletID,
                out palletItem,
                out BinItem binItem,
                out CraneFunction craneFunction))
            {
                if (binItem == null)
                {
                    CurrentPallet = palletItem;
                }
                else
                {
                    GetCommand = binItem.BinStatus == BinStatus.PutAllocated
                        ? InboundLocation
                        : binItem.Location;
                    CurrentPallet = binItem.Pallet;
                }
                ClearCraneFault();
                CurrentCraneFunction = craneFunction;
            }
            else
            {
                ClearGetLocationError();
                CurrentPallet = palletItem;
                CurrentCraneFunction = CraneFunction.Audit;
            }

            ClearExtendedState(true);

            if (CurrentPallet != null)
            {
                PalletEventTracker.Capture(
                    CurrentPallet,
                    OperationCode,
                    PalletEvent.RecoveringPalletOnStartUp);
            }
            else
            {
                PalletEventTracker.Capture(
                    PalletID,
                    OperationCode,
                    PalletEvent.RecoveringPalletOnStartUp);
            }

            if (DoGetCompleted())
            {
                TelemetrySetCurrentState(AssigningPutTaskState);
            }
        }

        #endregion

        //==================================================================================

        #region AwaitingCraneAutoState

        protected readonly string AwaitingCraneAutoState = "AwaitingCraneAuto";
        public string WaitingStateName { get; protected set; } = string.Empty;

        protected virtual void AwaitingCraneAutoStateHandler()
        {
            XTagData tagData = ReadPlc(Constant.CraneModeRoleName);
            if (tagData.TryGetTagValue(out int mode))
            {
                CraneMode = (CraneMode)mode;
                _systemSettings.SetCraneMode(CraneNumber, CraneMode);
            }

            if (AutoMode)
            {
                if (!string.IsNullOrWhiteSpace(WaitingStateName))
                {
                    TelemetrySetCurrentState(WaitingStateName);
                    WaitingStateName = string.Empty;
                }
                else
                {
                    SetRewind(250);
                }
            }
            else if (SemiAutoMode)
            {
                TelemetrySetCurrentState(MonitoringSemiAutoState);
                WaitingStateName = string.Empty;
            }
        }

        protected void TelemetrySetAwaitingCraneAutoState()
        {
            string currentStateName = CurrentState.Name;
            if (currentStateName != MonitoringSemiAutoState
                && currentStateName != DfxRewind)
            {
                WaitingStateName = CurrentState.Name;
            }
            TelemetrySetCurrentState(AwaitingCraneAutoState);
        }

        #endregion

        //==================================================================================

        #region MonitoringSemiAuto State

        protected readonly string MonitoringSemiAutoState = "MonitoringSemiAuto";

        protected virtual void MonitoringSemiAutoStateHandler()
        {
            if (!SemiAutoMode)
            {
                TelemetrySetAwaitingCraneAutoState();
                return;
            }
            if (SemiAutoGetCompleted)
            {
                _ProcessSemiAutoGet();
            }
            else if (SemiAutoPutCompleted)
            {
                _ProcessSemiAutoPut();
            }
        }

        protected bool SemiAutoGetCompleted =>
            SemiAutoMode
            && SemiAutoGetLocation != Constant.NoSemiAutoLocation
            && SemiAutoPutLocation == Constant.NoSemiAutoLocation
            && PalletReadyOnCrane;

        protected bool SemiAutoPutCompleted =>
            SemiAutoMode
            && SemiAutoGetLocation == Constant.NoSemiAutoLocation
            && SemiAutoPutLocation != Constant.NoSemiAutoLocation
            && !PalletOnCrane
            && !ValidPalletID;

        private void _ProcessSemiAutoGet()
        {
            if (SemiAutoGetLocation == LowerInboundLocation
                || SemiAutoGetLocation == UpperInboundLocation)
            {
                if (!FetchPitPallet(PalletID, out PalletItem palletItem, out string fault))
                {
                    TelemetrySetCraneFaulted(fault);
                    return;
                }
                CurrentPallet = palletItem;
            }
            else
            {
                if (!DataLayer.ProcessSemiAutoGet(SemiAutoGetLocation, out PalletItem palletItem))
                {
                    string fault = $"Invalid Semi Auto Get Location {SemiAutoGetLocation} received";
                    XSystemEvent.Publish(
                        ConfigurationItem.Name,
                        XSystemEventLevel.Error,
                        fault);
                    TelemetrySetCraneFaulted(fault);
                    return;
                }
                if (palletItem.Status == PalletStatus.Invalid
                    || palletItem.Sku.IsNullOrWhiteSpace()
                    || palletItem.PalletID != PalletID)
                {
                    if (!MesInterface.TryFetchPalletItem(PalletID, out palletItem, out _))
                    {
                        string fault = $"Unknown Pallet ID {PalletID} received from Crane in Semi-Auto Mode";
                        XSystemEvent.Publish(
                            ConfigurationItem.Name,
                            XSystemEventLevel.Error,
                            fault);
                        TelemetrySetCraneFaulted(fault);
                        return;
                    }
                }
                CurrentPallet = palletItem;
                SetExtendedState(
                    $"Pallet {CurrentPallet.PalletID} from location {SemiAutoGetLocation} on board",
                    true);
                PublishStateDetails();
                DataLayer.CompleteStorageGetByLocation(SemiAutoGetLocation);
            }
        }

        private void _ProcessSemiAutoPut()
        {
            if (CurrentPallet != null)
            {
                if (SemiAutoPutLocation != LowerOutboundLocation
                    && SemiAutoPutLocation != UpperOutboundLocation)
                {
                    DataLayer.DoSemiAutoPutByLocation(CurrentPallet, SemiAutoPutLocation);
                    DataLayer.RemovePitPallet(CurrentPallet.PalletID);
                    SetRewind(500);
                }
                SemiAutoPutLocation = Constant.NoSemiAutoLocation;
                ClearExtendedState(true);
                CurrentPallet = null;
                PublishStateDetails();
            }
        }

        #endregion

        //==================================================================================

        #region AwaitingGetTaskState

        protected readonly string AwaitingGetTaskState = "AwaitingGetTask";

        protected virtual void AwaitingGetTaskStateHandler()
        {
            if (!AutoMode)
            {
                TelemetrySetAwaitingCraneAutoState();
                return;
            }
            DataLayer.CleanUpAllocatedBins(CraneNumber);
            if (DoAwaitingGetTask(out int getCommand, out string extendedState))
            {
                TelemetrySetCurrentState(AwaitingGetCompletedState, extendedState);
                SendGetCommand(getCommand);
            }
        }

        protected bool DoAwaitingGetTask(out int getCommand, out string extendedState)
        {
            getCommand = Constant.NoCraneCommand;
            extendedState = string.Empty;

            bool success = false;
            if (LastCraneFunction == CraneFunction.Store)
            {
                if (_systemSettings.CanPrioritizeAuditPicks(CraneNumber))
                {
                    success = TryAuditPick(out getCommand, out extendedState, out _);
                }
                if (!success)
                {
                    foreach (PickFunction pickFunction in PickFunctionPriority)
                    {
                        if (pickFunction.PickFunc(out getCommand, out extendedState, out object extra))
                        {
                            success = true;
                            break;
                        }
                    }
                }
                if (!success)
                {
                    if (!TryStore(out getCommand, out extendedState, out string fault))
                    {
                        if (CheckForFault(fault))
                        {
                            return false;
                        }
                        success = false;
                    }
                    else
                    {
                        success = true;
                    }
                }
                if (!success)
                {
                    if (DataLayer.TryCompactStorage(CraneNumber))
                    {
                        StartStateTimer(0);
                    }
                }
            }
            else // not Store
            {
                if (_systemSettings.CanPrioritizeAuditPicks(CraneNumber))
                {
                    success = TryAuditPick(out getCommand, out extendedState, out _);
                }
                if (!success)
                {
                    if (!TryStore(out getCommand, out extendedState, out string fault))
                    {
                        if (CheckForFault(fault))
                        {
                            return false;
                        }
                        success = false;
                    }
                    else
                    {
                        success = true;
                    }
                }
                if (!success)
                {
                    foreach (PickFunction pickFunction in PickFunctionPriority)
                    {
                        if (pickFunction.PickFunc(out getCommand, out extendedState, out object extra))
                        {
                            success = true;
                            break;
                        }
                    }
                }
                if (!success)
                {
                    if (DataLayer.TryCompactStorage(CraneNumber))
                    {
                        StartStateTimer(0);
                    }
                }
            }
            return success;
        }

        protected void ClearGetLocationError()
        {
            GetCommand = Constant.NoCraneCommand;
            ClearCraneFault();
        }

        protected void SendGetCommand(int getCommand)
        {
            if (CurrentPallet != null)
            {
                PalletEventTracker.Capture(
                    CurrentPallet,
                    OperationCode,
                    PalletEvent.GetSent,
                    getCommand);
            }
            else
            {
                PalletEventTracker.Capture(
                    PalletID,
                    OperationCode,
                    PalletEvent.GetSent,
                    getCommand);
            }
            GetCommand = getCommand;
            WritePlc(
                Constant.CraneCommandRoleName,
                getCommand);
            PublishTelemetryOut("GetLocation", getCommand);
        }

        protected bool TryStore(
            out int getCommand,
            out string extendedState,
            out string fault)
        {
            getCommand = Constant.NoCraneCommand;
            extendedState = string.Empty;
            fault = string.Empty;
            if ((PalletAtLowerInbound || PalletAtUpperInbound)
                && DataLayer.CanDoStore(CraneNumber))
            {
                CurrentCraneFunction = CraneFunction.Store;
                getCommand = _GetStoreCommand();
                extendedState = $"({Constant.PalletTypeStore})  Getting Pallet from Inbound ({getCommand})";
                return true;
            }
            return false;
        }

        private int _GetStoreCommand()
        {
            // Pallet is guaranteed to be at one of the Inbound Levels
            if (_previousStoreLevel == Levels.Lower )
            {
                if (PalletAtUpperInbound)
                {
                    _previousStoreLevel = Levels.Upper;
                    return UpperInboundLocation;
                }
                else // PalletAtLowerInbound
                {
                    _previousStoreLevel = Levels.Lower;
                    return LowerInboundLocation;
                }
            }
            else
            {
                if (PalletAtLowerInbound)
                {
                    _previousStoreLevel = Levels.Lower;
                    return LowerInboundLocation;
                }
                else // PalletAtUpperInbound
                {
                    _previousStoreLevel = Levels.Upper;
                    return UpperInboundLocation;
                }
            }
        }

        protected bool TryAuditPick(
            out int getCommand,
            out string extendedState,
            out object extra)
        {
            extra = null;
            if (!DataLayer.TryAuditPick(
                CraneNumber,
                out PalletItem palletItem,
                out getCommand,
                out extendedState))
            {
                return false;
            }
            CurrentPallet = palletItem;
            CurrentCraneFunction = CraneFunction.Audit;
            return true;
        }

        protected bool TryPurgePick(
            out int getCommand,
            out string extendedState,
            out object extra)
        {
            extra = null;
            if (!DataLayer.TryPurgePick(
                CraneNumber,
                LowerOutboundClear,
                UpperOutboundClear,
                out _purgeLevel,
                out PalletItem palletItem,
                out getCommand,
                out extendedState))
            {
                return false;
            }
            CurrentCraneFunction = CraneFunction.PurgePick;
            CurrentLoadItem = null;
            CurrentPallet = palletItem;
            return true;
        }

        protected bool TryStackPick(
            out int getCommand,
            out string extendedState,
            out object extra)
        {
            extra = null;
            if (!DataLayer.TryStackPick(
                CraneNumber,
                LowerOutboundClear,
                UpperOutboundClear,
                out _stackPickLevel,
                out PalletItem palletItem,
                out getCommand,
                out extendedState))
            {
                return false;
            }
            CurrentCraneFunction = CraneFunction.StackPick;
            CurrentLoadItem = null;
            CurrentPallet = palletItem;
            return true;
        }

        protected bool TryLoadPick(
            out int getCommand,
            out string extendedState,
            out object extra)
        {
            extra = null;
            if (!DataLayer.TryLoadPick(
                CraneNumber,
                LowerOutboundClear,
                UpperOutboundClear,
                out LoadItem currentLoadItem,
                out getCommand,
                out extendedState))
            {
                return false;
            }
            CurrentCraneFunction = CraneFunction.LoadPick;
            CurrentLoadItem = currentLoadItem;
            CurrentPallet = currentLoadItem.Pallet;
            return true;
        }

        #endregion

        //==================================================================================

        #region AwaitingGetCompletedState

        protected readonly string AwaitingGetCompletedState = "AwaitingGetCompleted";

        protected virtual void AwaitingGetCompletedStateHandler()
        {
            if (InvalidLocationError)
            {
                if (CurrentPallet != null)
                {
                    PalletEventTracker.Capture(
                        CurrentPallet,
                        OperationCode,
                        PalletEvent.InvalidLocationError,
                        GetCommand);
                }
                else
                {
                    PalletEventTracker.Capture(
                        PalletID,
                        OperationCode,
                        PalletEvent.InvalidLocationError,
                        GetCommand);
                }
                XSystemEvent.Publish(
                    CraneNumber.ToText(),
                    XSystemEventLevel.Error,
                    $"{GetCommand} is an Invalid Location!");
                DataLayer.SetStorageLocationToDisabled(CraneNumber, GetCommand);
                CurrentCraneFunction = CraneFunction.None;
                ClearGetLocationError();
                ClearExtendedState(true);
                SetRewind();
            }
            else if (LocationEmptyError)
            {
                if (CurrentPallet != null)
                {
                    PalletEventTracker.Capture(
                        CurrentPallet,
                        OperationCode,
                        PalletEvent.LocationEmptyError,
                        GetCommand);
                }
                else
                {
                    PalletEventTracker.Capture(
                        PalletID,
                        OperationCode,
                        PalletEvent.LocationEmptyError,
                        GetCommand);
                }
                switch (CurrentCraneFunction)
                {
                    case CraneFunction.LoadPick:
                        DataLayer.RollBackLoadPick(CurrentPallet.PalletID, false);
                        DataLayer.ClearStorageBinByLocation(GetCommand);
                        break;
                    case CraneFunction.PurgePick:
                    case CraneFunction.StackPick:
                    case CraneFunction.Audit:
                        DataLayer.ClearStorageBinByLocation(GetCommand);
                        break;
                    case CraneFunction.Store:
                        XSystemEvent.Publish(
                            CraneNumber.ToText(),
                            XSystemEventLevel.Warning,
                            $"Received Location Empty Error for '{CurrentCraneFunction.ToText()}' Crane Operation. Rewinding Operation.");
                        break;
                    case CraneFunction.None:
                    default:
                        TelemetrySetCraneFaulted($"Received Location Empty Error for '{CurrentCraneFunction.ToText()}' Crane Operation");
                        return;
                }
                CurrentCraneFunction = CraneFunction.None;
                ClearGetLocationError();
                ClearExtendedState(true);
                SetRewind();
            }
            else if (GetCompleted)
            {
                ClearExtendedState(true);
                if (CurrentPallet != null)
                {
                    PalletEventTracker.Capture(
                        CurrentPallet,
                        OperationCode,
                        PalletEvent.GetCompleted,
                        GetCommand);
                }
                else
                {
                    PalletEventTracker.Capture(
                        PalletID,
                        OperationCode,
                        PalletEvent.GetCompleted,
                        GetCommand);
                }
                if (DoGetCompleted())
                {
                    TelemetrySetCurrentState(AssigningPutTaskState);
                }
            }
        }

        protected bool LocationEmptyError => CraneCommand == Constant.LocationEmptyError;

        protected bool InvalidLocationError => CraneCommand == Constant.InvalidLocationError;

        protected bool GetCompleted =>
            CraneCommand == Constant.NoCraneCommand
            && PalletReadyOnCrane;

        protected virtual bool DoGetCompleted()
        {
            PalletItem palletItem;
            string fault;
            switch (CurrentCraneFunction)
            {
                case CraneFunction.QAStore:
                    if (!QueryMesPallet(PalletID, out palletItem, out fault))
                    {
                        TelemetrySetCraneFaulted(fault);
                        return false;
                    }
                    CurrentPallet = palletItem;
                    DataLayer.MarkDuplicatesForAudit(CurrentPallet.PalletID);
                    break;
                case CraneFunction.Store:
                    if (!FetchPitPallet(PalletID, out palletItem, out fault))
                    {
                        TelemetrySetCraneFaulted(fault);
                        return false;
                    }
                    CurrentPallet = palletItem;
                    DataLayer.MarkDuplicatesForAudit(CurrentPallet.PalletID);
                    break;
                case CraneFunction.Audit:
                    if (_systemSettings.ForcePalletDataQueryOnAudit
                        || CurrentPallet == null
                        || (CurrentPallet != null && CurrentPallet.PalletID != PalletID))
                    {
                        if (!QueryMesPallet(PalletID, out palletItem, out fault))
                        {
                            TelemetrySetCraneFaulted(fault);
                            return false;
                        }
                        if (CurrentPallet != null && CurrentPallet.PalletID == PalletID)
                        {
                            // Preserve important data points
                            palletItem.Comment = CurrentPallet.Comment;
                            if (CurrentPallet.Status == PalletStatus.Hold
                                || CurrentPallet.Status == PalletStatus.QAPick)
                            {
                                palletItem.Status = CurrentPallet.Status;
                            }
                        }
                        CurrentPallet = palletItem;
                    }
                    DataLayer.CompleteStorageGetByLocation(GetCommand);
                    DataLayer.MarkDuplicatesForAudit(CurrentPallet.PalletID);
                    break;
                case CraneFunction.QAPick:
                    if (PalletID != CurrentPallet.PalletID)
                    {
                        ClearQAPickInProgress();
                        DataLayer.MarkDuplicatesForAudit(CurrentPallet.PalletID);
                        DataLayer.MarkDuplicatesForAudit(PalletID);
                        if (!QueryMesPallet(PalletID, out palletItem, out fault))
                        {
                            TelemetrySetCraneFaulted(fault);
                            return false;
                        }
                        XSystemEvent.Publish(
                            CraneNumber.ToText(),
                            XSystemEventLevel.Error,
                            $"Actual Q/A Pick Pallet ID {PalletID} did not match Inventory Pallet ID {CurrentPallet.PalletID} for Location {GetCommand}. Restoring pallet with correct data.");
                        CurrentPallet = palletItem;
                        CurrentCraneFunction = CraneFunction.Store;
                        PublishStateDetails();
                    }
                    DataLayer.CompleteStorageGetByLocation(GetCommand);
                    break;
                case CraneFunction.EmptyPick:
                    if (PalletID != CurrentPallet.PalletID)
                    {
                        DataLayer.MarkDuplicatesForAudit(CurrentPallet.PalletID);
                        DataLayer.MarkDuplicatesForAudit(PalletID);
                        if (!QueryMesPallet(PalletID, out palletItem, out fault))
                        {
                            TelemetrySetCraneFaulted(fault);
                            return false;
                        }
                        XSystemEvent.Publish(
                            CraneNumber.ToText(),
                            XSystemEventLevel.Error,
                            $"Actual Empty Pick Pallet ID {PalletID} did not match Inventory Pallet ID {CurrentPallet.PalletID} for Location {GetCommand}. Restoring pallet with correct data.");
                        CurrentPallet = palletItem;
                        CurrentCraneFunction = CraneFunction.Store;
                        PublishStateDetails();
                    }
                    DataLayer.CompleteStorageGetByLocation(GetCommand);
                    break;
                case CraneFunction.LoadPick:
                    if (!QueryMesPallet(PalletID, out palletItem, out fault))
                    {
                        TelemetrySetCraneFaulted(fault);
                        return false;
                    }
                    if (PalletID != CurrentPallet.PalletID)
                    {
                        DataLayer.MarkDuplicatesForAudit(CurrentPallet.PalletID);
                        DataLayer.MarkDuplicatesForAudit(PalletID);
                        XSystemEvent.Publish(
                            CraneNumber.ToText(),
                            XSystemEventLevel.Error,
                            $"Actual Load Pick Pallet ID {PalletID} did not match Inventory Pallet ID {CurrentPallet.PalletID} for Location {GetCommand}. Restored pallet with correct data and Rolled Back Pick.");
                        CurrentPallet = palletItem;
                        DataLayer.RollBackLoadPick(PalletID, true);
                        CurrentCraneFunction = CraneFunction.Store;
                        PublishStateDetails();
                    }
                    if (CurrentPallet.Sku != palletItem.Sku)
                    {
                        XSystemEvent.Publish(
                            CraneNumber.ToText(),
                            XSystemEventLevel.Error,
                            $"Actual Load Pick Pallet SKU {palletItem.Sku} did not match Inventory Pallet SKU {CurrentPallet.Sku} for Location {GetCommand}. Restored pallet with correct data and Rolled Back Pick.");
                        CurrentPallet = palletItem;
                        DataLayer.RollBackLoadPick(PalletID, true);
                        CurrentCraneFunction = CraneFunction.Store;
                        PublishStateDetails();
                    }
                    DataLayer.CompleteStorageGetByLocation(GetCommand);
                    break;
                case CraneFunction.None:
                default:
                    TelemetrySetCraneFaulted($"GET Completed with Current Crane Function of '{CurrentCraneFunction.ToText()}'");
                    return false;
            }
            DataLayer.RemovePitPallet(CurrentPallet.PalletID);
            DataLayer.RemoveEmptyPalletBufferPallet(CurrentPallet.PalletID);
            TelemetrySetCurrentState(AssigningPutTaskState, string.Empty);
            return true;
        }

        #endregion

        //==================================================================================

        #region AssigningPutTaskState

        protected readonly string AssigningPutTaskState = "AssigningPutTask";

        protected virtual void AssigningPutTaskStateHandler()
        {
            if (!AutoMode)
            {
                TelemetrySetAwaitingCraneAutoState();
                return;
            }
            if (GetCompleted) // Check needed for On-board Pallet Recovery
            {
                if (DoAssigningPutTask(out int putCommand, out string extendedState))
                {
                    TelemetrySetCurrentState(AwaitingPutCompletedState, extendedState);
                    SendPutCommand(putCommand);
                }
            }
        }

        protected void SendPutCommand(int putCommand)
        {
            if (CurrentPallet != null)
            {
                PalletEventTracker.Capture(
                    CurrentPallet,
                    OperationCode,
                    PalletEvent.PutSent,
                    putCommand);
            }
            else
            {
                PalletEventTracker.Capture(
                    PalletID,
                    OperationCode,
                    PalletEvent.PutSent,
                    putCommand);
            }
            PutCommand = putCommand;
            WritePlc(
                Constant.CraneCommandRoleName,
                putCommand);
            PublishTelemetryOut("PutLocation", putCommand);
        }

        protected virtual bool DoAssigningPutTask(out int putCommand, out string extendedState)
        {
            extendedState = string.Empty;
            putCommand = Constant.NoCraneCommand;
            switch (CurrentCraneFunction)
            {
                case CraneFunction.Store:
                case CraneFunction.Audit:
                    DataLayer.CompleteStorageGetByLocation(GetCommand);
                    if (DataLayer.TryAssignHotJobAtCrane(
                        CraneNumber,
                        CurrentPallet,
                        LowerOutboundClear,
                        UpperOutboundClear,
                        out LoadItem loadItem))
                    {
                        CurrentCraneFunction = CraneFunction.LoadPick;
                        CurrentLoadItem = loadItem;
                        putCommand = OutboundLocation(loadItem.Level);
                        extendedState = $"({Constant.PalletTypeHotJob})  Putting Hot Job Pallet {CurrentPallet.PalletID} to Outbound ({putCommand})";
                        PublishStateDetails();
                    }
                    else if (CurrentPallet.IsStack
                        && DataLayer.TryAllocateStorageStackPut(
                            CraneNumber,
                            CurrentPallet,
                            out BinItem binItem))
                    {
                        putCommand = binItem.Location;
                        string palletType = CurrentCraneFunction == CraneFunction.Store
                            ? Constant.PalletTypeStore
                            : Constant.PalletTypeAudit;
                        extendedState = $"({palletType})  Putting Stack {CurrentPallet.PalletID} to {putCommand}";
                    }
                    else if (DataLayer.TryAllocateStoragePut(
                        CraneNumber,
                        CurrentPallet,
                        out binItem))
                    {
                        putCommand = binItem.Location;
                        string palletType = CurrentCraneFunction == CraneFunction.Store
                            ? Constant.PalletTypeStore
                            : Constant.PalletTypeAudit;
                        extendedState = $"({palletType})  Putting Pallet {CurrentPallet.PalletID} to {putCommand}";
                    }
                    else
                    {
                        string fault = $"There is no available destination for Pallet {CurrentPallet.PalletID}.";
                        XSystemEvent.Publish(
                            CraneNumber.ToText(),
                            XSystemEventLevel.Error,
                            fault + " Operation faulted!");
                        TelemetrySetCraneFaulted(fault);
                        return false;
                    }
                    break;
                case CraneFunction.PurgePick:
                    putCommand = OutboundLocation(_purgeLevel);
                    extendedState = $"({Constant.PalletTypePurge})  Putting Pallet {CurrentPallet.PalletID} to Outbound ({putCommand})";
                    break;
                case CraneFunction.StackPick:
                    putCommand = OutboundLocation(_stackPickLevel);
                    extendedState = $"({Constant.PalletTypeStack})  Putting Pallet {CurrentPallet.PalletID} to Outbound ({putCommand})";
                    break;
                case CraneFunction.LoadPick:
                    putCommand = OutboundLocation(CurrentLoadItem.Level);
                    extendedState = $"({Constant.PalletTypeLoad})  Putting Pallet {CurrentPallet.PalletID} to Outbound ({putCommand})";
                    break;
                case CraneFunction.None:
                default:
                    TelemetrySetCraneFaulted($"Crane cannot assign PUT task when Current Function is {CurrentCraneFunction.ToText()}");
                    return false;
            }
            return true;
        }

        #endregion

        //==================================================================================

        #region AwaitingPutCompletedState

        protected readonly string AwaitingPutCompletedState = "AwaitingPutCompleted";

        protected virtual void AwaitingPutCompletedStateHandler()
        {
            if (InvalidLocationError)
            {
                if (CurrentPallet != null)
                {
                    PalletEventTracker.Capture(
                        CurrentPallet,
                        OperationCode,
                        PalletEvent.InvalidLocationError,
                        PutCommand);
                }
                else
                {
                    PalletEventTracker.Capture(
                        PalletID,
                        OperationCode,
                        PalletEvent.InvalidLocationError,
                        PutCommand);
                }
                XSystemEvent.Publish(
                    CraneNumber.ToText(),
                    XSystemEventLevel.Error,
                    $"{PutCommand} is an Invalid Location!");
                DataLayer.SetStorageLocationToDisabled(CraneNumber, PutCommand);
                ClearPutLocationError();
                ClearExtendedState(true);
                TelemetrySetCurrentState(AssigningPutTaskState);
                return;
            }
            else if (LocationFullError)
            {
                if (CurrentPallet != null)
                {
                    PalletEventTracker.Capture(
                        CurrentPallet,
                        OperationCode,
                        PalletEvent.LocationFullError,
                        PutCommand);
                }
                else
                {
                    PalletEventTracker.Capture(
                        PalletID,
                        OperationCode,
                        PalletEvent.LocationFullError,
                        PutCommand);
                }
                switch (CurrentCraneFunction)
                {
                    case CraneFunction.None:
                    case CraneFunction.LoadPick:
                    case CraneFunction.PurgePick:
                    case CraneFunction.StackPick:
                        ClearPutLocationError();
                        TelemetrySetCraneFaulted($"Received Location Full Error during PUT of {CurrentCraneFunction.ToText()} to Location {PutCommand}");
                        break;
                    case CraneFunction.Store:
                    case CraneFunction.Audit:
                        DataLayer.ClearStorageBinByLocation(PutCommand, true);
                        ClearPutLocationError();
                        ClearExtendedState(true);
                        TelemetrySetCurrentState(AssigningPutTaskState);
                        break;
                }
            }
            else if (PutCompleted)
            {
                if (CurrentPallet != null)
                {
                    PalletEventTracker.Capture(
                        CurrentPallet,
                        OperationCode,
                        PalletEvent.PutCompleted,
                        PutCommand);
                }
                else
                {
                    PalletEventTracker.Capture(
                        PalletID,
                        OperationCode,
                        PalletEvent.PutCompleted,
                        PutCommand);
                }
                DoPutCompleted();
                SetRewind();
            }
        }

        protected void ClearPutLocationError()
        {
            PutCommand = Constant.NoCraneCommand;
            ClearCraneFault();
        }

        protected bool LocationFullError => CraneCommand == Constant.LocationFullError;

        protected bool PalletUnloadedFromCrane => !PalletID.ValidPalletID() && !PalletOnCrane;

        protected bool PutCompleted => CraneCommand == Constant.NoCraneCommand
            && PalletUnloadedFromCrane;

        protected virtual void DoPutCompleted()
        {
            switch (CurrentCraneFunction)
            {
                case CraneFunction.Store:
                case CraneFunction.Audit:
                    DataLayer.CompleteStoragePutByLocation(PutCommand);
                    DataLayer.MarkDuplicatesForAudit(CurrentPallet.PalletID, BinItem.LocationToNodeIndex(PutCommand));
                    break;
                case CraneFunction.LoadPick:
                    if (!DataLayer.TrySetPicked(
                        CurrentPallet.PalletID,
                        out string fault))
                    {
                        XSystemEvent.Publish(
                            CraneNumber.ToText(),
                            XSystemEventLevel.Error,
                            fault);
                        TelemetrySetCraneFaulted(fault);
                        return;
                    }
                    break;
                case CraneFunction.PurgePick:
                    DataLayer.SetPitPallet(
                        _LevelFromOutboundLocation(PutCommand),
                        CurrentPallet,
                        PitCode.Purge);
                    break;
                case CraneFunction.StackPick:
                    DataLayer.SetPitPallet(
                        _LevelFromOutboundLocation(PutCommand),
                        CurrentPallet,
                        PitCode.Stack);
                    break;
                case CraneFunction.None:
                default:
                    TelemetrySetCraneFaulted($"Received Put Completed for Current Crane Operation '{CurrentCraneFunction.ToText()}'");
                    break;
            }
        }

        private Levels _LevelFromOutboundLocation(int outboundLocation)
        {
            return outboundLocation % 2 == 1
                ? Levels.Lower
                : Levels.Upper;
        }

        #endregion

        //==================================================================================

        private void _CraneMode_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out int mode))
            {
                if ((CraneMode)mode == CraneMode)
                {
                    return;
                }
                CraneMode previousCraneMode = CraneMode;
                CraneMode = (CraneMode)mode;
                _systemSettings.SetCraneMode(CraneNumber, CraneMode);

                PublishTelemetryIn("CraneMode", $"{previousCraneMode.ToText()}=>{CraneMode.ToText()}");

                string currentStateName = CurrentState.Name;
                if (previousCraneMode == CraneMode.Manual
                    && currentStateName == AwaitingCraneAutoState)
                {
                    RunCurrentStateHandler();
                }
                else if (previousCraneMode == CraneMode.Auto
                    && ManualMode)
                {
                    TelemetrySetAwaitingCraneAutoState();
                }
                else if (previousCraneMode == CraneMode.SemiAuto
                    && ManualMode)
                {
                    TelemetrySetAwaitingCraneAutoState();
                }
            }
        }

        private void _CraneCommand_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out int craneCommand))
            {
                if (CraneCommand != craneCommand)
                {
                    PublishTelemetryIn("CraneCommand", craneCommand);
                }
                CraneCommand = craneCommand;
                string currentStateName = CurrentState.Name;
                if (CraneCommand == Constant.CraneFault)
                {
                    ClearCraneFault();
                    TelemetrySetCraneFaulted($"Received Crane Fault Response ({Constant.CraneFault}) from Crane");
                }
                else if (CraneCommand <= Constant.NoCraneCommand // No command OR LocationFull (-2) OR LocationEmpty (-3) OR InvalidLocation (-4)
                    && (currentStateName == AwaitingGetCompletedState
                        || currentStateName == AwaitingPutCompletedState))
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _PalletID_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out string palletID))
            {
                if (PalletID != palletID.ToUpper())
                {
                    PublishTelemetryIn("PalletID", palletID);
                }
                PalletID = palletID.ToUpper();
                string currentStateName = CurrentState.Name;
                if ((currentStateName == AwaitingGetCompletedState && PalletReadyOnCrane)
                    || (currentStateName == AwaitingPutCompletedState && PalletUnloadedFromCrane)
                    || (currentStateName == AwaitingPalletReadyOnStartupState && PalletReadyOnCrane)
                    || currentStateName == MonitoringSemiAutoState)
                {
                    RunCurrentStateHandler();
                }
                else if (currentStateName == AwaitingPalletReadyOnStartupState
                    && PalletUnloadedFromCrane)
                {
                    _startup = false;
                    TelemetrySetCurrentState(AwaitingGetTaskState);
                }
            }
        }

        private void _PalletOnCrane_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out bool palletOnCrane))
            {
                if (PalletOnCrane != palletOnCrane)
                {
                    PublishTelemetryIn("PalletOnCrane", palletOnCrane.ToString());
                }
                PalletOnCrane = palletOnCrane;
                string currentStateName = CurrentState.Name;
                if ((currentStateName == AwaitingGetCompletedState && PalletReadyOnCrane)
                    || (currentStateName == AwaitingPutCompletedState && PalletUnloadedFromCrane)
                    || (currentStateName == AwaitingPalletReadyOnStartupState && PalletReadyOnCrane)
                    || currentStateName == MonitoringSemiAutoState)
                {
                    RunCurrentStateHandler();
                }
                else if (currentStateName == AwaitingPalletReadyOnStartupState
                    && PalletUnloadedFromCrane)
                {
                    _startup = false;
                    TelemetrySetCurrentState(AwaitingGetTaskState);
                }
            }
        }

        private void _PalletAtLowerInbound_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out bool palletAtInbound))
            {
                if (PalletAtLowerInbound != palletAtInbound)
                {
                    PublishTelemetryIn("PalletAtLowerInbound", palletAtInbound.ToString());
                }
                PalletAtLowerInbound = palletAtInbound;
                string currentStateName = CurrentState.Name;
                if (currentStateName == AwaitingGetTaskState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _PalletAtUpperInbound_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out bool palletAtInbound))
            {
                if (PalletAtUpperInbound != palletAtInbound)
                {
                    PublishTelemetryIn("PalletAtUpperInbound", palletAtInbound.ToString());
                }
                PalletAtUpperInbound = palletAtInbound;
                string currentStateName = CurrentState.Name;
                if (currentStateName == AwaitingGetTaskState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _LowerOutboundClear_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out bool outboundClear))
            {
                if (LowerOutboundClear != outboundClear)
                {
                    PublishTelemetryIn("LowerOutboundClear", outboundClear.ToString());
                }
                LowerOutboundClear = outboundClear;
                if (CurrentState.Name == AwaitingGetTaskState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _UpperOutboundClear_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out bool outboundClear))
            {
                if (UpperOutboundClear != outboundClear)
                {
                    PublishTelemetryIn("UpperOutboundClear", outboundClear.ToString());
                }
                UpperOutboundClear = outboundClear;
                if (CurrentState.Name == AwaitingGetTaskState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _SemiAutoGetLocation_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out int getLocation))
            {
                if (SemiAutoGetLocation != getLocation)
                {
                    PublishTelemetryIn("SemiAutoGetLocation", getLocation);
                }
                SemiAutoGetLocation = getLocation;
                if (CurrentState.Name == MonitoringSemiAutoState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        private void _SemiAutoPutLocation_TagValueChanged(object sender, XTagDataEventArgs e)
        {
            if (e.TagData.TryGetTagValue(out int putLocation))
            {
                if (SemiAutoPutLocation != putLocation)
                {
                    PublishTelemetryIn("SemiAutoPutLocation", putLocation);
                }
                if (putLocation > Constant.NoSemiAutoLocation)
                {
                    SemiAutoPutLocation = putLocation;
                }
                if (CurrentState.Name == MonitoringSemiAutoState)
                {
                    RunCurrentStateHandler();
                }
            }
        }

        //==================================================================================

        private void _SystemSettings_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            TelemetryEnabled = _systemSettings.CanGenerateCraneTelemetry(CraneNumber);

            string currentStateName = CurrentState.Name;
            if (currentStateName == AwaitingGetTaskState
                || currentStateName == AssigningPutTaskState)
            {
                RunCurrentStateHandler();
            }
        }

        private void _Storage_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            string currentStateName = CurrentState.Name;
            if (currentStateName == AwaitingGetTaskState
                || currentStateName == AssigningPutTaskState)
            {
                RunCurrentStateHandler();
            }
        }

        private void _LowerPit_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingGetTaskState)
            {
                RunCurrentStateHandler();
            }
        }

        private void _UpperPit_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingGetTaskState)
            {
                RunCurrentStateHandler();
            }
        }

        private void _Broadcast_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingGetTaskState)
            {
                RunCurrentStateHandler();
            }
        }

        private void _LoadA_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingGetTaskState)
            {
                RunCurrentStateHandler();
            }
        }

        private void _LoadB_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            if (CurrentState.Name == AwaitingGetTaskState)
            {
                RunCurrentStateHandler();
            }
        }

        protected override void DoDispose()
        {
            if (_storage != null)
            {
                _storage.DataItemChanged -= _Storage_DataItemChanged;
            }
            if (_lowerPit != null)
            {
                _lowerPit.DataItemChanged -= _LowerPit_DataItemChanged;
            }
            if (_upperPit != null)
            {
                _upperPit.DataItemChanged -= _UpperPit_DataItemChanged;
            }
            if (_systemSettings != null)
            {
                _systemSettings.DataItemChanged -= _SystemSettings_DataItemChanged;
            }
            if (_broadcast != null)
            {
                _broadcast.DataItemChanged -= _Broadcast_DataItemChanged;
            }
            if (_loadA != null)
            {
                _loadA.DataItemChanged -= _LoadA_DataItemChanged;
            }
            if (_loadB != null)
            {
                _loadB.DataItemChanged -= _LoadB_DataItemChanged;
            }

            DataLayer.Dispose();
            DataLayer = null;

            base.DoDispose();
        }

    }
}
