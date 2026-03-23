using Microsoft.JSInterop;

namespace MijnLinks.Services;

public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode;
    private bool _initialized;

    public bool IsDarkMode => _isDarkMode;

    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            var storedValue = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "darkMode");
            _isDarkMode = storedValue == "true";
        }
        catch
        {
            _isDarkMode = false;
        }

        _initialized = true;
    }

    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;

        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "darkMode", _isDarkMode.ToString().ToLower());
        }
        catch
        {
            // Local storage may not be available during prerendering
        }

        OnThemeChanged?.Invoke();
    }
}
