using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TextCommandFramework.Services;

public class PictureService
{
    private readonly HttpClient _http;

    public PictureService(HttpClient http) => _http = http;

    public async Task<Stream> HttpRequest(string url)
    {
        var resp = await _http.GetAsync(url);
        return await resp.Content.ReadAsStreamAsync();
    }

    public async Task<Stream> GetCatPictureAsync()
    {
        return await HttpRequest("https://cataas.com/cat");
    }

    public async Task<Stream> GetCatSayPictureAsync(string message)
    {
        return await HttpRequest("https://cataas.com/cat/says/" + message);
    }

    public async Task<Stream> GetCatTagPictureAsync(string message)
    {
        return await HttpRequest("https://cataas.com/cat/" + message);
    }

    public async Task<Stream> GetCatTagSayPictureAsync(string message)
    {
        return await HttpRequest(
            "https://cataas.com/cat/" + message.Split("/")[0] + "/says/" + message.Split("/")[1]
        );
    }

    public async Task<Stream> GetCatGifAsync()
    {
        return await HttpRequest("https://cataas.com/cat/gif");
    }

    public async Task<Stream> GetCatGifSayPictureAsync(string message)
    {
        return await HttpRequest("https://cataas.com/cat/gif/says/" + message);
    }
}
