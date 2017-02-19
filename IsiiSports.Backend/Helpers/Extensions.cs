using System.Net.Http;
using System.Web.Http;

namespace IsiiSports.Backend.Helpers
{
    public static class Extensions
    {
        public static HttpResponseException ToException(this string s, HttpRequestMessage msg)
        {
            return new HttpResponseException(msg.CreateBadRequestResponse(s));
        }

    }
}