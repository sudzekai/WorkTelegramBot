using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WorkTelegramBot
{
    partial class Bot
    {
        private static async Task CloseShift(Message message)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                result.AppendLine("Закрытие");
                bot.OnMessage -= Bot_OnMessage;
                ReplyKeyboardMarkup keyboard = new();
                foreach (var element in parks)
                {
                    keyboard.AddNewRow().AddButton(element);
                }
                await bot.SendMessage(message.Chat.Id, "Напиши название парка или выбери один из существующих",
                                      replyMarkup: keyboard);
                bot.OnMessage += OnGettingParkNameClose;
            }
        }

        private async static Task OnGettingParkNameClose(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                if (parks.Contains(message.Text, StringComparer.OrdinalIgnoreCase))
                {
                    result.AppendLine(message.Text);
                    await bot.SendMessage(message.Chat.Id, $"Выбран парк {message.Text}. Теперь напиши номер последнего билета", replyMarkup: new ReplyKeyboardMarkup());
                    bot.OnMessage += OnGettingLastTicket;
                    bot.OnMessage -= OnGettingParkNameClose;
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

        private async static Task OnGettingLastTicket(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                if (int.TryParse(message.Text, out _))
                {
                    bot.OnMessage -= OnGettingLastTicket;
                    result.AppendLine($"Билет: {message.Text}");
                    await bot.SendMessage(message.Chat.Id, $"Номер последнего билета: {message.Text}. Теперь напиши количество чеков", replyMarkup: new ReplyKeyboardMarkup());
                    bot.OnMessage += OnGettingTicketsAmount;
                }
                else
                {
                    await bot.SendMessage(message.Chat.Id, $"Номер последнего билета введён неверно, попробуй снова");
                }
            }

        }
        private async static Task OnGettingTicketsAmount(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                if (int.TryParse(message.Text, out _))
                {
                    bot.OnMessage -= OnGettingTicketsAmount;
                    result.AppendLine($"Чеки: {message.Text}");
                    await bot.SendMessage(message.Chat.Id, $"Количество чеков: {message.Text}. Теперь напиши сумму наличных", replyMarkup: new ReplyKeyboardMarkup());
                    bot.OnMessage += OnGettingCashAmount;
                }
                else
                {
                    await bot.SendMessage(message.Chat.Id, $"Количество чеков введено неверно, попробуй снова");
                }
            }
        }

        private async static Task OnGettingCashAmount(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                if (int.TryParse(message.Text, out _))
                {
                    bot.OnMessage -= OnGettingCashAmount;
                    result.AppendLine($"Наличка: {message.Text}");
                    bot.OnMessage += Bot_OnMessage;

                    await bot.SendMessage(-1002361758193, result.ToString());
                    await bot.SendMessage(message.Chat.Id, UpdateTable(result.ToString()));
                    await GetStartMessage(message);
                    result = new StringBuilder();
                    gettingMessagesFromId = 0;

                }
                else
                {
                    await bot.SendMessage(message.Chat.Id, $"Сумма введена неверно, попробуй снова");
                }
            }
        }
    }
}
