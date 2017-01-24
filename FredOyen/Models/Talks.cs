using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FredOyen.Models
{
    public class Talks
    {
        [Key]
        public int talk_id { get; set; }
        public bool? isPaul { get; set; }
        public string filePath { get; set; }
        public string Comment { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime SentDate { get; set; }

        [NotMapped]
        public string sourceFile { get; set; }
        [NotMapped]
        public byte[] fileBytes { get; set; }

    }
}