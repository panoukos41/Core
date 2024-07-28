namespace Microsoft.JSInterop;

public static class IJSInProcessRuntimeExtensions
{
    /// <summary>
    /// Returns the language code name from the browser, e.g. "de"
    /// </summary>
    public static string? GetBrowserLang(this IJSInProcessRuntime? js)
    {
        var language = js.GetBrowserCulture();
        if (language is null) return null;

        if (language.Contains('-'))
        {
            language = language.Split('-')[0];
        }
        if (language.Contains('_'))
        {
            language = language.Split('_')[0];
        }
        return language;
    }

    /// <summary>
    /// Returns the culture language code name from the browser, e.g. "de-DE"
    /// </summary>
    public static string? GetBrowserCulture(this IJSInProcessRuntime? js)
    {
        using var navigator = js?.Invoke<IJSInProcessObjectReference>("navigator.valueOf");
        if (navigator is null) return null;

        var language = navigator.Invoke<string[]>("languages.valueOf")?.ElementAtOrDefault(0);
        language ??= navigator.Invoke<string>("language.valueOf");
        language ??= navigator.Invoke<string>("browserLanguage.valueOf");
        language ??= navigator.Invoke<string>("userLanguage.valueOf");
        return language;
    }
}
