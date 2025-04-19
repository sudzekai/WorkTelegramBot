using OfficeOpenXml;

namespace WorkTelegramBot
{
    partial class Bot
    {
        static string UpdateTable(string message)
        {
            int plus = -1;

            var fileInfo = new FileInfo("file.xlsx");

            if (message.Contains("открытие", StringComparison.OrdinalIgnoreCase))
            {
                if (message.Contains("галушина", StringComparison.OrdinalIgnoreCase))
                    plus = 0;

                else if (message.Contains("катунино", StringComparison.OrdinalIgnoreCase))
                    plus = 13;

                else if (message.Contains("новодвинск", StringComparison.OrdinalIgnoreCase))
                    plus = 26;

                else if (message.Contains("красная пристань", StringComparison.OrdinalIgnoreCase))
                    plus = 39;

                if (plus != -1)
                {
                    using (var package = new ExcelPackage(fileInfo))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Получаем первый лист

                        foreach (string line in message.Split("\n"))
                        {
                            if (line.Contains("билет", StringComparison.OrdinalIgnoreCase))
                            {
                                worksheet.Cells[2 + plus, Convert.ToInt32(DateTime.Now.ToString("dd")) + 1].Value = Convert.ToInt32(line.Split(":")[1].Trim()); // первый билет 
                            }
                            else if (line.Contains("админ", StringComparison.OrdinalIgnoreCase))
                            {
                                worksheet.Cells[8 + plus, Convert.ToInt32(DateTime.Now.ToString("dd")) + 1].Value = line.Split(":")[1].Trim(); // админ 
                            }
                            else if (line.Contains("помощник", StringComparison.OrdinalIgnoreCase))
                            {
                                worksheet.Cells[9 + plus, Convert.ToInt32(DateTime.Now.ToString("dd")) + 1].Value = line.Split(":")[1].Trim(); // помощник
                            }
                        }
                        worksheet.Cells[11 + plus, Convert.ToInt32(DateTime.Now.ToString("dd")) + 1].Value = DateTime.Now.ToString("HH:mm"); // помощник

                        package.Save();
                    }

                    return "Смена открыта";
                }
                return "Неверное название парка";
            }

            else if (message.Contains("закрытие", StringComparison.OrdinalIgnoreCase))
            {
                if (message.Contains("галушина", StringComparison.OrdinalIgnoreCase))
                    plus = 0;

                else if (message.Contains("катунино", StringComparison.OrdinalIgnoreCase))
                    plus = 13;

                else if (message.Contains("новодвинск", StringComparison.OrdinalIgnoreCase))
                    plus = 26;

                else if (message.Contains("красная пристань", StringComparison.OrdinalIgnoreCase))
                    plus = 39;

                if (plus != -1)
                {
                    using (var package = new ExcelPackage(fileInfo))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // Получаем первый лист

                        foreach (string line in message.Split("\n"))
                        {
                            if (line.Contains("билет", StringComparison.OrdinalIgnoreCase))
                            {
                                worksheet.Cells[3 + plus, Convert.ToInt32(DateTime.Now.ToString("dd")) + 1].Value = Convert.ToInt32(line.Split(":")[1].Trim());
                            }
                            else if (line.Contains("чеки", StringComparison.OrdinalIgnoreCase))
                            {
                                worksheet.Cells[7 + plus, Convert.ToInt32(DateTime.Now.ToString("dd")) + 1].Value = Convert.ToInt32(line.Split(":")[1].Trim());
                            }
                            else if (line.Contains("наличка", StringComparison.OrdinalIgnoreCase))
                            {
                                worksheet.Cells[5 + plus, Convert.ToInt32(DateTime.Now.ToString("dd")) + 1].Value = Convert.ToInt32(line.Split(":")[1].Trim());
                            }
                        }
                        worksheet.Cells[12 + plus, Convert.ToInt32(DateTime.Now.ToString("dd")) + 1].Value = DateTime.Now.ToString("HH:mm"); // помощник

                        package.Save();
                    }
                    return "Смена закрыта";
                }
                return "Неверное название парка";
            }
            return "Не указано действие \"Закрытие\" или \"Открытие\"";
        }

        static string ClearTable()
        {
            int[] indexes = [2, 8, 9, 3, 7, 5, 11, 12];
            var fileInfo = new FileInfo("file.xlsx");
            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                for (int j = 0; j < 40; j += 13)
                {
                    foreach (var index in indexes)
                    {
                        int resindex = index + j;
                        for (int i = 2; i < 33; i++)
                        {
                            worksheet.Cells[resindex, i].Value = null;
                        }
                    }
                }
                package.Save();

            }
            return "";
        }
    }
}
