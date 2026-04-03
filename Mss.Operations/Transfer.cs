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

        protected string LoadALevelCompletedName = "Load A Level Completed";
        protected bool LoadALevelCompleted
        {
            get => GetVariable<bool>(LoadALevelCompletedName);
            set => SetVariable(LoadALevelCompletedName, value);
        }

        protected string LoadBLevelCompletedName = "Load A Level Completed";
        protected bool LoadBLevelCompleted
        {
            get => GetVariable<bool>(LoadBLevelCompletedName);
            set => SetVariable(LoadBLevelCompletedName, value);
        }

        protected Levels Level => _parameters.Level;

        protected override void ProcessParameters(XConfigurationParameterSet parameters)
        {
            _parameters = parameters as TransferParameterSetWrapper;
            LoadALevelCompletedName = $"Load A {Level.ToText()} Level Completed";
            LoadBLevelCompletedName = $"Load B {Level.ToText()} Level Completed";
        }

        protected override void DoStart()
        {
            base.DoStart();

            SlugA.DataItemChanged += _LoadA_DataItemChanged;
            SlugA.Touched += _LoadA_CollectionTouched;
            SlugB.DataItemChanged += _LoadB_DataItemChanged;
            SlugB.Touched += _LoadB_CollectionTouched;

            if (Level == Levels.Upper)
            {
                StartPlcTagCapture(
                    Constant.LoadAUpperLevelCompletedRoleName,
                    _LevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);

                StartPlcTagCapture(
                    Constant.LoadBUpperLevelCompletedRoleName,
                    _LevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);
            }
            else if (Level == Levels.Lower)
            {
                StartTagDataCapture(
                    Constant.PlcRoleName,
                    Constant.LoadALowerLevelCompletedRoleName,
                    _LevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);

                StartTagDataCapture(
                    Constant.PlcRoleName,
                    Constant.LoadBLowerLevelCompletedRoleName,
                    _LevelCompleted_TagChanged,
                    XTagDataCaptureUpdateMode.OnChange);
            }
        }

        protected override void DoStop()
        {
            if (SlugA != null)
            {
                SlugA.DataItemChanged -= _LoadA_DataItemChanged;
                SlugA.Touched -= _LoadA_CollectionTouched;
            }
            if (SlugB != null)
            {
                SlugB.DataItemChanged -= _LoadB_DataItemChanged;
                SlugB.Touched -= _LoadB_CollectionTouched;
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
                case Constant.LoadAUpperLevelCompletedRoleName:
                case Constant.LoadALowerLevelCompletedRoleName:
                    LoadALevelCompleted = levelCompleted;
                    break;
                case Constant.LoadBUpperLevelCompletedRoleName:
                case Constant.LoadBLowerLevelCompletedRoleName:
                    LoadBLevelCompleted = levelCompleted;
                    break;
            }
        }

        private void _LoadA_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _ProcessLoadChangeEvent(SlugLetter.A);
        }

        private void _LoadA_CollectionTouched(object sender, EventArgs e)
        {
            _ProcessLoadChangeEvent(SlugLetter.A);
        }

        private void _LoadB_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _ProcessLoadChangeEvent(SlugLetter.B);
        }

        private void _LoadB_CollectionTouched(object sender, EventArgs e)
        {
            _ProcessLoadChangeEvent(SlugLetter.B);
        }

        private void _ProcessLoadChangeEvent(SlugLetter slugLetter)
        {
            if (DataLayer.IsLoadLevelDone(
                slugLetter,
                Level))
            {
                string levelCompletedTagRoleName = slugLetter == SlugLetter.A
                    ? Level == Levels.Upper
                        ? Constant.LoadAUpperLevelCompletedRoleName
                        : Constant.LoadALowerLevelCompletedRoleName
                    : Level == Levels.Upper
                        ? Constant.LoadBUpperLevelCompletedRoleName
                        : Constant.LoadBLowerLevelCompletedRoleName;

                WritePlc(
                    levelCompletedTagRoleName,
                    true);
            }
        }

        //==================================================================================

        #region ProcessPallet State

        protected override bool DoProcessPallet(out int moveCommand, out string extendedState)
        {
            moveCommand = Constant.NoMoveCommand;
            if (DataLayer.ProcessPalletAtTransfer(
                _parameters.Level,
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
            if (CachedMoveCommand != Constant.TFFinalPurgeMoveCommand
                && CachedMoveCommand != Constant.TFStackMoveCommand)
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
