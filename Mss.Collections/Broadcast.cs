using DacQuest.DFX.Core;
using DacQuest.DFX.Core.DataItems;
using DacQuest.DFX.Core.DataItems.Collections;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Core.SystemEvents;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Mss.Collections
{
    public class Broadcast : XSharedDictionary<string, BroadcastItem>
    {
        protected override void RegisterCustomQueries()
        {
            RegisterCustomQuery(Constant.CurrentBroadcastQuery, _CurrentBroadcastQuery);
        }

        private bool _CurrentBroadcastQuery(ref List<XDataItem> list)
        {
            if (!Open(
                Constant.SystemSettingsName,
                out SystemSettings systemSettings))
            {
                XSystemEvent.Publish(
                    nameof (_CurrentBroadcastQuery),
                    XSystemEventLevel.Error,
                    $"Failed to open the {Constant.SystemSettingsName} collection");
                return false;
            }
            _ = systemSettings.Lock();
            string lastCsnReleased = systemSettings.LastCsnReleased;
            int largestRotationReceived = systemSettings.LargestRotationReceived;
            systemSettings.Unlock();
            systemSettings.Close();

//             list = null;
//             _ = Lock();
//             try
//             {
//                 list = GetCurrentBroadcastItems(
//                     lastCsnReleased,
//                     largestRotationReceived).ToList<XDataItem>();
//             }
//             finally
//             {
//                 Unlock();
//             }
            list =  GetCurrentBroadcastItems(
                lastCsnReleased,
                largestRotationReceived).ToList<XDataItem>();
            return true;
        }

        public List<BroadcastItem> GetCurrentBroadcastItems(
            string lastCsnReleased,
            int largestRotationReceived)
        {
            _ = Lock();
            try
            {
                _InsertMissingBroadcasts(
                    lastCsnReleased,
                    largestRotationReceived);
                return _GetCurrentBroadcastItems(
                    lastCsnReleased,
                    largestRotationReceived);
            }
            finally
            {
                Unlock();
            }
        }

        private void _InsertMissingBroadcasts(
            string lastCsnReleased,
            int largestRotationReceived)
        {
            IEnumerable<BroadcastItem> broadcastItems = _GetCurrentBroadcastItems(
                lastCsnReleased,
                largestRotationReceived).OrderBy(b => b.Csn);

            if (!broadcastItems.Any())
            {
                return;
            }

            int currentRotation = BroadcastItem.SequenceFromCsn(lastCsnReleased);
            if (lastCsnReleased.Right(1) == Constant.VehicleRow2CsnSuffix)
            {
                string matchingRow1Csn = BroadcastItem.MakeCsn(currentRotation, Constant.VehicleRow1CsnSuffix);
                BroadcastItem matchingRow1BroadcastItem = broadcastItems.SingleOrDefault(b => b.Csn == matchingRow1Csn);
                if (matchingRow1BroadcastItem == null)
                {
                    BroadcastItem missingBroadcastItem = BroadcastItem.CreateMissingBroadcastItem(currentRotation);
                    this[missingBroadcastItem.Csn] = missingBroadcastItem;
                }
            }

            BroadcastItem broadcastItem;
            currentRotation++;
            while (currentRotation <= largestRotationReceived)
            {
                IEnumerable<BroadcastItem> currentItems = broadcastItems
                    .Where(b => b.Sequence == currentRotation)
                    .OrderBy(b => b.Csn);

                int count = currentItems.Count();
                if (count == 2)
                {
                    if (!currentItems.First().Csn.EndsWith(Constant.VehicleRow2CsnSuffix)
                        || !currentItems.Last().Csn.EndsWith(Constant.VehicleRow1CsnSuffix))
                    {
                        foreach (BroadcastItem currentItem in currentItems)
                        {
                            _ = Remove(currentItem.Csn);
                        }
                        BroadcastItem missingBroadcastItem = BroadcastItem.CreateMissingBroadcastItem(currentRotation);
                        this[missingBroadcastItem.Csn] = missingBroadcastItem;
                    }
                }
                else if (count == 1)
                {
                    broadcastItem = currentItems.First();
                    int vehicleRowCount = broadcastItem.VehicleRowCount;
                    if (vehicleRowCount != 1
                        || !broadcastItem.Csn.EndsWith(Constant.VehicleRow1CsnSuffix))
                    {
                        _ = Remove(broadcastItem.Csn);
                        BroadcastItem missingBroadcastItem = BroadcastItem.CreateMissingBroadcastItem(currentRotation);
                        this[missingBroadcastItem.Csn] = missingBroadcastItem;
                    }
                }
                else if (count == 0)
                {
                    BroadcastItem missingBroadcastItem = BroadcastItem.CreateMissingBroadcastItem(currentRotation);
                    this[missingBroadcastItem.Csn] = missingBroadcastItem;
                }
                else //too many
                {
                    foreach (BroadcastItem currentItem in currentItems)
                    {
                        _ = Remove(currentItem.Csn);
                    }
                    BroadcastItem missingBroadcastItem = BroadcastItem.CreateMissingBroadcastItem(currentRotation);
                    this[missingBroadcastItem.Csn] = missingBroadcastItem;
                }
                currentRotation++;
            }
        }

//         private void _HandleMissingMatchingRow1Csn(string csn, string matchingRow1Csn)
//         {
//             _ = Remove(csn);
// 
//             int rotation = BroadcastItem.RotationFromCsn(csn);
//             BroadcastItem missingBroadcastItem = BroadcastItem.CreateMissingBroadcastItem(rotation);
//             this[missingBroadcastItem.Csn] = missingBroadcastItem;
//         }

        private List<BroadcastItem> _GetCurrentBroadcastItems(
            string lastCsnReleased,
            int largestRotationReceived)
        {
            _ = Lock();
            try
            {
                IEnumerable<BroadcastItem> broadcastItems = Values
                    .Where(b =>
                    {
                        return b.Csn.IsGreaterThan(lastCsnReleased, true)
                            && b.Sequence <= largestRotationReceived;
                    })
                    .OrderBy(b => b.Csn);
                return broadcastItems.ToList();
            }
            finally
            {
                Unlock();
            }
        }

        public List<BroadcastItem> GetReleasableItems(
            string lastCsnReleased,
            int largestBroadcastReceived)
        {
            return _GetReleasableItems(
                lastCsnReleased,
                largestBroadcastReceived);
        }

        private List<BroadcastItem> _GetReleasableItems(
            string lastCsnReleased,
            int largestBroadcastReceived)
        {
            _ = Lock();
            try
            {
                List<BroadcastItem> currentItems = GetCurrentBroadcastItems(
                    lastCsnReleased,
                    largestBroadcastReceived);
                List<BroadcastItem> releasableItems = new List<BroadcastItem>();
                foreach (BroadcastItem item in currentItems)
                {
                    BroadcastStatus status = item.Status;
                    if (status == BroadcastStatus.OK)
                    {
                        releasableItems.Add(item);
                    }
                    else if (status != BroadcastStatus.Skip)
                    {
                        break;
                    }
                }
                return releasableItems;
            }
            finally
            {
                Unlock();
            }
        }

        public int ReleasableBroadcastItemCount(
            string lastCsnReleased,
            int largestBroadcastReceived)
        {
            return _GetReleasableItems(
                lastCsnReleased,
                largestBroadcastReceived).Count;
        }

        public void PurgeOldBroadcast()
        {
            _ = Lock();
//             bool oldSetting = InhibitChangeNotifications;
//             InhibitChangeNotifications = true;
//             bool touch = false;
            try
            {
                int activeCount = this.Where(b => b.Value.Active).Count();
                if (activeCount >= (int)(ItemCount * 0.94F)) // allows for at least 108 records
                {
                    int countToPurge = activeCount - (int)(ItemCount * 0.8F);
                    IEnumerable<BroadcastItem> listToPurge = Values
                        .Where(b =>
                        {
                            return b.Status == BroadcastStatus.Shipped
                                || b.Status == BroadcastStatus.Skip;
                        })
                        .OrderBy(b => b.ReceivedOn)
                        .Take(countToPurge);
                    foreach (BroadcastItem broadcastItem in listToPurge)
                    {
                        _ = Remove(broadcastItem.Csn, true);
//                         touch = true;
                    }
                }
            }
            catch (Exception x)
            {
                x.PublishSystemEvent(nameof (PurgeOldBroadcast));
            }
            finally
            {
//                 InhibitChangeNotifications = oldSetting;
//                 if (touch)
//                 {
//                     Touch();
//                 }
                Unlock();
            }
        }

    }
}
