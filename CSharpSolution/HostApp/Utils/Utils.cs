using System.Net;

namespace HostApp.Utils;

public class Utils
{
    public static async Task CheckResponse(HttpResponseMessage res)
    {
        if (res.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await res.Content.ReadAsStringAsync());
        if (!res.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"{res.StatusCode} {res.ReasonPhrase}: {await res.Content.ReadAsStringAsync()}");
    }
}