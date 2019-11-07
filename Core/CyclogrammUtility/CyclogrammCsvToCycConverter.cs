using NLog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Core.CyclogrammUtility
{
    public class CyclogrammCsvToCycConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<ConvertionProgressEventArgs> OnProgressChanged;

        private enum ConvertState
        {
            Reading,
            Converting,
            Writing
        }

        private struct ColorData
        {
            public bool Initialized { get; }

            private readonly byte[] colorParts;

            public ColorData(int index, byte[] colorParts)
            {
                Initialized = true;
                Index = index;
                this.colorParts = colorParts;
            }

            public int Index { get; }

            public byte GetColorByte()
            {
                string colorString =
                    $"{System.Convert.ToChar(colorParts[0])}" +
                    $"{System.Convert.ToChar(colorParts[1])}" +
                    $"{System.Convert.ToChar(colorParts[2])}";
                byte colorByte = System.Convert.ToByte(colorString);
                return colorByte;
            }
        }

        private CancellationTokenSource cts = new CancellationTokenSource();
        private readonly long memoryBufferSizeBytes;
        private int colorBufferLength;
        private ConvertState state;
        private bool readDataEnding;
        private ConcurrentQueue<ColorData> colorsBufferQueue = new ConcurrentQueue<ColorData>();
        //private BlockingCollection<ColorData> colorsBufferQueue = new BlockingCollection<ColorData>();
        private byte[] colorBuffer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryBufferSizeMb">Размер буфера памяти для конвертирования (в мегабайтах)</param>
        public CyclogrammCsvToCycConverter(int memoryBufferSizeMb = 100)
        {
            long bufferSizeByte = (long)memoryBufferSizeMb * 1024L * 1024L;
            var divisionRemainder = bufferSizeByte % 3;
            if(divisionRemainder > 0) {
                bufferSizeByte += 3 - divisionRemainder;
            }

            colorBufferLength = (int)(bufferSizeByte / 3);
            this.memoryBufferSizeBytes = bufferSizeByte;
        }

        byte[] colorParts = new byte[3];
        int writeColorIndex = 0;
        int bufferByteCounter = 0;
        int bufferByteMaxCounter = 0;

        private void WriteColorPartToBuffer(byte colorPart)
        {
            colorParts[writeColorIndex] = colorPart;
            if(writeColorIndex == 2) {
                colorsBufferQueue.Enqueue(new ColorData(bufferByteCounter, colorParts));
                bufferByteCounter++;
                colorParts = new byte[3];
                writeColorIndex = 0;
                return;
            }
            writeColorIndex++;
        }

        public void Convert(Stream csvCyclogrammStream, Stream cycOutputStream)
        {
            //Stopwatch measure = Stopwatch.StartNew();

            if(csvCyclogrammStream is null) {
                throw new ArgumentNullException(nameof(csvCyclogrammStream));
            }

            if(!csvCyclogrammStream.CanRead) {
                throw new ArgumentException($"Can't read {nameof(csvCyclogrammStream)}");
            }

            if(cycOutputStream is null) {
                throw new ArgumentNullException(nameof(cycOutputStream));
            }

            if(!cycOutputStream.CanWrite) {
                throw new ArgumentException($"Can't write to {nameof(cycOutputStream)}");
            }

            Task writingTask = Task.Factory.StartNew(() => WriteToOutputStream(cycOutputStream));
            Task.Factory.StartNew(() => ConvertToMemoryStream());
            Task.Factory.StartNew(() => ConvertToMemoryStream());
            Task.Factory.StartNew(() => ConvertToMemoryStream());
            Task.Factory.StartNew(() => ConvertToMemoryStream());
            Task.Factory.StartNew(() => ReadToBufferQueue(csvCyclogrammStream));
            Task.Factory.StartNew(() => ProgressTask(csvCyclogrammStream));
            //Task.Factory.StartNew(() => MonitorTask());
            writingTask.Wait();

            OnProgressChanged?.Invoke(this, new ConvertionProgressEventArgs(100, true));

            /*measure.Stop();
            Console.WriteLine($"Converting time: {measure.Elapsed.Minutes}:{measure.Elapsed.Seconds}");*/
        }

        private void ProgressTask(Stream csvCyclogrammStream)
        {
            while(!cts.IsCancellationRequested) {
                int convertPosition = csvCyclogrammStream.Position == inputCyclogrammLength ? colorsBufferQueue.Count : 0;
                byte readPercent = (byte)(((double)(csvCyclogrammStream.Position + convertPosition) / (double)(inputCyclogrammLength + bufferByteMaxCounter)) * 100D);
                OnProgressChanged?.Invoke(this, new ConvertionProgressEventArgs(readPercent, false));
                Thread.Sleep(100);
            }
        }

        private void MonitorTask()
        {
            DateTime last = DateTime.Now;
            while(!cts.IsCancellationRequested) {
                if((DateTime.Now - last).TotalSeconds >= 1) {
                    last = DateTime.Now;
                    logger.Info($"Items in queue: {colorsBufferQueue.Count}, converted colors: {convertedColorsCount}");
                }
                Thread.Sleep(10);
            }
        }

        private long inputCyclogrammLength = 0;

        private void ReadToBufferQueue(Stream csvCyclogrammStream)
        {
            logger.Info("Reading task started");
            csvCyclogrammStream.Position = 0;
            inputCyclogrammLength = csvCyclogrammStream.Length;

            while(!cts.IsCancellationRequested) {
                if(state == ConvertState.Reading) {
                    logger.Info("Start reading converted CSV file");

                    ResetBufferWriteIteration();
                    
                    while(csvCyclogrammStream.Position <= inputCyclogrammLength) {
                        byte currentByte = (byte)csvCyclogrammStream.ReadByte();

                        // Проверка, является ли текущий байт запятой "," символом перевода на новую строку или символом возврата каретки
                        if(currentByte == 44 || currentByte == 13 || currentByte == 10) {
                            continue;
                        }

                        WriteColorPartToBuffer(currentByte);                        

                        if(bufferByteCounter == colorBufferLength || csvCyclogrammStream.Position == inputCyclogrammLength) {
                            readDataEnding = csvCyclogrammStream.Position == inputCyclogrammLength;
                            //часть данных записана в буффер, переключение на ковертирование
                            state = ConvertState.Converting;
                            logger.Info($"Switch to converting. Items in queue: {colorsBufferQueue.Count}");
                            break;
                        }
                    }
                }
                else {
                    Thread.Sleep(10);
                }
            }
            logger.Info("Reading task ended");
        }

        private void ResetBufferWriteIteration()
        {
            if(bufferByteMaxCounter == 0) {
                bufferByteMaxCounter = bufferByteCounter;
            }
            bufferByteCounter = 0;            
            colorBuffer = new byte[colorBufferLength];
        }

        private int convertedColorsCount = 0;

        private void ConvertToMemoryStream()
        {
            logger.Info("Converting task started");
            while(!cts.IsCancellationRequested) {
                /*ColorData result = colorsBufferQueue.Take(cts.Token);
                if(result.Initialized) {
                    colorBuffer[result.Index] = result.GetColorByte();
                    Interlocked.Increment(ref convertedColorsCount);
                }*/
                
                if(colorsBufferQueue.TryDequeue(out ColorData result)) {
                    colorBuffer[result.Index] = result.GetColorByte();
                    Interlocked.Increment(ref convertedColorsCount);
                }
                else {
                    if(state == ConvertState.Converting && colorsBufferQueue.Count == 0) {
                        state = ConvertState.Writing;
                    }
                    Thread.Sleep(10);
                }
            }
            logger.Info("Converting task ended");
        }

        private void WriteToOutputStream(Stream outputStream)
        {
            logger.Info("Writing task started");
            while(!cts.IsCancellationRequested) {
                if(state == ConvertState.Writing) {
                    logger.Info("Start writing converted memory buffer to output file");
                    outputStream.Write(colorBuffer, 0, bufferByteCounter);
                    if(readDataEnding) {
                        cts.Cancel();
                    }
                    state = ConvertState.Reading;
                }
                else {
                    Thread.Sleep(10);
                }
            }
            logger.Info("Writing task ended");
        }
    }

    public class ConvertionProgressEventArgs : EventArgs
    {
        public byte ProgressPercent { get; }
        public bool Finished { get; }

        public ConvertionProgressEventArgs(byte progressPercent, bool finished)
        {
            ProgressPercent = progressPercent;
            Finished = finished;
        }

    }
}
