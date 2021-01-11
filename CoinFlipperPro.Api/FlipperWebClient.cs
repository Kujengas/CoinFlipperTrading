using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoinFlipperPro.Api
{
    class FlipperWebClient:WebClient
    {

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).KeepAlive = false;
                (request as HttpWebRequest).ProtocolVersion = HttpVersion.Version10;
            }
            return request;

        }
    }
}
