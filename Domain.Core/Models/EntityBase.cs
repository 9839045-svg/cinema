using Domain.Core.Interfaces;

namespace Domain.Core.Models
{
    /// <summary>
    /// Абстрактний базовий клас усіх сутностей предметної області.
    /// Містить ідентифікатор і спільні методи валідації значень для setter-ів властивостей.
    /// </summary>
    public abstract class EntityBase : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        protected static string RequireText(string? value, string fieldName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Поле «{fieldName}» не може бути порожнім.");

            string text = value.Trim();
            if (text.Length > maxLength)
                throw new ArgumentException($"Поле «{fieldName}» задовге (максимум {maxLength} символів).");

            return text;
        }

        protected static int RequireRange(int value, int min, int max, string fieldName)
        {
            if (value < min || value > max)
                throw new ArgumentException($"Поле «{fieldName}» має бути в межах від {min} до {max}.");

            return value;
        }

        protected static decimal RequireRange(decimal value, decimal min, decimal max, string fieldName)
        {
            if (value < min || value > max)
                throw new ArgumentException($"Поле «{fieldName}» має бути в межах від {min} до {max}.");

            return value;
        }
    }
}
