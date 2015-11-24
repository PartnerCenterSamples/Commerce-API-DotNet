using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Partner.CSP.Api.V1.Samples
{
    public static  class Invoker
    {
        public static bool Invoke<T>( Func<T> func, int successCode)
        {
            bool result = TryInvoke(func, successCode);
            return result;
        }

        private static bool TryInvoke<T>(Func<T> initFunc, int successCode)
        {
          
            int apiResultCode = -1;
            bool first = true;

            do
            {
                if (!first)
                {
                    Thread.Sleep(2000);
                }
                else
                {
                    first = false;
                };

                var response = initFunc();
                var httpResponse = response as HttpResponse;
                if(httpResponse == null) continue;
                apiResultCode = httpResponse.StatusCode;

            } while (successCode != apiResultCode);

            return true;
        }
    }
}
