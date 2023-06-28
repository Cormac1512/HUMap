namespace HUMap.Services;

public class TimetableService
{
    private static async Task DownloadFile(string fileUrl, string destinationPath)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(fileUrl);
        using var content = response.Content;
        await using var stream = await content.ReadAsStreamAsync();
        await using var fileStream = File.Create(destinationPath);
        await stream.CopyToAsync(fileStream);
    }

    public async Task<IEnumerable<TimetableItem>> GetItems()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "cal.ics");
        if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, "cal.ics")))
        {
            const string fileUrl =
                "https://mytimetable.hull.ac.uk/ical?649c351a&group=false&eu=NjYwMjg0&h=RA6Kkai83GpoDHCJ7VmSQdXbrmDCLe0_5VWn0iTqNhs=";
            await DownloadFile(fileUrl, path);
        }

        var filepath = Path.Combine(FileSystem.AppDataDirectory, "cal.ics");
        var file = new StreamReader(filepath);
        var icsContent = await file.ReadToEndAsync();
        var today = DateOnly.FromDateTime(DateTime.Now);
        today = today.AddDays(-100);
        file.Close();

        var calendar = Ical.Net.Calendar.Load(icsContent);
        const char delimiter = '[';
        var date = DateTime.Now.ToString("dddd");
        return calendar.Events
            .Where(cal => DateOnly.FromDateTime(cal.Start.AsSystemLocal.Date) >= today) // Filter out past events
            .Select(cal =>
            {
                var startIndex = cal.Summary.IndexOf('[') + 1;
                var endIndex = cal.Summary.IndexOf(']');
                var lType = cal.Summary.Substring(startIndex, endIndex - startIndex);

                return new TimetableItem
                {
                    Title = cal.Summary[..cal.Summary.IndexOf(delimiter)],
                    Description = cal.Description,
                    lType = lType,
                    Location = cal.Location,
                    StartTime = cal.Start.AsSystemLocal.ToString("HH:mm"),
                    EndTime = cal.End.AsSystemLocal.ToString("HH:mm"),
                };
            })
            .ToList();
    }
}