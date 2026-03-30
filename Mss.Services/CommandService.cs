using DacQuest.DFX.Core;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Mss.Common;
using Mss.Collections;
using Mss.Data;

namespace Mss.Services
{
    public class CommandService : XService
    {
        //public static readonly int MyCustomCommand1 = XService.FirstCustomCommand;
        //public static readonly int MyCustomCommand2 = XService.FirstCustomCommand + 1;

        private Storage _storage;
        private LowerPit _lowerPit;
        private UpperPit _upperPit;
        private SystemSettings _systemSettings;
        private Broadcast _broadcast;
        private HoldCodes _holdCodes;
        private LowerRecirc _lowerRecirc;
        private UpperRecirc _upperRecirc;
        private LoadA _loadA;
        private LoadB _loadB;
        private DataLayer _dataLayer;

        protected override bool OnStart()
        {
            // There is no need to call SetServiceStatus() when overriding

            _dataLayer = DataLayer.Factory(
                out _storage,
                out _lowerPit,
                out _upperPit,
                out _systemSettings,
                out _broadcast,
                out _holdCodes,
                out _lowerRecirc,
                out _upperRecirc,
                out _loadA,
                out _loadB);

            return true;
        }

        protected override int OnStop()
        {
            // There is no need to call SetServiceStatus() when overriding
            _dataLayer.Dispose();
            _dataLayer = null;

            return ExitCode;
        }

        protected override bool ProcessParameters(XConfigurationParameterSet parameters)
        {
            return true;
        }

        private void _BulkPalletStatusConversion_OnMessage(
             object sender,
             XMessageEventArgs e)
        {
            BulkPalletStatusConversionMessageData md = (BulkPalletStatusConversionMessageData)e.MessageData;
            SetExtendedServiceStatus("Doing Bulk Pallet Status Conversion");
            _dataLayer.BulkConvertPalletStatus(md, out List<BinItem> failedConversionItems);
            md.PublishResponse(new BulkPalletStatusConversionMessageData(failedConversionItems));
            SetExtendedServiceStatus("Waiting...");
        }

        protected override void AutoSubscribe()
        {
            // base class MUST be called if this override is used
            base.AutoSubscribe();

            Subscribe(
                BulkPalletStatusConversionMessageData.BulkPalletStatusConversionMessageTopic,
                _BulkPalletStatusConversion_OnMessage,
                XMessageScopes.All);

        }

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
