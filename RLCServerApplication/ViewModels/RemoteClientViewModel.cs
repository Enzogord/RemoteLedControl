using System;
using System.IO;
using Core.CyclogrammUtility;
using Core.IO;
using RLCCore.Domain;
using RLCServerApplication.Infrastructure;
using RLCServerApplication.Infrastructure.Command;

namespace RLCServerApplication.ViewModels
{
    public class RemoteClientEditorCloseEventArgs : EventArgs
    {
        public bool Commited { get; }

        public RemoteClientEditorCloseEventArgs(bool commited)
        {
            Commited = commited;
        }
    }

    public class RemoteClientViewModel : ViewModelBase
    {
        public RemoteClient RemoteClient { get; }

        public event EventHandler<RemoteClientEditorCloseEventArgs> OnClose;

        public RemoteClientViewModel(RemoteClient remoteClient, RemoteControlProject project, SaveController saveController, FileHolder fileHolder)
        {
            this.RemoteClient = remoteClient ?? throw new ArgumentNullException(nameof(remoteClient));
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            this.saveController = saveController ?? throw new ArgumentNullException(nameof(saveController));
            this.fileHolder = fileHolder ?? throw new ArgumentNullException(nameof(fileHolder));
            LoadValues();
            CreateCommands();
        }

        private int number;
        public int Number {
            get => number;
            set {
                SetField(ref number, value);
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        private string name;
        public string Name {
            get => name;
            set {
                SetField(ref name, value);
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        private Cyclogramm cyclogramm;
        public Cyclogramm Cyclogramm {
            get => cyclogramm;
            set {
                SetField(ref cyclogramm, value);
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        private bool isDigitalPWMSignal;
        public bool IsDigitalPWMSignal {
            get => isDigitalPWMSignal;
            set {
                SetField(ref isDigitalPWMSignal, value);
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        private bool isInvertedPWMSignal;
        public bool IsInvertedPWMSignal {
            get => isInvertedPWMSignal;
            set {
                SetField(ref isInvertedPWMSignal, value);
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        private bool defaultLight;
        public bool DefaultLight {
            get => defaultLight;
            set {
                SetField(ref defaultLight, value);
                OnPropertyChanged(nameof(HasChanges));
            }
        }

        private byte spiLedGlobalBrightnessPercent;
        public byte SPILedGlobalBrightnessPercent {
            get => spiLedGlobalBrightnessPercent;
            set {
                if(SetField(ref spiLedGlobalBrightnessPercent, value)) {
                    spiLedGlobalBrightness = (byte)(Math.Round((float)spiLedGlobalBrightnessPercent / (float)100 * (float)255));
                    OnPropertyChanged(nameof(SPILedGlobalBrightness));
                    OnPropertyChanged(nameof(HasChanges));
                }
            }
        }

        private byte spiLedGlobalBrightness;
        public byte SPILedGlobalBrightness {
            get => spiLedGlobalBrightness;
            set {
                if(SetField(ref spiLedGlobalBrightness, value)) {
                    spiLedGlobalBrightnessPercent = (byte)(Math.Round((float)spiLedGlobalBrightness / (float)255 * (float)100));
                    OnPropertyChanged(nameof(SPILedGlobalBrightnessPercent));
                    OnPropertyChanged(nameof(HasChanges));
                }
            }
        }

        private CyclogrammViewModel cyclogrammViewModel;
        private readonly RemoteControlProject project;
        private readonly SaveController saveController;
        private readonly FileHolder fileHolder;

        public CyclogrammViewModel CyclogrammViewModel {
            get => cyclogrammViewModel;
            set => SetField(ref cyclogrammViewModel, value, () => CyclogrammViewModel);
        }

        private void LoadValues()
        {
            Number = RemoteClient.Number;
            Name = RemoteClient.Name;
            IsDigitalPWMSignal = RemoteClient.IsDigitalPWMSignal;
            IsInvertedPWMSignal = RemoteClient.IsInvertedPWMSignal;
            DefaultLight = RemoteClient.DefaultLight;
            SPILedGlobalBrightness = RemoteClient.SPILedGlobalBrightness;
            if(RemoteClient.Cyclogramm == null) {
                Cyclogramm = new Cyclogramm();
            } else {
                Cyclogramm = RemoteClient.Cyclogramm;
            }
            CyclogrammViewModel = new CyclogrammViewModel(Cyclogramm, RemoteClient, saveController);
        }

        public bool HasClientNameChanged => Number != RemoteClient.Number
            || Name != RemoteClient.Name;

        public bool HasChanges => HasClientNameChanged
            || IsDigitalPWMSignal != RemoteClient.IsDigitalPWMSignal
            || IsInvertedPWMSignal != RemoteClient.IsInvertedPWMSignal
            || DefaultLight != RemoteClient.DefaultLight
            || SPILedGlobalBrightness != RemoteClient.SPILedGlobalBrightness;

        private void CommitChanges()
        {
            if(string.IsNullOrWhiteSpace(Name)) {
                return;
            }
            string clientWorkPath = saveController.GetClientFolder(RemoteClient.Number, RemoteClient.Name);            

            if((Name != RemoteClient.Name || Number != RemoteClient.Number) && Directory.Exists(clientWorkPath)) {
                Directory.Move(clientWorkPath, Path.Combine(Directory.GetParent(clientWorkPath).FullName, $"{Number}_{Name}"));
            }
            
            clientWorkPath = saveController.GetClientFolder(Number, Name);
            if(!Directory.Exists(clientWorkPath)) {
                Directory.CreateDirectory(clientWorkPath);
            }

            RemoteClient.Number = Number;
            RemoteClient.Name = Name;
            RemoteClient.Cyclogramm = Cyclogramm;
            RemoteClient.IsDigitalPWMSignal = IsDigitalPWMSignal;
            RemoteClient.IsInvertedPWMSignal = IsInvertedPWMSignal;
            RemoteClient.DefaultLight = DefaultLight;
            RemoteClient.SPILedGlobalBrightness = SPILedGlobalBrightness;

            if(!string.IsNullOrWhiteSpace(Cyclogramm.FilePath)) {
                string fullSavePath = Path.Combine(clientWorkPath, "Data.cyc");
                CyclogrammCsvToCycConverter converter = new CyclogrammCsvToCycConverter();
                using(FileStream sourceStream = new FileStream(Cyclogramm.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using(FileStream destinationStream = new FileStream(fullSavePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) {
                    converter.Convert(sourceStream, destinationStream);
                }
                Cyclogramm.FilePath = null;
            }
            
            OnClose?.Invoke(this, new RemoteClientEditorCloseEventArgs(true));
        }

        private void Discard()
        {
            OnClose?.Invoke(this, new RemoteClientEditorCloseEventArgs(false));
        }

        #region Commands

        public DelegateCommand SaveChangesCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }

        public void CreateCommands()
        {
            SaveChangesCommand = new DelegateCommand(
                () => {
                    CommitChanges();
                },
                () => {
                    return Number > 0
                        && !string.IsNullOrWhiteSpace(Name)
                        && (
                            (!project.ClientExists(Number, Name) && HasClientNameChanged)
                            || !string.IsNullOrWhiteSpace(Cyclogramm.FilePath)
                            || (HasChanges && !HasClientNameChanged)
                        )
                        && (!string.IsNullOrWhiteSpace(Cyclogramm.FilePath) || CyclogrammViewModel.ConvertedCyclogrammExists);
                } 
            );
            SaveChangesCommand.CanExecuteChangedWith(Cyclogramm, x => x.FilePath);
            SaveChangesCommand.CanExecuteChangedWith(this, x => x.HasChanges);

            CloseCommand = new DelegateCommand(
                () => { Discard(); },
                () => true
            );
        }

        #endregion
    }
}
