using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using DacQuest.DFX.Core.Services;
using DacQuest.DFX.Core.Configuration;
using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.Messaging;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Collections;
using Mss.Common;
using Mss.Data;
using System.Data.SqlClient;

// using Msd.Dacm.Data.LoadArchive;
// using DevExpress.XtraReports.UI;
// using DevExpress.XtraPrinting;
// using Msd.Dacm.Data.Pocos;
// using Msd.Dacm.Data.Repositories;

namespace Mss.Services
{
    public class ShortageCalculatorService : XService
    {
        public static readonly int ForceCalculateShortages = XService.FirstCustomCommand;
        //public static readonly int MyCustomCommand2 = XService.FirstCustomCommand + 1;

        private Storage _storage;
        private AssignmentPit _assignmentPit;
        private LowerPit _lowerPit;
        private UpperPit _upperPit;
        private SystemSettings _systemSettings;
        private Broadcast _broadcast;
        private HoldCodes _holdCodes;
        private LowerRecircBuffer _lowerRecircBuffer;
        private UpperRecircBuffer _upperRecircBuffer;
        private SlugA _slugA;
        private SlugB _slugB;
        private DataLayer _dataLayer;

        protected override bool OnStart()
        {
            // There is no need to call SetServiceStatus() when overriding

            _dataLayer = DataLayer.Create(
                out _storage,
                out _assignmentPit,
                out _lowerPit,
                out _upperPit,
                out _systemSettings,
                out _broadcast,
                out _holdCodes,
                out _lowerRecircBuffer,
                out _upperRecircBuffer,
                out _slugA,
                out _slugB);

            _upperPit.DataItemChanged += _Collection_DataItemChanged;
            _lowerPit.DataItemChanged += _Collection_DataItemChanged;
            _storage.DataItemChanged += _Collection_DataItemChanged;
            _storage.Touched += _Collection_Touched;

            SetExtendedServiceStatus("Waiting...");

            RegisterCustomCommand(ForceCalculateShortages, "Force Shortage Calculation");

            _CalculateShortages();

            return true;
        }

        protected override int OnStop()
        {
            // There is no need to call SetServiceStatus() when overriding

            if (_storage != null)
            {
                _storage.DataItemChanged -= _Collection_DataItemChanged;
                _storage.Touched -= _Collection_Touched;
            }
            if (_upperPit != null)
            {
                _upperPit.DataItemChanged -= _Collection_DataItemChanged;
            }
            if (_lowerPit != null)
            {
                _lowerPit.DataItemChanged -= _Collection_DataItemChanged;
            }

            _dataLayer.Dispose();
            _dataLayer = null;

            return ExitCode;
        }

        protected override void AutoSubscribe()
        {
            Subscribe(
                Constant.CalculateShortagesMessageTopicName,
                _CalculateShortages_OnMessage,
                XMessageScopes.All);
        }

        private void _CalculateShortages_OnMessage(
            object sender,
            XMessageEventArgs e)
        {
            _CalculateShortages();
        }


        private void _Collection_DataItemChanged(object sender, XDataItemChangedEventArgs e)
        {
            _CalculateShortages();
        }

        private void _Collection_Touched(object sender, EventArgs e)
        {
            _CalculateShortages();
        }

        private void _CalculateShortages()
        {
            _dataLayer.CalculateShortages();
        }

        //public override bool CanStop => true;

        //public override bool CanShutdown => true;

        //public override bool CanPauseAndContinue => false;

        //public override bool CanHandlePowerEvent => false;

        public override bool CanHandleCustomCommand => GetCustomCommands().Count > 0;

        //protected override bool OnPause()
        //{
        //    return true;
        //}

        //protected override bool OnContinue()
        //{
        //    return true;
        //}

        protected override void OnCustomCommand(int customCommand)
        {
            // Remember to call SetServiceStatus() as necessary when overriding
            if (customCommand == ForceCalculateShortages)
            {
                _CalculateShortages();
            }
        }

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


