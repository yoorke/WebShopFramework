using eshop.PaymentProcessor.BE;
using eshop.PaymentProcessor.BL.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Drawing;
using System.IO;
using System.Globalization;

namespace eshop.PaymentProcessor.BL
{
    public class NBSIPSQRCodeGenerator : IIPSGenerator
    {
        public async Task<bool> GenerateIPSQRCodeAsync(IPSQRCodeRequest request, string imagePath)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = request.DecimalSeparator;

            var serviceRequest = new NBSQRCodeGeneratorRequest() {
                K = request.IPSCode,
                V = request.IPSVersion,
                C = request.IPSCharSet,
                R = request.PayeeAccount,
                N = request.PayeeName,
                I = $"{request.Currency}{request.Amount.ToString("F", nfi)}",
                P = request.PayerName,
                SF = request.PaymentCode.ToString(),
                S = request.Purpose,
                RO = $"{request.Model}{request.ReferenceNumber}"
            };

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue($"image/{request.ImageType}"));

                var content = new StringContent(JsonConvert.SerializeObject(serviceRequest), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request.ServiceUrl, content).ConfigureAwait(false);

                if(response.StatusCode == HttpStatusCode.OK)
                {
                    using(var stream = new MemoryStream())
                    {
                        var image = Image.FromStream(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                        image.Save(imagePath);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
