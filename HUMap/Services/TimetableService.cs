using System.Net;
using Ical.Net;

namespace HUMap.Services;

public class TimetableService
{
    /// <summary>
    ///     This method downloads a file from an URL and saves it to the given destination path.
    /// </summary>
    /// <param name="fileUrl">The URL of the file to download</param>
    /// <param name="destinationPath">The path to save the downloaded file</param>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs</exception>
    private static async Task DownloadFile(string fileUrl, string destinationPath)
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(7);
        using var response = await client.GetAsync(fileUrl);
        if (response is not { StatusCode: HttpStatusCode.OK })
            throw new Exception("Error downloading file");
        using var content = response.Content;
        await using var stream = await content.ReadAsStreamAsync();
        await using var fileStream = File.Create(destinationPath);
        await stream.CopyToAsync(fileStream);
    }

    /// <summary>
    ///     Gets the timetable items from the calendar URL set in user preferences and downloads the calendar file if not
    ///     already downloaded.
    /// </summary>
    /// <returns>An IEnumerable of TimetableItem objects representing the events in the calendar</returns>
    public static async Task<IEnumerable<TimetableItem>> GetItems()
    {
        var filepath = Path.Combine(FileSystem.AppDataDirectory, "cal.ics");
        var icsKeyExists = Preferences.Default.ContainsKey("ICalUrl");
        var fileUrl = Preferences.Default.Get("ICalUrl", "");
        var timetableItems = new List<TimetableItem>();
        var fileExists = File.Exists(filepath);
        if (!icsKeyExists)
        {
            timetableItems.Add(new TimetableItem
            {
                Title = "Setup",
                lType = "To use the timetable go to the settings page",
                IsNotDayOfWeekItem = false,
                Colour = Color.FromArgb("#28C2D1")
            });
            return timetableItems;
        }

        try
        {
            await DownloadFile(fileUrl, filepath);
        }
        catch
        {
            timetableItems.Add(new TimetableItem
            {
                Title = "Error downloading timetable",
                lType = "Check your connection or settings link",
                IsNotDayOfWeekItem = false,
                Colour = Color.FromArgb("#FF0000")
            });
            if (!fileExists) return timetableItems;
        }

        try
        {
            var addedDays = new HashSet<DateOnly>();
            var file = new StreamReader(filepath);
            var icsContent = await file.ReadToEndAsync();
            var today = DateOnly.FromDateTime(DateTime.Now);
            today = today.AddDays(-900);
            file.Close();

            var calendar = Calendar.Load(icsContent);
            const char delimiter = '[';
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
                    if (startDay == today) item.Title = "(Today) " + startDay.ToString("dddd");

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
        }
        catch
        {
            timetableItems.Add(new TimetableItem
            {
                Title = "Error updating timetable",
                lType = "Check your connection or file link in settings",
                IsNotDayOfWeekItem = false,
                Colour = Color.FromArgb("#FF0000")
            });
        }

        return timetableItems;
    }
}