        //=======================================================================================================
        //=======================================================================================================
        //=======================================================================================================
        //=======================================================================================================
        //=======================================================================================================
        //=======================================================================================================



//         private long _loadManifestTimerID = 0;
//         private void _StartLoadManifestTimer()
//         {
//             _StopLoadManifestTimer();
//             _loadManifestTimerID = StartTimer(
//                 _LoadManifestTimer_Expired,
//                 1000 * 30,  //  30 seconds
//                 null);
//         }
//
//         private void _StopLoadManifestTimer()
//         {
//             if (_loadManifestTimerID != 0)
//             {
//                 StopTimer(_loadManifestTimerID);
//                 _loadManifestTimerID = 0;
//             }
//         }
//
//         private void _LoadManifestTimer_Expired(XTimerEventArgs e)
//         {
//             // Fetch record from db
//             // Load Archive Data
//             // Print Shipper Report
//             // Delete record
//
//             _CheckLoadPrintQueue();
//
//             _StartLoadManifestTimer();
//         }
//
//         private void _CheckLoadPrintQueue()
//         {
//             if (!_FetchLoadPrintQueueNotification(out LoadPrintQueueNotification notification))
//             {
//                 return;
//             }
//
//             if (!_FetchArchive(notification.LoadNumber, out LoadArchive loadArchive, out string error))
//             {
//                 XSystemEvent.Publish(
//                     "Fetch Shipper Archive",
//                     XSystemEventLevel.Error,
//                     error);
//                 return;
//             }
//             try
//             {
//                 _PrintShipperReport(loadArchive);
//                 _DeletePrintShipperNotification(notification, out error);
//             }
//             catch (Exception x)
//             {
//                 string context = "Print Shipper Report";
//                 XSystemEvent.Publish(
//                     context,
//                     XSystemEventLevel.Error,
//                     $"Exception thrown while printing Shipper Report for Load Number {notification.LoadNumber}");
//                 x.PublishSystemEvent(context);
//             }
//         }
//
//         private bool _FetchArchive(
//             int loadNumber,
//             out LoadArchive loadArchive,
//             out string error)
//         {
//             loadArchive = null;
//             try
//             {
//                 string connectionString = XConfiguration.GetConnectionString(Constant.ArchiveConnectionStringName);
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     LoadArchiveRepository loadRepository = new LoadArchiveRepository(connection);
//                     LoadItemArchiveRepository loadItemRepository = new LoadItemArchiveRepository(connection);
//                     LoadBroadcastArchiveRepository broadcastRepository = new LoadBroadcastArchiveRepository(connection);
//                     LoadPalletArchiveRepository palletRepository = new LoadPalletArchiveRepository(connection);
//                     LoadPallet3rdArchiveRepository pallet3rdRepository = new LoadPallet3rdArchiveRepository(connection);
//
//                     loadArchive = loadRepository
//                         .FindAll(l => l.LoadNumber == loadNumber)
//                         .FirstOrDefault();
//
//                     if (loadArchive == null)
//                     {
//                         error = $"Failed to find Load Number {loadNumber} in Load Archive";
//                         return false;
//                     }
//
//                     int loadArchiveID = loadArchive.ID;
//                     IEnumerable<LoadItemArchive> loadItems = loadItemRepository.FindAll(li => li.LoadArchiveID == loadArchiveID);
//                     foreach (LoadItemArchive itemArchive in loadItems)
//                     {
//                         itemArchive.Broadcast = broadcastRepository.Find(b => b.LoadItemArchiveID == itemArchive.ID);
//                         itemArchive.Pallet = palletRepository.Find(p => p.LoadItemArchiveID == itemArchive.ID);
//                         itemArchive.Pallet3rd = pallet3rdRepository.Find(p => p.LoadItemArchiveID == itemArchive.ID);
//                     }
//                     loadArchive.LoadItems = loadItems.OrderBy(l => l.Broadcast.InternalSequenceNumber).ToList();
//                 }
//                 error = string.Empty;
//                 return true;
//             }
//             catch (Exception x)
//             {
//                 error = $"Exception thrown while fetching Load Archive for Load Number {loadNumber}. See System Events.";
//                 x.PublishSystemEvent("Fetch Shipper Archive");
//                 return false;
//             }
//         }
//
//         private void _PrintShipperReport(LoadArchive loadArchive)
//         {
//             //             if (_parameters.ShipperReportCopies > 0)
//             //             {
//             ShipperReport report = new ShipperReport(loadArchive);
//             report.PrintingSystem.StartPrint += _ShipperReport_StartPrint;
//             //                 report.Print("DacQuest: HP LaserJet Pro P1109w");
//             report.Print();
//             report.PrintingSystem.StartPrint -= _ShipperReport_StartPrint;
//             //             }
//         }
//
//         private void _ShipperReport_StartPrint(object sender, PrintDocumentEventArgs eventArgs)
//         {
//             //             eventArgs.PrintDocument.PrinterSettings.Copies = _parameters.ShipperReportCopies;
//             eventArgs.PrintDocument.PrinterSettings.Copies = 1;
//             //             if (_systemSettings.UseSecondaryShipperReportPrinter)
//             //             {
//             //                 eventArgs.PrintDocument.PrinterSettings.PrinterName = _parameters.SecondaryShipperReportPrinterName;
//             //             }
//             //             else
//             //             {
//             //                 eventArgs.PrintDocument.PrinterSettings.PrinterName = _parameters.ShipperReportPrinterName;
//             eventArgs.PrintDocument.PrinterSettings.PrinterName = "DacQuest: Brother MFC-L2750DW";
//             //             }
//         }
//
//         private bool _FetchLoadPrintQueueNotification(out LoadPrintQueueNotification notification)
//         {
//             try
//             {
//                 IEnumerable<LoadPrintQueueNotification> notifications = null;
//                 string connectionString = XConfiguration.GetConnectionString(Constant.ArchiveConnectionStringName);
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     LoadPrintQueueRepository repository = new LoadPrintQueueRepository(connection);
//                     notifications = repository.FindAll().OrderBy(n => n.InsertedOn);
//                     if (notifications == null || notifications.Count() == 0)
//                     {
//                         notification = null;
//                         return false;
//                     }
//                 }
//                 notification = notifications.First();
//                 return true;
//             }
//             catch (Exception x)
//             {
//                 notification = null;
//                 XSystemEvent.Publish(
//                     "Fetch Print Shipper Notification",
//                     XSystemEventLevel.Error,
//                     "An exception occurred while accessing LoadPrintQueue table.");
//                 x.PublishSystemEvent("Fetch Print Shipper Notification");
//                 return false;
//             }
//         }
//
//         private void _DeletePrintShipperNotification(LoadPrintQueueNotification notification, out string error)
//         {
//             error = "";
//             try
//             {
//                 string sql = $"DELETE FROM LoadPrintQueue WHERE ID={notification.ID}";
//                 string connectionString = XConfiguration.GetConnectionString(Constant.ArchiveConnectionStringName);
//                 using (SqlConnection connection = new SqlConnection(connectionString))
//                 {
//                     connection.Open();
//                     using (SqlCommand command = new SqlCommand(sql, connection))
//                     {
//                         command.ExecuteNonQuery();
//                     }
//                 }
//             }
//             catch (Exception x)
//             {
//                 x.PublishSystemEvent("Delete Print Shipper Notification");
//                 //                 return false;
//             }
//         }


    }
}
