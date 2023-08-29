namespace HUMap.Models;

public class TimetableItem
{
    public Color Colour { get; set; }
    public string Description { get; set; }
    public string EndTime { get; set; }
    public bool IsNotDayOfWeekItem { get; set; }
    public string Latitude { get; set; }

    // Possible future use
    public string Location { get; set; }

    public string Longitude { get; set; }
    public string lType { get; set; }
    public string StartTime { get; set; }
    public string Title { get; set; }
}