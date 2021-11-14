using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DotNetCorezhHans.Db.Models
{
    public class FileMd5
    {
        [Key]
        public Guid Id { get; set; }

        public string FileName { get; set; }
    }
}
