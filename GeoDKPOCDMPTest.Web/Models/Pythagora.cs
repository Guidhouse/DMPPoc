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
        public float? ValueA { get; set; }
        public float? ValueB { get; set; }
        public float? ValueC { get; set; }
    }

}