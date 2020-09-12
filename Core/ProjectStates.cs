using System.ComponentModel.DataAnnotations;

namespace Core
{
    public enum SessionState
    {
        [Display(Name = "Настройка")]
        Setup,
        [Display(Name = "Тестирование")]
        Test,
        [Display(Name = "Работа")]
        Work
    }
}
