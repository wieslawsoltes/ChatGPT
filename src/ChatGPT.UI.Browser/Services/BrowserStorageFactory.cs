using ChatGPT.Model.Services;

namespace ChatGPT.UI.Browser.Services;

public class BrowserStorageFactory : IStorageFactory
{
    public IStorageService<T> CreateStorageService<T>() => new BrowserStorageService<T>();
}
