namespace MijnLinks.Models;

public class Link
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string SvgIcon { get; set; } = string.Empty;
    public string IconColor { get; set; } = "#1976d2";
    public string Category { get; set; } = string.Empty;
    public bool IsImportant { get; set; }
}
