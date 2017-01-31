using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoDKPOCDMPTest.Web.Models
{
    public class PythagoraModel
    {
        public List<Dataset> Datasets { get; set; }
        public string Msg { get; set; }
    }

    public class Dataset
    {
        public int Id { get; set; }
        public int? ValueA { get; set; }
        public int? ValueB { get; set; }
        public int? ValueC { get; set; }
    }
    public class CalculatedDataset
    {
        public int Id { get; set; }
        public double ValueA { get; set; }
        public double ValueB { get; set; }
        public double ValueC { get; set; }
        public string Message { get; set; }
    }

}