using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using Mss.Collections;
using Mss.Common;
using Mss.Data;

namespace Mss.Data.LoadArchive
{
    [Table("LoadItemArchive")]
    public class LoadItemArchive
    {
        [Key, Identity]
        public int ID { get; set; }
        public int LoadArchiveID { get; set; }
        public int LoadIndex { get; set; }
        public LoadItemStatus Status { get; set; }
        public CraneNumber CraneNumber { get; set; }
//        public bool InsertEmpty { get; set; }

        [LeftJoin("PalletArchive", nameof(ID), "LoadItemArchiveID")]
        public LoadPalletArchive Pallet { get; set; }

        [LeftJoin("BroadcastArchive", nameof(ID), "LoadItemArchiveID")]
        public LoadBroadcastArchive Broadcast { get; set; }

    }
}
