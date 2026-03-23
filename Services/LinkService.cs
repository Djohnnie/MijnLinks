using System.Text.Json;
using MijnLinks.Models;

namespace MijnLinks.Services;

public class LinkService
{
    private readonly string _filePath;
    private readonly ILogger<LinkService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LinkService(IConfiguration configuration, ILogger<LinkService> logger)
    {
        _filePath = configuration["PATH_TO_LINKS_FILE"]
            ?? throw new InvalidOperationException("PATH_TO_LINKS_FILE environment variable is not configured.");
        _logger = logger;
    }

    public async Task<List<Link>> GetAllLinksAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var links = JsonSerializer.Deserialize<List<Link>>(json, JsonOptions);
            return links ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load links from {FilePath}", _filePath);
            return [];
        }
    }

    public async Task<List<Link>> GetImportantLinksAsync()
    {
        var links = await GetAllLinksAsync();
        return links.Where(l => l.IsImportant).ToList();
    }

    public async Task<List<Link>> GetLinksByCategoryAsync(string category)
    {
        var links = await GetAllLinksAsync();
        return links.Where(l => l.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        var links = await GetAllLinksAsync();
        return links.Select(l => l.Category).Distinct().OrderBy(c => c).ToList();
    }
}
