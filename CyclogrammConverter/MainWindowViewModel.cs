using Core.CyclogrammUtility;
using NotifiedObjectsFramework;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CyclogrammConverter
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private string inputCyclogrammFileName;
        public string InputCyclogrammFileName {
            get => inputCyclogrammFileName;
            set {
                SetField(ref inputCyclogrammFileName, value);
                ConvertCommand.RaiseCanExecuteChanged();
            }
        }

        private string outputCyclogrammFileName;
        public string OutputCyclogrammFileName {
            get => outputCyclogrammFileName;
            set {
                SetField(ref outputCyclogrammFileName, value);
                ConvertCommand.RaiseCanExecuteChanged();
            }
        }

        private bool inProgress;
        public bool InProgress {
            get => inProgress;
            set {
                SetField(ref inProgress, value);
                ConvertCommand.RaiseCanExecuteChanged();
            }
        }

        private byte percent;
        public byte Percent {
            get => percent;
            set => SetField(ref percent, value);
        }


        #region OpenInputCyclogrammCommand

        public OpenDialogCommand openInputCyclogrammCommand;
        public OpenDialogCommand OpenInputCyclogrammCommand {
            get {
                if(openInputCyclogrammCommand == null) {
                    openInputCyclogrammCommand = new OpenDialogCommand(
                        (fileName) => {
                            InputCyclogrammFileName = fileName;
                        },
                        "CSV unconverted cyclogramms(.csv) | *.csv"
                    );
                }
                return openInputCyclogrammCommand;
            }
        }

        #endregion OpenInputCyclogrammCommand	

        #region SaveOutputCyclogrammCommand

        public SaveDialogCommand saveOutputCyclogrammCommand;
        public SaveDialogCommand SaveOutputCyclogrammCommand {
            get {
                if(saveOutputCyclogrammCommand == null) {
                    saveOutputCyclogrammCommand = new SaveDialogCommand(
                        (fileName) => {
                            OutputCyclogrammFileName = fileName;
                        },
                        "CSV converted cyclogramms(.cyc) | *.cyc"
                    );
                }
                return saveOutputCyclogrammCommand;
            }
        }

        #endregion SaveOutputCyclogrammCommand

        #region ConvertCommand

        public Command convertCommand;
        public Command ConvertCommand {
            get {
                if(convertCommand == null) {
                    convertCommand = new Command(
                        () => {
                            InProgress = true;
                            Task.Factory.StartNew(() => {
                                try {                                    
                                    using(Stream inStream = new FileStream(InputCyclogrammFileName, FileMode.Open, FileAccess.Read))
                                    using(Stream outStream = new FileStream(OutputCyclogrammFileName, FileMode.Create, FileAccess.ReadWrite)) {
                                        CyclogrammCsvToCycConverter converter = new CyclogrammCsvToCycConverter();
                                        converter.OnProgressChanged += (sender, e) => {
                                            Application.Current.Dispatcher.BeginInvoke(
                                                new Action(() => { 
                                                    Percent = e.ProgressPercent;
                                                    if(e.Finished) {
                                                        InProgress = false;
                                                    }
                                                })
                                            );
                                        };
                                        converter.Convert(inStream, outStream);
                                    }
                                }
                                catch(Exception ex) {
                                    MessageBox.Show("Ошибка!", ex.Message);
                                    Application.Current.Dispatcher.BeginInvoke(
                                        new Action(() => InProgress = false)
                                    );
                                    return;
                                }
                            });
                        },
                        () => !string.IsNullOrWhiteSpace(OutputCyclogrammFileName) && !string.IsNullOrWhiteSpace(InputCyclogrammFileName) && !InProgress
                    ); ;
                }
                return convertCommand;
            }
        }

        private void Converter_OnProgressChanged(object sender, ConvertionProgressEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion ConvertCommand	

    }
}
