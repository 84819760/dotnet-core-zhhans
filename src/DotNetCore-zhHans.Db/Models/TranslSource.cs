using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DotNetCorezhHans.Db.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class TranslSource
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
