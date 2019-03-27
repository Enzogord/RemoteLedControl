using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.Exceptions
{
    public static class ExceptionsHelper
    {
        public static Exception GetUserNotificationNotImplementedException(Exception innerException)
        {
            return new NotImplementedException("Не реализован вывод сообщения пользователю из модели", innerException);
        }
    }
}
