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




        public const int PalletIDLength = 30;




        //Storage
        //public const string StackPalletIDValidator = "StackPalletIDValidator";
        public const string StackSku = "STACK";
        public const string StorageName = "Storage";
        public const int MaxCranes = 4;
        public const int CraneSides = 2;
        public const int MaxHorizontal = 22;
        public const int MaxVertical = 6;
        public const int StorageSize = 1056;
        public const int ShortVinLength = 8;

        public const Int32 LoadSize = 54;

        public const Int32 CommandMoveToSlugLane1 = 1;
        public const Int32 CommandMoveToSlugLane2 = 2;
        public const Int32 CommandMoveToSlugLane3 = 3;




        public const string AutoDetectedDuplicatePalletIDComment = "Auto-detected duplicate Pallet ID";
        public const int DuplicatePalletIDHoldCode = 256;
        public const string AutoDetectedDuplicateJobIDComment = "Auto-detected duplicate Job ID";
        public const int DuplicateJobIDHoldCode = 257;

        public const string Row1PalletIDValidatorName = "Row1PalletIDValidator";
        public const string Row2PalletIDValidatorName = "Row2PalletIDValidator";
        public static readonly string NoPalletID = string.Empty;
        public const int NoJobID = 0;
        public const string UpperPitName = "Upper Pit";
        public const string LowerPitName = "Lower Pit";



        public const int StoragePerCrane = MaxHorizontal * MaxVertical * CraneSides;



        public const int CsnLength = 10;
        public const int MaxSkuLength = 20;
        public const int CommentLength = 50;

        public const string PalletIDSequenceTelemetryEnabledName = "PalletIDSequenceTelemetryEnabled";
        public const string StorageViewInitialNodeIndexMessageName = "StorageViewInitialNodeIndex";
        public const string AdminStorageViewName = "AdminStorageView";




    }
}
