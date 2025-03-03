using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;

namespace eshopBL
{
    public class RecaptchaValidate
    {
        public bool Validate(string secretKey, string responseToken)
        {
            using (HttpClient httpClinet = new HttpClient())
            {
                var res = httpClinet.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={responseToken}").Result;

                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                string responseVal = res.Content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(responseVal);

                if (data.success != "true" || data.score <= 0.5m)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
