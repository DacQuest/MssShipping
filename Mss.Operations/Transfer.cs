using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.SystemEvents;
using DacQuest.DFX.Devices.Tags;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Mss.Operations
{
    public class Transfer : LineOperationBase
    {
        private TransferParameterSetWrapper _parameters;

        public override OperationCode OperationCode
        {
            get
            {
                switch (Level)
                {
                    case Levels.Upper:
                        return OperationCode.UT;
                    case Levels.Lower:
                        return OperationCode.LT;
                    default:
                        return OperationCode.Unknown;
                }
            }
        }

//         protected override void RegisterCustomStates()
//         {
//             RegisterState(AwaitingTrailerLoadState, "Awaiting Trailer Load", AwaitingTrailerLoadStateHandler);
//             base.RegisterCustomStates();
//         }

        protected string SlugALevelCompletedName = "Slug A Level Completed";
        protected bool SlugALevelCompleted
        {
            get => GetVariable<bool>(SlugALevelCompletedName);
            set => SetVariable(SlugALevelCompletedName, value);
        }

        protected string SlugBLevelCompletedName = "Slug B Level Completed";
        protected bool SlugBLevelCompleted
        {
            get => GetVariable<bool>(SlugBLevelCompletedName);
            set => SetVariable(SlugBLevelCompletedName, value);
        }

        protected Levels Level => _parameters.Level;

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = parameters as TransferParameterSetWrapper;
            SlugALevelCompletedName = $"Slug A {Level.ToText()} Level Completed";
            SlugBLevelCompletedName = $"Slug B {Level.ToText()} Level Completed";
        }

        protected override void DoStart()
        {
            base.DoStart();

            SlugA.DataItemChanged += _SlugA_DataItemChanged;
            SlugA.Touched += _SlugA_CollectionTouched;
            SlugB.DataItemChanged += _SlugB_DataItemChanged;
            SlugB.Touched += _SlugB_CollectionTouched;

            if (Level == Levels.Upper)
            {
                StartPlcTagCapture(
                    Constant.SlugAUpperLevelCompletedRoleName,
                    _LevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);

                StartPlcTagCapture(
                    Constant.SlugBUpperLevelCompletedRoleName,
                    _LevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);
            }
            else if (Level == Levels.Lower)
            {
                StartTagDataCapture(
                    Constant.PlcRoleName,
                    Constant.SlugALowerLevelCompletedRoleName,
                    _LevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);

                StartTagDataCapture(
                    Constant.PlcRoleName,
                    Constant.SlugBLowerLevelCompletedRoleName,
                    _LevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);
            }
        }

        protected override void DoStop()
        {
            if (SlugA != null)
            {
                SlugA.DataItemChanged -= _SlugA_DataItemChanged;
                SlugA.Touched -= _SlugA_CollectionTouched;
            }
            if (SlugB != null)
            {
                SlugB.DataItemChanged -= _SlugB_DataItemChanged;
                SlugB.Touched -= _SlugB_CollectionTouched;
            }
            base.DoStop();
        }

        private void _LevelCompleted_TagChanged(object sender, XTagDataEventArgs e)
        {
            XTagData tagData = e.TagData;
            if (!tagData.TryGetTagValue(out bool levelCompleted))
            {
                return;
            }
            switch (tagData.ConfigurationItem.RoleName)
            {
                case Constant.SlugAUpperLevelCompletedRoleName:
                case Constant.SlugALowerLevelCompletedRoleName:
                    SlugALevelCompleted = levelCompleted;
                    break;
                case Constant.SlugBUpperLevelCompletedRoleName:
                case Constant.SlugBLowerLevelCompletedRoleName:
                    SlugBLevelCompleted = levelCompleted;
                    break;
            }
        }

        private void _SlugA_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _ProcessSlugChangeEvent(SlugLetter.A);
        }

        private void _SlugA_CollectionTouched(object sender, EventArgs e)
        {
            _ProcessSlugChangeEvent(SlugLetter.A);
        }

        private void _SlugB_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _ProcessSlugChangeEvent(SlugLetter.B);
        }

        private void _SlugB_CollectionTouched(object sender, EventArgs e)
        {
            _ProcessSlugChangeEvent(SlugLetter.B);
        }

        private void _ProcessSlugChangeEvent(SlugLetter slugLetter)
        {
            if (DataLayer.IsLoadLevelDone(
                slugLetter,
                Level))
            {
                string levelCompletedTagRoleName = slugLetter == SlugLetter.A
                    ? Level == Levels.Upper
                        ? Constant.SlugAUpperLevelCompletedRoleName
                        : Constant.SlugALowerLevelCompletedRoleName
                    : Level == Levels.Upper
                        ? Constant.SlugBUpperLevelCompletedRoleName
                        : Constant.SlugBLowerLevelCompletedRoleName;

                WritePlc(
                    levelCompletedTagRoleName,
                    true);
            }
        }

        //==================================================================================

        #region ProcessPallet State

        protected override bool DoProcessPallet(out int moveCommand, out string extendedState)
        {
            if (DataLayer.ProcessPalletAtTransfer(
                Level,
                PalletID,
                out PalletItem palletItem,
                out moveCommand,
                out extendedState,
                out string fault))
            {
                CurrentPallet = palletItem;
                return true;
            }
            if (!string.IsNullOrWhiteSpace(fault))
            {
                SetOperationFaulted(fault);
                return false;
            }
            return false;
        }


        #endregion

        //==================================================================================

        #region AwaitingMoveCompletedState

        protected override bool DoMoveCompleted()
        {
            DataLayer.RemovePitPallet(CurrentPallet.PalletID);
            if (CachedMoveCommand != Constant.TransferFinalPurgeMoveCommand
                && CachedMoveCommand != Constant.TransferStackMoveCommand)
            {
                if (!DataLayer.TrySetDone(
                    CurrentPallet.PalletID,
                    out string fault))
                {
                    if (!string.IsNullOrWhiteSpace(fault))
                    {
                        SetOperationFaulted(fault);
                    }
                    return false;
                }
            }
            return true;
        }

        #endregion

        //==================================================================================
        //         protected override void DoDispose()
        //         {
        //             base.DoDispose();
        //         }
        //==================================================================================

        //         #region AwaitingTrailerLoadState
        // 
        //         protected readonly string AwaitingTrailerLoadState = "AwaitingTrailerLoad";
        // 
        //         protected virtual void AwaitingTrailerLoadStateHandler()
        //         {
        //         }
        // 
        //         #endregion

    }
}
