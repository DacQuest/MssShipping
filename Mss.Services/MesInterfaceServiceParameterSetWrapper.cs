using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DacQuest.DFX.Core.Configuration;

namespace Mss.Services
{
    [Serializable]
    public class MesInterfaceServiceParameterSetWrapper : XConfigurationParameterSet
    {

        private int _broadcastPollingPeriodSeconds = 0;
        [XConfigurationProperty(
            "MesDataServiceParameterSetWrapper.BroadcastPollingPeriodSeconds",
            false,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt,
            ValidationRegexString = @"^\d+$", // 0 and up
            DefaultValue = "0")]
        public int BroadcastPollingPeriodSeconds => _broadcastPollingPeriodSeconds;

        private bool _fetchBroadcastOnStartUp = false;
        [XConfigurationProperty(
            "",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool FetchBroadcastOnStartUp => _fetchBroadcastOnStartUp;

        private int _holdCodesPollingPeriodSeconds = 0;
        [XConfigurationProperty(
            "MesDataServiceParameterSetWrapper.HoldCodesPollingPeriodSeconds",
            false,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt,
            ValidationRegexString = @"^\d+$", // 0 and up
            DefaultValue = "0")]
        public int HoldCodesPollingPeriodSeconds => _holdCodesPollingPeriodSeconds;

        private bool _fetchHoldCodesOnStartUp = false;
        [XConfigurationProperty(
            "",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool FetchHoldCodesOnStartUp => _fetchHoldCodesOnStartUp;

        private int _statusChangePollingPeriodSeconds = 0;
        [XConfigurationProperty(
            "MesDataServiceParameterSetWrapper.StatusChangePollingPeriodSeconds",
            false,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt,
            ValidationRegexString = @"^\d+$", // 0 and up
            DefaultValue = "0")]
        public int StatusChangePollingPeriodSeconds => _statusChangePollingPeriodSeconds;

        private bool _processPalletStatusChangesOnStartUp = false;
        [XConfigurationProperty(
            "",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool ProcessPalletStatusChangesOnStartUp => _processPalletStatusChangesOnStartUp;

        private int _broadcastCoverageTimerPeriodSeconds = 60;
        [XConfigurationProperty(
            "MesDataServiceParameterSetWrapper.BroadcastCoverageTimerPeriodSeconds",
            false,
            ValidDirectives = XConfigurationDirectives.AliasAndEncrypt,
            ValidationRegexString = @"^\d+$", // 0 and up
            DefaultValue = "60")]
        public int BroadcastCoverageTimerPeriodSeconds => _broadcastCoverageTimerPeriodSeconds;


        private bool _updateBroadcastCoverageOnStartUp = false;
        [XConfigurationProperty(
            "",
            false,
            DefaultValue = "false",
            PickListValues = "true,false")]
        public bool UpdateBroadcastCoverageOnStartUp => _updateBroadcastCoverageOnStartUp;

    }
}
