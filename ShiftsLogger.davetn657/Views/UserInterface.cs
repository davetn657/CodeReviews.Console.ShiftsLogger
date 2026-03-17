using ShiftsLogger.davetn657.Controllers;
using ShiftsLogger.davetn657.Models;
using Spectre.Console;
using System.Text;
using System.Text.Json;

namespace ShiftsLogger.davetn657.Views
{
    internal class UserInterface
    {
        private readonly Validation _validation;
        private readonly HttpClient _client;
        private readonly Uri _baseUri;

        public UserInterface(Validation validation, HttpClient client, Uri baseUri)
        {
            _validation = validation;
            _client = client;
            _baseUri = baseUri;
        }

        internal async Task StartMenu()
        {
            while (true)
            {
                TitleCard("Shifts Logger");

                var menuOptions = new List<string>()
                {
                    "Exit",
                    "Add Shift"
                };

                var logNames = new List<Shift>();
                var logs = new Dictionary<string, Shifts>();

                try
                {
                    var response = await _client.GetAsync(_baseUri);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    logNames = JsonSerializer.Deserialize<List<Shift>>(json);

                    foreach (var log in logNames)
                    {
                        if (logs.ContainsKey(log.LoggerName))
                        {
                            var userLogs = logs[log.LoggerName];
                            userLogs.UserShifts.Add(log);
                        }
                        else
                        {
                            var userLogs = new Shifts();
                            userLogs.UserShifts.Add(log);
                            logs.Add(log.LoggerName, userLogs);
                            menuOptions.Add(log.LoggerName);
                        }
                    }

                    var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(menuOptions));

                    switch (selected)
                    {
                        case "Exit":
                            return;
                        case "Add Shift":
                            await AddShiftMenu();
                            break;
                        default:
                            EditUserShifts(logs[selected]);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteLine($"Request failed: {ex.Message}");
                    AnsiConsole.WriteLine(ex.ToString());
                    return;
                }

            }
            
        }

        private void EditUserShifts(Shifts shifts)
        {
            TitleCard($"Edit {shifts.UserShifts.FirstOrDefault().LoggerName}'s Shifts");

            var table = new Table();
            table.AddColumn("Shift #");
            table.AddColumn("Start Time");
            table.AddColumn("End Time");
            table.AddColumn("Total Duration");

            var menuOptions = new List<string>()
                {
                    "Return"
                };

            var count = 0;

            foreach (var shift in shifts.UserShifts)
            {
                table.AddRow(count.ToString(), shift.ShiftStart.ToString(), shift.ShiftEnd.ToString(), shift.Duration.ToString());
                menuOptions.Add(count.ToString());
                count++;
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine("Choose which shift to edit");
            var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(menuOptions));

            if (selected.Equals("Return")) return;

            if (Int32.TryParse(selected, out int i))
            {
                EditShift(shifts.UserShifts[i]);
            }
            else
            {
                AnsiConsole.WriteLine("Couldn't find shift data!");
                AnsiConsole.Prompt(new TextPrompt<string>("Press Enter to Return").AllowEmpty());
            }
        }

        private async Task EditShift(Shift shift)
        {
            var menuOptions = new List<string>()
                {
                    "Return",
                    "Delete Shift",
                    "Edit Start Time",
                    "Edit End Time"
                };

            var selected = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(menuOptions));

            if (selected.Equals("Return")) return;

            if(selected.Equals("Delete Shift"))
            {
                try
                {
                    var response = await _client.DeleteAsync(_baseUri + $"/{shift.id}");
                    response.EnsureSuccessStatusCode();

                    return;
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteLine($"Request failed: {ex.Message}");
                    AnsiConsole.WriteLine(ex.ToString());
                    return;
                }
            }

            if (selected.Equals("Edit Start Time"))
            {
                while (true)
                {
                    var startTime = GetTime();
                    var endTime = _validation.IsValidTime(startTime, shift.ShiftEnd);
                    if (endTime != null)
                    {
                        shift.ShiftStart = startTime;
                        break;
                    }
                    AnsiConsole.WriteLine("!!! End time must be ahead of start time !!!");
                }

            }
            else
            {
                while (true)
                {
                    var endTime = _validation.IsValidTime(shift.ShiftStart, GetTime());
                    if (endTime != null)
                    {
                        shift.ShiftEnd = endTime;
                        break;
                    }
                    AnsiConsole.WriteLine("!!! End time must be ahead of start time !!!");
                }
            }

            shift.Duration = shift.ShiftEnd - shift.ShiftStart;

            try
            {
                var json = JsonSerializer.Serialize(shift);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PutAsync(_baseUri + $"/{shift.id}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Request failed: {ex.Message}");
                AnsiConsole.WriteLine(ex.ToString());
            }

        }

        private async Task AddShiftMenu()
        {
            TitleCard("Log a Shift");

            var name = AnsiConsole.Ask<string>("Enter shift logger's name (or type r to return): ");

            if (name.ToLower() == "r") return;

            var startTime = GetTime();
            var endTime = _validation.IsValidTime(startTime, GetTime());

            while (endTime == null)
            {
                AnsiConsole.WriteLine("End time must be ahead of start time!");
                endTime = _validation.IsValidTime(startTime, GetTime());
            }

            var duration = endTime - startTime;

            var newShift = new Shift()
            {
                LoggerName = name,
                ShiftStart = startTime,
                ShiftEnd = endTime,
                Duration = duration
            };

            var json = JsonSerializer.Serialize(newShift);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync(_baseUri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Request failed: {ex.Message}");
                AnsiConsole.WriteLine(ex.ToString());
            }

            AnsiConsole.Prompt(new TextPrompt<string>("Return").AllowEmpty());

        }

        private DateTime? GetTime()
        {
            while (true)
            {
                var time = _validation.IsValidTime(AnsiConsole.Ask<string>("Enter a valid time (hh:mm): "));

                if (time != null) return time;
                else
                {
                    AnsiConsole.WriteLine("Please enter a valid time!");
                }
            }
        }

        private void TitleCard(string title)
        {
            var titleCard = new FigletText(title)
            {
                Justification = Justify.Center
            };

            AnsiConsole.Clear();
            AnsiConsole.Write(titleCard);
        }
    }
}
