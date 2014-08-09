using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnblogsImageBed
{
	/*
	* Http Header实体类
	*/
    public class HttpHeader
    {
        public string Accept { get; set; }

        public string ContentType { get; set; }

        public string Method { get; set; }

        public int MaxTry { get; set; }

        public string UserAgent { get; set; }

        public string AcceptEncoding { get; set; }

        public string AcceptLanguage { get; set; }

        public string CacheControl { get; set; }

        public string Connection { get; set; }




    }
}
