using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnblogsImageBed
{
    public class HttpHeader
    {
        public string Accept { get; set; }

        public string ContentType { get; set; }

        public string Method { get; set; }

        public int MaxTry { get; set; }

        public string UserAgent { get; set; }
    }
}
