using System.Net;

namespace HUMap.Services;

public class TimetableService
{
    private static async Task DownloadFile(string fileUrl, string destinationPath)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(fileUrl);
        if (response is not { StatusCode: HttpStatusCode.OK })
            throw new Exception("Error downloading file");
        using var content = response.Content;
        await using var stream = await content.ReadAsStreamAsync();
        await using var fileStream = File.Create(destinationPath);
        await stream.CopyToAsync(fileStream);
    }

    public async Task<IEnumerable<TimetableItem>> GetItems()
    {
        var filepath = Path.Combine(FileSystem.AppDataDirectory, "cal.ics");
        var error = false;
        var fileExists = File.Exists(filepath);
        var icsKeyExists = Preferences.Default.ContainsKey("ICalUrl");
        string fileUrl = Preferences.Default.Get("ICalUrl", "");
        try
        {
            await DownloadFile(fileUrl, filepath);
        }
        catch
        {
            error = true;
        }

        var timetableItems = new List<TimetableItem>();
        if (!icsKeyExists)
        {
            timetableItems.Add(new TimetableItem
            {
                Title = "Error",
                lType = "No ICal URL",
                IsNotDayOfWeekItem = false,
                Colour = Color.FromArgb("#FF0000")
            });
            return timetableItems;
        }

        if (!fileExists)
        {
            timetableItems.Add(new TimetableItem
            {
                Title = "Error",
                lType = "Failed first download of timetable",
                IsNotDayOfWeekItem = false,
                Colour = Color.FromArgb("#FF0000")
            });
            return timetableItems;
        }

        var addedDays = new HashSet<DateOnly>();
        var file = new StreamReader(filepath);
        var icsContent = await file.ReadToEndAsync();
        var today = DateOnly.FromDateTime(DateTime.Now);
        today = today.AddDays(-300);
        file.Close();

        var calendar = Ical.Net.Calendar.Load(icsContent);
        const char delimiter = '[';
        if (error)
        {
            timetableItems.Add(new TimetableItem
            {
                Title = "Error updating timetable",
                lType = "Check your connection or file link in settings",
                IsNotDayOfWeekItem = false,
                Colour = Color.FromArgb("#FF0000")
            });
        }

        foreach (var cal in calendar.Events)
        {
            var startDay = DateOnly.FromDateTime(cal.Start.AsSystemLocal.Date);
            if (startDay < today)
                continue;

            if (!addedDays.Contains(startDay))
            {
                addedDays.Add(startDay);
                var item = new TimetableItem
                {
                    Title = startDay.ToString("dddd"),
                    lType = startDay.ToString("dd/MM/yyyy"),
                    IsNotDayOfWeekItem = false,
                    Colour = Color.FromArgb("#dcf2f5")
                };
                if (startDay == today)
                {
                    item.Title = "(Today) " + startDay.ToString("dddd");
                }

                timetableItems.Add(item);
            }

            var startIndex = cal.Summary.IndexOf('[') + 1;
            var endIndex = cal.Summary.IndexOf(']');
            var lType = cal.Summary.Substring(startIndex, endIndex - startIndex);

            timetableItems.Add(new TimetableItem
            {
                Title = cal.Summary[..cal.Summary.IndexOf(delimiter)],
                Description = cal.Description,
                lType = lType,
                Location = cal.Location,
                StartTime = cal.Start.AsSystemLocal.ToString("HH:mm"),
                EndTime = cal.End.AsSystemLocal.ToString("HH:mm"),
                IsNotDayOfWeekItem = true,
                Colour = Color.FromArgb("#f5f7f6")
            });
        }

        return timetableItems;
    }
}