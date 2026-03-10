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

        // Connection String Names
        public const string ArchiveConnectionStringName = "ArchiveConnectionString";
        public const string MesConnectionStringName = "MesConnectionString";

        // Shared Collection Names
        public const string StorageName = "Storage";
        public const string LowerPitName = "LowerPit";
        public const string UpperPitName = "UpperPit";
        public const string SystemSettingsName = "SystemSettings";
        public const string BroadcastName = "Broadcast";
        public const string LoadAName = "LoadA";
        public const string LoadBName = "LoadB";

        public const int OperationDetailsLeadingSpaceCount = 6;


        public const int PalletIDLength = 4;
        public const int SkuLength = 12;




        //Storage
        //public const string StackPalletIDValidator = "StackPalletIDValidator";
        public const string StackSku = "STACK";
        public const int MaxCranes = 4;
        public const int MaxHorizontal = 22;
        public const int MaxVertical = 6;
        public const int CraneSides = 2;
        public const int StoragePerCrane = MaxHorizontal * MaxVertical * CraneSides;
        public const int StorageSize = MaxCranes * StoragePerCrane;

        public const int LoadSize = 54;

        // Pallet Types
        public const string PalletTypeStore = "STORE";
        public const string PalletTypeLoad = "LOAD";
        public const string PalletTypeAudit = "AUDIT";
        public const string PalletTypePurge = "PURGE";
        public const string PalletTypeStack = "STACK";
        public const string PalletTypeHotJob = "HOTJOB";

        // Slug/Load
        public const int NoLoadID = 0;
        public const int SlugLanes = 3;
//         public const int LoadPickableCount = 12;
//         public const int LoadPickableCountByLevel = 6;
        public const int NoPairSequence = 0;
        public const int LoadCellCharacterWidth = 25;
        public const string CalculateShortagesMessageTopicName = "CalculateShortages";


        // Device Role Names
        public const string PlcRoleName = "CC1";

        // Tag Role Names
        public const string PalletIDRoleName = "PalletID";
        public const string MoveCommandRoleName = "MoveCommand";
        public const string SoftwareFaultRoleName = "SoftwareFault";
        public const string PlcFaultRoleName = "PlcFault";

        public const string PalletOnCraneRoleName = "PalletOnCrane";
        public const string CraneCommandRoleName = "CraneCommand";
        public const string CraneModeRoleName = "CraneMode";
        public const string PalletAtLowerInboundRoleName = "PalletAtLowerInbound";
        public const string PalletAtUpperInboundRoleName = "PalletAtUpperInbound";
        public const string LowerOutboundClearRoleName = "LowerOutboundClear";
        public const string UpperOutboundClearRoleName = "UpperOutboundClear";
        public const string CraneSemiAutoGetLocationRoleName = "CraneSemiAutoGetLocation";
        public const string CraneSemiAutoPutLocationRoleName = "CraneSemiAutoPutLocation";


        // Crane Errors and Faults
        public const int CraneFault           = -1;
        public const int LocationFullError    = -2;
        public const int LocationEmptyError   = -3;
        public const int InvalidLocationError = -4;

        // Cranes
        public const int NoCraneCommand = 0;
        public const int NoSemiAutoLocation = 0;
        public const int CraneInboundBufferSize = 3;
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

        // Operation Move Commands
        public const int NoMoveCommand = 0;

        public const int IRMoveCommandForward = 1;
        public const int IRMoveCommandToCrane = 2;


//         public const int TFMoveCommandToSlugLaneA1 = 1;
//         public const int TFMoveCommandToSlugLaneA2 = 2;
//         public const int TFMoveCommandToSlugLaneA3 = 3;
//         public const int TFMoveCommandToSlugLaneB1 = 4;
//         public const int TFMoveCommandToSlugLaneB2 = 5;
//         public const int TFMoveCommandToSlugLaneB3 = 6;




        public const string AutoDetectedDuplicatePalletIDComment = "Auto-detected duplicate Pallet ID";
        public const int DuplicatePalletIDHoldCode = 256;
        public const string AutoDetectedDuplicateJobIDComment = "Auto-detected duplicate Job ID";
        public const int DuplicateJobIDHoldCode = 257;

        public const string Row1PalletIDValidatorName = "Row1PalletIDValidator";
        public const string Row2PalletIDValidatorName = "Row2PalletIDValidator";
        public static readonly string NoPalletID = string.Empty;
        public const int NoJobID = 0;






        public const int CsnLength = 15;
        public const int MaxSkuLength = 20;
        public const int VinLength = 20;
        public const int CommentLength = 50;

        public const string PalletIDSequenceTelemetryEnabledName = "PalletIDSequenceTelemetryEnabled";
        public const string StorageViewInitialNodeIndexMessageName = "StorageViewInitialNodeIndex";
        public const string AdminStorageViewName = "AdminStorageView";


        // === THESE ARRAYS APPLY TO LOADS OF 54 PALLETS ====================================================
        //!!! STILL NEED TO BE DONE!
        public static readonly int[] PickSearchOrder = new int[]
        {
            00, 59, 02,
            57, 30, 27,
            32, 29, 58,
            01, 28, 31,

            03, 56, 05,
            54, 33, 24,
            35, 26, 55,
            04, 25, 34,

            06, 53, 08,
            51, 36, 21,
            38, 23, 52,
            07, 22, 37,

            09, 50, 11,
            48, 39, 18,
            41, 20, 49,
            10, 19, 40,

            12, 47, 14,
            45, 42, 15,
            44, 17, 46,
            13, 16, 43
        };

        public const int AfterLast = -1;
        public static readonly int[] NextInLaneLoadIndex = new int[]
        {
            //Lower Level
            03, 04, 05,
            06, 07, 08,
            09, 10, 11,
            12, 13, 14,
            AfterLast, AfterLast, AfterLast,

            AfterLast, AfterLast, AfterLast,
            15, 16, 17,
            18, 19, 20,
            21, 22, 23,
            24, 25, 26,

            //Upper Level
            33, 34, 35,
            36, 37, 38,
            39, 40, 41,
            42, 43, 44,
            AfterLast, AfterLast, AfterLast,

            AfterLast, AfterLast, AfterLast,
            45, 46, 47,
            48, 49, 50,
            51, 52, 53,
            54, 55, 56
        };

        public const int BeforeFirst = -1;
        public static readonly int[] _previousInLaneLoadIndex = new int[]
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
            27, 28, 29,
            BeforeFirst, BeforeFirst, BeforeFirst,

            // Upper
            BeforeFirst, BeforeFirst, BeforeFirst,
            30, 31, 32,
            33, 34, 35,
            36, 37, 38,
            39, 40, 41,

            48, 49, 50,
            51, 52, 53,
            54, 55, 56,
            57, 58, 59,
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
