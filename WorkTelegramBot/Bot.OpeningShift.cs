using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WorkTelegramBot
{
    partial class Bot
    {
        private static async Task OpenShift(Message message)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                result.AppendLine("Открытие");
                bot.OnMessage -= Bot_OnMessage;
                ReplyKeyboardMarkup keyboard = new();
                foreach (var element in parks)
                {
                    keyboard.AddNewRow().AddButton(element);
                }
                await bot.SendMessage(message.Chat.Id, "Напиши название парка или выбери один из существующих",
                                      replyMarkup: keyboard);
                bot.OnMessage += OnGettingParkName;
            }

        }



        private async static Task OnGettingParkName(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                if (parks.Contains(message.Text, StringComparer.OrdinalIgnoreCase))
                {
                    result.AppendLine(message.Text);
                    await bot.SendMessage(message.Chat.Id, $"Выбран парк {message.Text}. Теперь напиши номер первого билета", replyMarkup: new ReplyKeyboardMarkup());
                    bot.OnMessage += OnGettingFirstTicket;
                    bot.OnMessage -= OnGettingParkName;
                }
                else
                {
                    ReplyKeyboardMarkup keyboard = new();
                    foreach (var element in parks)
                    {
                        keyboard.AddNewRow().AddButton(element);
                    }
                    await bot.SendMessage(message.Chat.Id, "Написан несуществующий парк, попробуй снова",
                                          replyMarkup: keyboard);
                }
            }
        }

        private async static Task OnGettingFirstTicket(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                if (int.TryParse(message.Text, out _))
                {
                    result.AppendLine($"Билет: {message.Text}");
                    ReplyKeyboardMarkup keyboard = new();
                    foreach (var element in workers)
                    {
                        keyboard.AddNewRow().AddButton(element);
                    }
                    await bot.SendMessage(message.Chat.Id, $"Номер первого билета: {message.Text} Теперь напиши админа смены или выбери из списка", replyMarkup: keyboard);
                    bot.OnMessage += OnGettingAdminFullName;
                    bot.OnMessage -= OnGettingFirstTicket;
                }
                else
                {
                    await bot.SendMessage(message.Chat.Id, $"Номер первого билета введён неверно, попробуй снова");
                }
            }

        }
        private async static Task OnGettingAdminFullName(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                result.AppendLine($"Админ: {message.Text}");
                ReplyKeyboardMarkup keyboard = new();
                foreach (var element in workers)
                {
                    keyboard.AddNewRow().AddButton(element);
                }
                await bot.SendMessage(message.Chat.Id, $"Админ смены: {message.Text} Теперь напиши помощника смены или выбери из списка", replyMarkup: keyboard);
                bot.OnMessage -= OnGettingAdminFullName;
                bot.OnMessage += OnGettingSupportFullName;
            }
        }

        private async static Task OnGettingSupportFullName(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                result.AppendLine($"Помощник: {message.Text}");
                bot.OnMessage -= OnGettingSupportFullName;
                bot.OnMessage += Bot_OnMessage;

                await bot.SendMessage(_adminGroupId, result.ToString());
                await bot.SendMessage(message.Chat.Id, UpdateTable(result.ToString()));
                await GetStartMessage(message);
                result = new StringBuilder();
                gettingMessagesFromId = 0;

            }
        }
    }
}
