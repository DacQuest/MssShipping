using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public static class Constant
    {
        public static readonly DateTime BeginningOfTime = DateTime.Parse("01/01/2000 00:00:00");
        public static readonly DateTime BeforeBeginningOfTime = DateTime.Parse("12/31/1999 11:59:59");
        public static readonly string LongDateTimeFormat24 = "yyyy-MM-dd HH:mm:ss.fff";
        public static readonly string DateTimeFormat24 = "yyyy-MM-dd HH:mm:ss";
        public static readonly string DisplayDateTimeFormat12 = "MM/dd/yyyy hh:mm:ss tt";

        public const string PalletTrackerTableName = "PalletTracker";
        public const string PurgePalletsTableName = "PurgePallets";

        // Connection String Names
        public const string ArchiveConnectionStringName = "ArchiveConnectionString";
        public const string MesConnectionStringName = "MesConnectionString";

        public const string PalletStatusChangeEventContext = "Pallet Status Change";

        public const string ShippingLabelName = "ShippingLabelP708";
        public const string TrailerLabelName = "TrailerLabelP708";

        // Shared Collection Names
        public const string StorageName = "Storage";
        public const string AssignmentPitName = "AssignmentPit";
        public const string LowerPitName = "LowerPit";
        public const string UpperPitName = "UpperPit";
        public const string SystemSettingsName = "SystemSettings";
        public const string HoldCodesName = "HoldCodes";
        public const string BroadcastName = "Broadcast";
        public const string LowerRecircName = "LowerRecircBuffer";
        public const string UpperRecircName = "UpperRecircBuffer";
        public const string SlugAName = "SlugA";
        public const string SlugBName = "SlugB";
        public const string PlcTagsName = "PlcTags";

        public const int OperationDetailsLeadingSpaceCount = 6;

        public const string TransferTelemetryEnabledName = "TransferTelemetryEnabled";


        public const int MaxSkuLength = 50;
        public const int MaxBroadcastSkip = 100;
        public const int MaxRotation = 9998;
        public const int MaxPalletIDLength = 10;
        public const int PalletIDLength = 4;
        public const int CsnLength = 20;
        public const int VinLength = 20;
        public const int CommentLength = 50;
        public const int HoldCodeDescriptionLength = 50;
        public const int JobIDLength = 50;
        public const int ActualSkuLength = 15;
        public const int PickModeKeyLength = 50;
        public const int NoHoldCode = 0;
        public const string NoHoldCodeDescription = "No Hold Code";
        public const int IgnoreHoldCode = -1;

        public const string VehicleRow1CsnSuffix = "F";
        public const string VehicleRow2CsnSuffix = "B";
        public const string RotationNumberTextFormat = "0000000";

        //Broadcast
        public const string CurrentBroadcastQuery = "CurrentBroadcastQuery";

        //Storage
        public const string StackSku1 = "STACK1";
        public const string StackSku2 = "STACK2";
        public const int MaxCranes = 4;
        public const int MaxHorizontal = 23;
        public const int MaxVertical = 6;
        public const int CraneSides = 2;
        public const int StoragePerCrane = MaxHorizontal * MaxVertical * CraneSides;
        public const int StorageSize = MaxCranes * StoragePerCrane;

        public const int LoadSize = 54;
        public const int MaxActivePalletsPerLevel = 12;
        public const int MaxActivePalletsPerLoadLevel = 6;
        public const int MaxPalletsPerRecirc = 3;

        // Pallet Types
        public const string PalletTypeStore = "STORE";
        public const string PalletTypeLoad = "LOAD";
        public const string PalletTypeAudit = "AUDIT";
        public const string PalletTypePurge = "PURGE";
        public const string PalletTypeStack1 = "STACK1";
        public const string PalletTypeStack2 = "STACK2";
        public const string PalletTypeHotJob = "HOTJOB";

        // Load
        public const int NoLoadNumber = 0;
        public const int SlugLanes = 3;
//         public const int LoadPickableCount = 12;
//         public const int LoadPickableCountByLevel = 6;
        public const int LoadCellCharacterWidth = 25;
        public const string CalculateShortagesMessageTopicName = "CalculateShortages";
//         public const int LoadAllocatablePositions = 6;
        public const string NoTrailerID = "";
        public const string Row1EmptyPalletSku = "ROW1EMPTY";
        public const string Row2EmptyPalletSku = "ROW2EMPTY";

        // Device Role Names
        public const string PlcRoleName = "Plc";

        // Tag Role Names
        public const string LabelPrinterTesterDeviceSetName = "LabelPrinterTester";
        public const string LabelPrintCommandRoleName = "LabelPrintCommand";
        public const string PalletIDRoleName = "PalletID";
        public const string MoveCommandRoleName = "MoveCommand";
        public const string SoftwareFaultRoleName = "SoftwareFault";
        public const string PlcFaultRoleName = "PlcFault";

        public const string SizingTestResultRoleName = "SizingTestResult";

        public const string PalletOnCraneRoleName = "PalletOnCrane";
        public const string CraneCommandRoleName = "CraneCommand";
        public const string CraneModeRoleName = "CraneMode";
        public const string LowerInboundPalletIDRoleName = "LowerInboundPalletID";
        public const string UpperInboundPalletIDRoleName = "UpperInboundPalletID";
        public const string LowerOutboundClearRoleName = "LowerOutboundClear";
        public const string UpperOutboundClearRoleName = "UpperOutboundClear";
        public const string CraneSemiAutoGetLocationRoleName = "CraneSemiAutoGetLocation";
        public const string CraneSemiAutoPutLocationRoleName = "CraneSemiAutoPutLocation";

        public const string LoadAUpperLevelCompletedRoleName = "LoadAUpperLevelCompleted";
        public const string LoadALowerLevelCompletedRoleName = "LoadALowerLevelCompleted";
        public const string LoadBUpperLevelCompletedRoleName = "LoadBUpperLevelCompleted";
        public const string LoadBLowerLevelCompletedRoleName = "LoadBLowerLevelCompleted";

        public const string LowerLevelCompletedRoleName = "LowerLevelCompleted";
        public const string UpperLevelCompletedRoleName = "UpperLevelCompleted";
        public const string TrailerIDRoleName = "TrailerID";
        public const string TrailerTypeRoleName = "TrailerType";
        public const string TrailerLoadedRoleName = "TrailerLoaded";


        // Crane Errors and Faults
        public const int CraneFault           = -1;
        public const int LocationFullError    = -2;
        public const int LocationEmptyError   = -3;
        public const int InvalidLocationError = -4;

        // Cranes
        public const int NoCraneCommand = 0;
        public const int NoSemiAutoLocation = 0;
        public const int CraneInboundBufferSize = 2;
        public const string CraneTelemetryTopicNameBase = "CraneTelemetry";
        public const int MasterSettingArrayIndex = 0;
        public const CraneNumber LastCraneNumber = CraneNumber.Crane4;
        public const int Crane1LowerInboundLocation  = 11001;
        public const int Crane1LowerOutboundLocation = 12001;
        public const int Crane2LowerInboundLocation  = 21001;
        public const int Crane2LowerOutboundLocation = 22001;
        public const int Crane3LowerInboundLocation  = 31001;
        public const int Crane3LowerOutboundLocation = 32001;
        public const int Crane4LowerInboundLocation  = 41001;
        public const int Crane4LowerOutboundLocation = 42001;
        public const int Crane1UpperInboundLocation  = 11002;
        public const int Crane1UpperOutboundLocation = 12002;
        public const int Crane2UpperInboundLocation  = 21002;
        public const int Crane2UpperOutboundLocation = 22002;
        public const int Crane3UpperInboundLocation  = 31002;
        public const int Crane3UpperOutboundLocation = 32002;
        public const int Crane4UpperInboundLocation  = 41002;
        public const int Crane4UpperOutboundLocation = 42002;

        public const int UpperAssignmentBufferSize = 2;
        public const int LowerAssignmentBufferSize = 1;
        public const int ConsoleAssignmentBufferSize = 5;

        // Operation Move Commands
        public const int NoMoveCommand = 0;

        public const int Assignment1MoveCommandLower = 1;
        public const int Assignment1MoveCommandForward = 2;

        public const int Assignment2MoveCommandLower = 1;
        public const int Assignment2MoveCommandUpper = 2;
        public const int Assignment2MoveCommandConsole = 3;

        public const int Assignment3MoveCommandLower = 1;
        public const int Assignment3MoveCommandUpper = 2;
        public const int Assignment3MoveCommandConsole = 3;

        public const int RouterMoveCommandForward = 1;
        public const int RouterMoveCommandToCrane = 2;

        public const int LoadDirectorMoveCommandRelease = 1;

        public const int RecircRouterMoveCommandForward = 1;
        public const int RecircRouterMoveCommandToRecircBuffer = 2;

        public const int RecircBufferMoveCommandRelease = 1;

        public const int PurgeMoveCommandForward = 1;
        public const int PurgeMoveCommandToPurgeLane = 2;

        public const int TransferFinalPurgeMoveCommand = 13;
        public const int TransferStackMoveCommand      = 14; //Upper Level only


        public const int DeviceNameLength = 100;
        public const int DisplayNameLength = 100;
        public const int RoleNameLength = 100;
        public const int TagNameLength = 100;
        public const int PlcTagNameLength = 100;
        public const int StringValueLength = 100;
        public const int LoadTypeLength = 10;



        public const string AutoDetectedDuplicatePalletIDComment = "Auto-detected duplicate Pallet ID";
        public const int DuplicatePalletIDHoldCode = 256;
        public const string AutoDetectedDuplicateJobIDComment = "Auto-detected duplicate Job ID";
        public const int DuplicateJobIDHoldCode = 257;

        public const string Row1PalletIDValidatorName = "Row1PalletIDValidator";
        public const string Row2PalletIDValidatorName = "Row2PalletIDValidator";
        public static readonly string NoPalletID = string.Empty;
        public static readonly string NoJobID = string.Empty;

        public const string PalletIDSequenceTelemetryEnabledName = "PalletIDSequenceTelemetryEnabled";
        public const string StorageViewInitialNodeIndexMessageName = "StorageViewInitialNodeIndex";
        public const string AdminStorageViewName = "AdminStorageView";


        public const string LD_OperatorResponseName = "LD_OperatorResponse";
        public const string LD_RequestPalletDataName = "LD_RequestPallet";
        public const string LD_PalletItemName = "LD_PalletItem";
        public const string LD_IsAutoModeName = "LD_IsAutoMode";
        public const string LD_LoadItemName = "LD_LoadItem";
        public const string LD_AutoReleaseNonLoadPalletsInManualModeName = "LD_AutoReleaseNonLoadPalletsInManualMode";
        public const string LD_IsAwaitingOperatorResponseName = "LD_IsAwaitingOperatorResponse";
        public const string LD_BroadcastSkuMismatchName = "LD_BroadcastSkuMismatch";

        // === THESE ARRAYS APPLY TO LOADS OF 54 PALLETS ====================================================

        public static readonly int[] UpperLevelStartIndexes = new[] { 0, 1, 2, 24, 25, 26 };
        public static readonly int[] LowerLevelStartIndexes = new[] { 27, 28, 29, 51, 52, 53 };

        public static readonly int[] PickSearchOrder = new int[]
        {
            // Row 9
            00, 28, 02,
            27, 01, 29,
            // Row 1
            24, 52, 26,
            51, 25, 53,
            // Row 8
            03, 31, 05,
            30, 04, 32,
            // Row 2
            21, 49, 23,
            48, 22, 50,
            // Row 7
            06, 34, 08,
            33, 07, 35,
            // Row 3
            18, 46, 20,
            45, 19, 47,
            // Row 6
            09, 37, 11,
            36, 10, 38,
            // Row 4
            15, 43, 17,
            42, 16, 44,
            // Row 5
            12, 40, 14,
            39, 13, 41
        };

        public const int AfterLast = -1;
        public static readonly int[] NextInLaneLoadIndex = new int[]
        {
            //Upper Level
            03, 04, 05,
            06, 07, 08,
            09, 10, 11,
            12, 13, 14,
            AfterLast, AfterLast, AfterLast,

            AfterLast, AfterLast, AfterLast,
            15, 16, 17,
            18, 19, 20,
            21, 22, 23,

            //Lower Level
            30, 31, 32,
            33, 34, 35,
            36, 37, 38,
            39, 40, 41,
            AfterLast, AfterLast, AfterLast,

            AfterLast, AfterLast, AfterLast,
            42, 43, 44,
            45, 46, 47,
            48, 49, 50
        };

        public const int BeforeFirst = -1;
        public static readonly int[] PreviousInLaneLoadIndex = new int[]
        {
            //Lower
            BeforeFirst, BeforeFirst, BeforeFirst,
            00, 01, 02,
            03, 04, 05,
            06, 07, 08,
            09, 10, 11,

            18, 19, 20,
            21, 22, 23,
            24, 25, 26,
            BeforeFirst, BeforeFirst, BeforeFirst,

            // Upper
            BeforeFirst, BeforeFirst, BeforeFirst,
            27, 28, 29,
            30, 31, 32,
            33, 34, 35,
            36, 37, 38,

            45, 46, 47,
            48, 49, 50,
            51, 52, 53,
            BeforeFirst, BeforeFirst, BeforeFirst,
        };


//         // === BELOW THIS LINE ARE ARRAYS THAT APPLY TO LOADS OF 60 PALLETS ======================================
// 
//         public static readonly int[] PickSearchOrder = new int[]
//         {
//             00, 59, 02,
//             57, 30, 27,
//             32, 29, 58,
//             01, 28, 31,
// 
//             03, 56, 05,
//             54, 33, 24,
//             35, 26, 55,
//             04, 25, 34,
// 
//             06, 53, 08,
//             51, 36, 21,
//             38, 23, 52,
//             07, 22, 37,
// 
//             09, 50, 11,
//             48, 39, 18,
//             41, 20, 49,
//             10, 19, 40,
// 
//             12, 47, 14,
//             45, 42, 15,
//             44, 17, 46,
//             13, 16, 43
//         };
// 
//         public const int AfterLast = -1;
//         public static readonly int[] NextInLaneLoadIndex = new int[]
//         {
//             //Lower Level
//             03, 04, 05,
//             06, 07, 08,
//             09, 10, 11,
//             12, 13, 14,
//             AfterLast, AfterLast, AfterLast,
// 
//             AfterLast, AfterLast, AfterLast,
//             15, 16, 17,
//             18, 19, 20,
//             21, 22, 23,
//             24, 25, 26,
// 
//             //Upper Level
//             33, 34, 35,
//             36, 37, 38,
//             39, 40, 41,
//             42, 43, 44,
//             AfterLast, AfterLast, AfterLast,
// 
//             AfterLast, AfterLast, AfterLast,
//             45, 46, 47,
//             48, 49, 50,
//             51, 52, 53,
//             54, 55, 56
//         };
// 
//         public const int BeforeFirst = -1;
//         public static readonly int[] _previousInLaneLoadIndex = new int[]
//         {
//             //Lower
//             BeforeFirst, BeforeFirst, BeforeFirst,
//             00, 01, 02,
//             03, 04, 05,
//             06, 07, 08,
//             09, 10, 11,
// 
//             18, 19, 20,
//             21, 22, 23,
//             24, 25, 26,
//             27, 28, 29,
//             BeforeFirst, BeforeFirst, BeforeFirst,
// 
//             // Upper
//             BeforeFirst, BeforeFirst, BeforeFirst,
//             30, 31, 32,
//             33, 34, 35,
//             36, 37, 38,
//             39, 40, 41,
// 
//             48, 49, 50,
//             51, 52, 53,
//             54, 55, 56,
//             57, 58, 59,
//             BeforeFirst, BeforeFirst, BeforeFirst,
//         };
    }
}
