using System.ComponentModel.DataAnnotations;

namespace Core
{
    /// <summary>
    /// Режим работы проекта
    /// </summary>
    public enum ProjectWorkModes
    {
        [Display(Name = "Настройка")]
        Setup,
        [Display(Name = "Тестирование")]
        Test,
        [Display(Name = "Работа")]
        Work
    }
}
