using NLog;
using System.Diagnostics;
using System.IO;

namespace Core.CyclogrammUtility
{
    public class LinearCyclogrammConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public void Convert(Stream csvCyclogrammStream, Stream cycOutputStream)
        {
            Stopwatch measure = Stopwatch.StartNew();

            byte currentByte;
            int currentByteBuffer;

            int ColorByteCount = 1; //счетчик от 1 до 3 (Определяет значение одного цвета)
            string colorBuffer = "";

            cycOutputStream.Position = 0;
            while(csvCyclogrammStream.Position <= csvCyclogrammStream.Length) {
                currentByteBuffer = csvCyclogrammStream.ReadByte();
                if(currentByteBuffer > 0) {
                    currentByte = (byte)(currentByteBuffer);
                }
                else {
                    break;
                }

                // Проверка, является ли текущий байт запятой "," символом перевода на новую строку или символом возврата каретки
                if((currentByte == 44) || (currentByte == 13) || (currentByte == 10))
                    continue;

                if(ColorByteCount <= 3) {
                    colorBuffer += System.Convert.ToChar(currentByte);
                    ColorByteCount++;
                }

                if(ColorByteCount == 4) {
                    ColorByteCount = 1;
                    cycOutputStream.WriteByte(System.Convert.ToByte(colorBuffer));
                    colorBuffer = "";
                }
            }

            measure.Stop();
            logger.Info($"Converting time: {measure.Elapsed.Minutes}:{measure.Elapsed.Seconds}");
        }
    }
}
