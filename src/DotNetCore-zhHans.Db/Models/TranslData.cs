using System;
using System.ComponentModel.DataAnnotations.Schema;
using DotNetCorezhHans;
using Microsoft.EntityFrameworkCore;

namespace DotNetCorezhHans.Db.Models
{
    [Index(nameof(Original), IsUnique = true)]
    public class TranslData : ITranslateValue
    {
        public long Id { get; set; }

        public string Original { get; set; }

        public string Translation { get; set; }
 
        public TranslSource TranslSource { get; set; }


        [Column(TypeName = "DateTime")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;

    }
}
