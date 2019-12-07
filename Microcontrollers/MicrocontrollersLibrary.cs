using Microcontrollers.SupportedMicrocontrollers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcontrollers
{
    public static class MicrocontrollersLibrary
    {
        private static IDictionary<string, IMicrocontroller> microcontrollersDictionary = new Dictionary<string, IMicrocontroller>();

        public static IEnumerable<IMicrocontroller> GetMicrocontrollers()
        {
            List<IMicrocontroller> sds = microcontrollersDictionary.Values.ToList();
            return sds;
        }

        /// <summary>
        /// Возвращает поддерживаемый системой микроконтроллер по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <exception cref="ArgumentNullException">Если идентификатор Null или пуст</exception>
        /// <exception cref="ArgumentException">Если не удалось найти микроконтроллер по заданному идентификатору</exception>
        /// <returns>Микроконтроллер</returns>
        public static IMicrocontroller GetMicrocontroller(string id)
        {
            if(string.IsNullOrWhiteSpace(id)) {
                throw new ArgumentNullException(nameof(id));
            }
            if(!microcontrollersDictionary.ContainsKey(id)) {
                throw new ArgumentException($"Не найден микроконтроллер по ключу '{id}'");
            }
            return microcontrollersDictionary[id];
        }

        /// <summary>
        /// Возвращает поддерживаемый системой микроконтроллер по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <exception cref="ArgumentNullException">Если идентификатор Null или пуст</exception>
        /// <returns>Микроконтроллер</returns>
        public static bool TryGetMicrocontroller(string id, out IMicrocontroller microcontroller)
        {
            microcontroller = null;
            if(string.IsNullOrWhiteSpace(id)) {
                throw new ArgumentNullException(nameof(id));
            }
            if(!microcontrollersDictionary.ContainsKey(id)) {
                return false;
            }
            microcontroller = microcontrollersDictionary[id];
            return true;
        }

        static MicrocontrollersLibrary()
        {
            #region Esp8266

            #region Wemos
            
            microcontrollersDictionary.Add(Wemos.D1ProMiniv110.Id, Wemos.D1ProMiniv110);

            #endregion

            #endregion

            #region Esp32

            #endregion

            #region STM32

            #endregion
        }
    }
}
