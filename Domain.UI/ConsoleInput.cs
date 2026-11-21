using System.Globalization;

namespace Domain.UI
{
    /// <summary>
    /// Безпечне читання даних з консолі: кожен метод повторює запит,
    /// доки користувач не введе коректне значення.
    /// </summary>
    public static class ConsoleInput
    {
        public const string DateTimeFormat = "dd.MM.yyyy HH:mm";

        private static string ReadLine(string prompt)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();

            // null означає, що потік введення закрито — продовжувати цикл запитів немає сенсу.
            if (line == null)
                throw new OperationCanceledException("Введення завершено.");

            return line.Trim();
        }

        public static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                string text = ReadLine(prompt);
                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                    && result >= min && result <= max)
                {
                    return result;
                }

                Console.WriteLine($"Помилка! Введіть ціле число від {min} до {max}.");
            }
        }

        public static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                string text = ReadLine(prompt).Replace(',', '.');
                if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture, out decimal result)
                    && result >= min && result <= max)
                {
                    return result;
                }

                Console.WriteLine($"Помилка! Введіть число від {min} до {max} (наприклад, 150.50).");
            }
        }

        public static string ReadText(string prompt, int maxLength)
        {
            while (true)
            {
                string text = ReadLine(prompt);
                if (text.Length > 0 && text.Length <= maxLength)
                    return text;

                Console.WriteLine($"Помилка! Рядок не має бути порожнім і довшим за {maxLength} символів.");
            }
        }

        public static DateTime ReadDateTime(string prompt)
        {
            while (true)
            {
                string text = ReadLine(prompt);
                if (DateTime.TryParseExact(text, DateTimeFormat, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }

                Console.WriteLine("Помилка! Формат дати й часу: дд.мм.рррр гг:хх (наприклад, 25.12.2026 18:30).");
            }
        }

        public static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                string text = ReadLine(prompt + " (так/ні): ").ToLowerInvariant();
                if (text is "так" or "т" or "y" or "yes")
                    return true;
                if (text is "ні" or "н" or "n" or "no")
                    return false;

                Console.WriteLine("Помилка! Введіть «так» або «ні».");
            }
        }

        public static void WaitForEnter()
        {
            Console.Write("\nНатисніть Enter, щоб продовжити...");
            if (Console.ReadLine() == null)
                throw new OperationCanceledException("Введення завершено.");
        }
    }
}
