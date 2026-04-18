namespace Market.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,    // Створено, очікує підтвердження/оплати
    Confirmed = 1,  // Підтверджено (резерв інвентарю закріплено)
    Shipped = 2,    // Відправлено клієнту
    Delivered = 3,  // Успішно доставлено
    Cancelled = 4   // Скасовано (резерв потрібно зняти)
}
