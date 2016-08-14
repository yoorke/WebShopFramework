using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace eshopUtilities
{
    public class CertificateWebClient:WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            string path = System.Web.HttpContext.Current.Server.MapPath("~/b2b_pin_servis_stkr.p12");
            request.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate(System.IO.File.ReadAllBytes(path), "0328", System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet));
            return request;
        }
    }
}
