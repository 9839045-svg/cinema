using System.Text;
using Domain.Core.Exceptions;
using Domain.Core.Models;
using Domain.Core.Repositories;
using Domain.Core.Services;
using Domain.UI;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");

var halls = new Repository<Hall>();
var shows = new Repository<MovieShow>();
var tickets = new Repository<Ticket>();

var storage = new CinemaStorage(dataDirectory, halls, shows, tickets);
var service = new CinemaService(halls, shows, tickets);
var app = new CinemaApp(service, storage);

bool canSave = true;

try
{
    storage.Load();
}
catch (DataStorageException ex)
{
    ConsolePrinter.PrintError(ex.Message);
    canSave = ConsoleInput.ReadYesNo("Продовжити з порожніми даними (старі файли будуть перезаписані)?");
    if (!canSave)
        return;
}

try
{
    app.Run();
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nВведення завершено.");
}
finally
{
    // Дані зберігаються при будь-якому виході: звичайному, через помилку чи закриття вводу.
    if (canSave)
    {
        try
        {
            storage.Save();
            Console.WriteLine($"Дані збережено у «{dataDirectory}».");
        }
        catch (DataStorageException ex)
        {
            ConsolePrinter.PrintError(ex.Message);
        }
    }
}
