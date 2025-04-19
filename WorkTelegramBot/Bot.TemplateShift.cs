using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WorkTelegramBot
{
    partial class Bot
    {
        private async static Task OnGettingTemplateMessage(Message message, Telegram.Bot.Types.Enums.UpdateType type)
        {
            if (message.From.Id == gettingMessagesFromId)
            {
                bot.OnMessage += Bot_OnMessage;
                gettingMessagesFromId = 0;
                await bot.SendMessage(message.Chat.Id, UpdateTable(message.Text));
                await GetStartMessage(message);
            }
        }
    }
}
