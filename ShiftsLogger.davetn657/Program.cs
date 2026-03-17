using ShiftsLogger.davetn657.Controllers;
using ShiftsLogger.davetn657.Views;

namespace ShiftsLogger.davetn657
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var validation = new Validation();
            var client = new HttpClient();
            var baseUri = new Uri("https://localhost:7129/api/Shifts");

            var UserInterface = new UserInterface(validation, client, baseUri);

            await UserInterface.StartMenu();
        }
    }
}
