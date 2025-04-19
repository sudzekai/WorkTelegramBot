using OfficeOpenXml;
using System.IO;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WorkTelegramBot
{
    partial class Bot
    {
        static string _token = Constants._token;
        static long _adminGroupId = Constants._adminGroupId;
        public const long _adminId = Constants._adminId;


        static EPPlusLicense license = new();
        private static TelegramBotClient bot;
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        static long gettingMessagesFromId = 0;

        static StringBuilder result = new();

        static string[] parks = Constants.parks;

        static List<string> workers = Constants.workers;

        static void Main(string[] args)
        {
            license.SetNonCommercialPersonal("zalupa");
            bot = new(token: _token, cancellationToken: _cts.Token);

            bot.OnError += Bot_OnError;
            bot.OnMessage += Bot_OnMessage;
            bot.OnUpdate += Bot_OnUpdate;

            var me = bot.GetMe().Result;

            Console.WriteLine($"ID: {me.Id}, Username: @{me.Username}\nEnter, чтобы выключить бота.");

            Console.ReadLine();
            _cts.Cancel(); // stop the bot

            workers.Sort();
        }

        private static Task Bot_OnUpdate(Update update)
        {
            return Task.CompletedTask;
        }

        private async static Task Bot_OnMessage(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == _adminId)
            {
                if (message.Text.Contains("/workers", StringComparison.OrdinalIgnoreCase))
                {
                    if (message.Text.Contains("-add", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            workers.Add(message.Text.Split("-add")[1]);
                            await bot.SendMessage(message.Chat.Id, $"{message.Text.Split("-add")[1]} добавлен в список сотрудников");
                        }
                        catch (Exception e)
                        {
                            await bot.SendMessage(message.Chat.Id, e.Message);
                        }
                        return;
                    }
                    else if (message.Text.Contains("-rm", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            workers.Remove(message.Text.Split("-rm")[1].Trim());
                            ReplyKeyboardMarkup keyboard = new();
                            foreach (var element in workers)
                            {
                                keyboard.AddNewRow().AddButton($"/workers -rm {element}");
                            }
                            await bot.SendMessage(message.Chat.Id, $"{message.Text.Split("-rm")[1].Trim()} удален из списка сотрудников", replyMarkup: keyboard);
                        }
                        catch (Exception e)
                        {
                            await bot.SendMessage(message.Chat.Id, e.Message);
                        }
                        return;
                    }
                }
            }

            switch (message.Text.ToLower())
            {
                case "/start":
                    await GetStartMessage(message);
                    break;

                case "/test":
                    break;

                case "открытие":
                    gettingMessagesFromId = message.From.Id;
                    await OpenShift(message);
                    break;

                case "закрытие":
                    gettingMessagesFromId = message.From.Id;
                    await CloseShift(message);
                    break;

                case "шаблон":
                    gettingMessagesFromId = message.From.Id;
                    bot.OnMessage -= Bot_OnMessage;
                    bot.OnMessage += OnGettingTemplateMessage;
                    await bot.SendMessage(message.Chat.Id, "Отправь шаблонное сообщение об открытии или закрытии смены");
                    break;

                case "/table"
                or "/таблица"
                or "/итог"
                or "/-t":
                    {
                        if (message.Chat.Id == _adminGroupId)
                        {
                            await using Stream stream = File.OpenRead("file.xlsx");
                            await bot.SendDocument(
                                message.Chat.Id,
                                document: InputFile.FromStream(stream));
                        }
                    }
                    break;

                case "/clear":
                    if (message.Chat.Id == _adminGroupId)
                    {
                        await using Stream stream = File.OpenRead("file.xlsx");
                        await bot.SendDocument(
                            message.Chat.Id,
                            document: InputFile.FromStream(stream),
                            caption: "Таблица очищена\nТаблица перед очищением:");
                        ClearTable();
                    }
                    break;
                default:
                    break;
            }
        }

        private static async Task GetStartMessage(Message message)
        {
            await bot.SendMessage(message.Chat.Id, "Это рабочий бот для открытия и закрытия смен проката электромобилей и продажи сахарной ваты и попкорна.\n\nДля работы нажмите на одну из кнопок", replyMarkup: new ReplyKeyboardMarkup().AddButton("Открытие").AddButton("Закрытие").AddNewRow().AddButton("Шаблон"));
        }

        private static async Task Bot_OnError(Exception exception, Telegram.Bot.Polling.HandleErrorSource source)
        {
            await using Stream stream = File.OpenRead("file.xlsx");
            await bot.SendDocument(
                            _adminId,
                            document: InputFile.FromStream(stream));
            await bot.SendMessage(_adminId, $"Ошибка: {exception.Message}\n Полный код оишбки: {exception}");
        }
    }
}
