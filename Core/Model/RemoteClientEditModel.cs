using Core.CyclogrammUtility;
using Core.Domain;
using Core.Infrastructure;
using Core.IO;
using Core.Messages;
using Microcontrollers;
using NLog;
using RLCCore.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Core.Model
{
    public class RemoteClientEditModel : ValidatableNotifierObjectBase
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IUniqueClientProvider uniqueClientProvider;
        private readonly SaveController saveController;

        private enum EditMode { Create, Edit }
        private EditMode editMode;

        public RemoteClient RemoteClient { get; private set; }

        public RemoteClientEditModel(RemoteClient remoteClient, IUniqueClientProvider uniqueClientProvider, SaveController saveController)
        {
            RemoteClient = remoteClient ?? throw new ArgumentNullException(nameof(remoteClient));
            this.uniqueClientProvider = uniqueClientProvider ?? throw new ArgumentNullException(nameof(uniqueClientProvider));
            this.saveController = saveController ?? throw new ArgumentNullException(nameof(saveController));
            Initialize(EditMode.Edit);
        }

        public RemoteClientEditModel(IUniqueClientProvider uniqueClientProvider, SaveController saveController)
        {
            this.uniqueClientProvider = uniqueClientProvider ?? throw new ArgumentNullException(nameof(uniqueClientProvider));
            this.saveController = saveController ?? throw new ArgumentNullException(nameof(saveController));            
            Initialize(EditMode.Create);
        }

        private void Initialize(EditMode editMode)
        {
            this.editMode = editMode;
            if(editMode == EditMode.Create) {
                int newClientNumber = uniqueClientProvider.GenerateClientNumber();
                RemoteClient = new RemoteClient("Новый клиент", newClientNumber);
            }
            MicrocontrollerUnit = new MicrocontrollerUnit();

        }

        private IEnumerable<IMicrocontroller> availableMicrocontrollers;
        public IEnumerable<IMicrocontroller> AvailableMicrocontrollers {
            get {
                if(availableMicrocontrollers == null) {
                    availableMicrocontrollers = MicrocontrollersLibrary.GetMicrocontrollers();
                }
                return availableMicrocontrollers;
            }
        }

        private string name;
        public string Name {
            get => name;
            set {
                if(SetField(ref name, value)) {
                    NotifyProperties();
                }
            }
        }

        private int number;
        public int Number {
            get => number;
            set {
                if(ValidatableSetField(ref number, value)) {
                    NotifyProperties();
                }
            }
        }

        private bool isDigitalPWMSignal;
        public bool IsDigitalPWMSignal {
            get => isDigitalPWMSignal;
            set {
                if(SetField(ref isDigitalPWMSignal, value)) {
                    NotifyProperties();
                }
            }
        }

        private bool isInvertedPWMSignal;
        public bool IsInvertedPWMSignal {
            get => isInvertedPWMSignal;
            set {
                if(SetField(ref isInvertedPWMSignal, value)) {
                    NotifyProperties();
                }
            }
        }

        private bool defaultLight;
        public bool DefaultLight {
            get => defaultLight;
            set {
                if(SetField(ref defaultLight, value)) {
                    NotifyProperties();
                }
            }
        }

        private byte spiLedGlobalBrightnessPercent;
        public byte SPILedGlobalBrightnessPercent {
            get => spiLedGlobalBrightnessPercent;
            set {
                if(SetField(ref spiLedGlobalBrightnessPercent, value)) {
                    spiLedGlobalBrightness = (byte)(Math.Round((float)spiLedGlobalBrightnessPercent / (float)100 * (float)255));
                    OnPropertyChanged(nameof(SPILedGlobalBrightness));
                    NotifyProperties();
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
                    NotifyProperties();
                }
            }
        }

        private Cyclogramm cyclogramm;
        public Cyclogramm Cyclogramm {
            get => cyclogramm;
            set {
                if(SetField(ref cyclogramm, value)) {
                    CreateNotificationBinding().AddAction(NotifyProperties).SetNotifier(Cyclogramm)
                        .BindToProperty(x => x.Name)
                        .End();
                }
            }
        }

        private string unconvertedCyclogrammFile;
        public string UnconvertedCyclogrammFile {
            get => unconvertedCyclogrammFile;
            set {
                if(ValidatableSetField(ref unconvertedCyclogrammFile, value)) {
                    NotifyProperties();
                }
            }
        }

        public bool ConvertedCyclogrammExists {
            get {
                string clientPath = saveController.GetClientFolder(RemoteClient.Number, RemoteClient.Name);
                return File.Exists(Path.Combine(clientPath, "Data.cyc"));
            }
        }

        private MicrocontrollerUnit microcontrollerUnit;
        public MicrocontrollerUnit MicrocontrollerUnit {
            get => microcontrollerUnit;
            set {
                if(SetField(ref microcontrollerUnit, value)) {
                    CreateNotificationBinding().AddAction(NotifyProperties).SetNotifier(MicrocontrollerUnit)
                        .BindToProperty(x => x.Microcontroller)
                        .BindToProperty(x => x.MicrocontrollerId)
                        .BindToProperty(x => x.ZeroCharge)
                        .BindToProperty(x => x.FullCharge)
                        .End();
                }
            }
        }

        private ClientState clientState;
        public ClientState ClientState {
            get => clientState;
            set {
                if(SetField(ref clientState, value)) {
                    NotifyProperties();
                }
            }
        }

        private byte batteryChargeLevel;
        public byte BatteryChargeLevel {
            get => batteryChargeLevel;
            set {
                if(SetField(ref batteryChargeLevel, value)) {
                    NotifyProperties();
                }
            }
        }

        public void Load()
        {
            Name = RemoteClient.Name;
            Number = RemoteClient.Number;
            IsDigitalPWMSignal = RemoteClient.IsDigitalPWMSignal;
            IsInvertedPWMSignal = RemoteClient.IsInvertedPWMSignal;
            DefaultLight = RemoteClient.DefaultLight;
            SPILedGlobalBrightness = RemoteClient.SPILedGlobalBrightness;
            LoadMicrocontroller();
            LoadCyclogramm();
            ClientState = RemoteClient.ClientState;
            BatteryChargeLevel = RemoteClient.BatteryChargeLevel;
        }

        private MicrocontrollerUnit backupMicrocontrollerUnit;
        private void LoadMicrocontroller()
        {
            backupMicrocontrollerUnit = RemoteClient.MicrocontrollerUnit;
            MicrocontrollerUnit.MicrocontrollerId = RemoteClient.MicrocontrollerUnit.MicrocontrollerId;
            MicrocontrollerUnit.ZeroCharge = RemoteClient.MicrocontrollerUnit.ZeroCharge;
            MicrocontrollerUnit.FullCharge = RemoteClient.MicrocontrollerUnit.FullCharge;
        }

        private Cyclogramm backupCyclogramm;

        private void LoadCyclogramm()
        {
            Cyclogramm = new Cyclogramm();
            if(RemoteClient.Cyclogramm == null) {
                return;
            }
            backupCyclogramm = RemoteClient.Cyclogramm;
            Cyclogramm.Name = RemoteClient.Cyclogramm.Name;
        }

        public void Commit()
        {
            if(!CanCommit) {
                return;
            }

            var backupClientNumber = RemoteClient.Number;
            var backupClientName = RemoteClient.Name;
            var backupClientIsDigitalPWMSignal = RemoteClient.IsDigitalPWMSignal;
            var backupClientIsInvertedPWMSignal = RemoteClient.IsInvertedPWMSignal;
            var backupClientDefaultLight = RemoteClient.DefaultLight;
            var backupClientSPILedGlobalBrightness = RemoteClient.SPILedGlobalBrightness;
            var backupClientClientState = RemoteClient.ClientState;
            var backupClientBatteryChargeLevel = RemoteClient.BatteryChargeLevel;
            bool clientDirMoved = false;
            bool clientDirCreated = false;
            string clientRootPath = "";
            string clientWorkPath = "";

            try {
                clientWorkPath = saveController.GetClientFolder(RemoteClient.Number, RemoteClient.Name);
                if(ClientUniqNameChanged && Directory.Exists(clientWorkPath)) {
                    clientRootPath = Directory.GetParent(clientWorkPath).FullName;
                    Directory.Move(clientWorkPath, Path.Combine(clientRootPath, $"{Number}_{Name}"));
                    clientDirMoved = true;
                }

                clientWorkPath = saveController.GetClientFolder(Number, Name);
                if(!Directory.Exists(clientWorkPath)) {
                    Directory.CreateDirectory(clientWorkPath);
                    clientDirCreated = true;
                }

                if(!string.IsNullOrWhiteSpace(UnconvertedCyclogrammFile)) {
                    string fullSavePath = Path.Combine(clientWorkPath, "Data.cyc");
                    CyclogrammCsvToCycConverter converter = new CyclogrammCsvToCycConverter();
                    using(FileStream sourceStream = new FileStream(UnconvertedCyclogrammFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using(FileStream destinationStream = new FileStream(fullSavePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)) {
                        converter.Convert(sourceStream, destinationStream);
                    }
                    UnconvertedCyclogrammFile = null;
                }

                RemoteClient.Name = Name;
                RemoteClient.Number = Number;
                RemoteClient.IsDigitalPWMSignal = IsDigitalPWMSignal;
                RemoteClient.IsInvertedPWMSignal = IsInvertedPWMSignal;
                RemoteClient.DefaultLight = DefaultLight;
                RemoteClient.SPILedGlobalBrightness = SPILedGlobalBrightness;
                RemoteClient.ClientState = ClientState;
                RemoteClient.BatteryChargeLevel = BatteryChargeLevel;

                SaveMicrocontroller();
                SaveCyclogramm();
            }
            catch(Exception ex) {
                logger.Error(ex, "Ошибка при сохранении клиента");

                RemoteClient.Name = backupClientName;
                RemoteClient.Number = backupClientNumber;
                RemoteClient.IsDigitalPWMSignal = backupClientIsDigitalPWMSignal;
                RemoteClient.IsInvertedPWMSignal = backupClientIsInvertedPWMSignal;
                RemoteClient.DefaultLight = backupClientDefaultLight;
                RemoteClient.SPILedGlobalBrightness = backupClientSPILedGlobalBrightness;
                RemoteClient.ClientState = backupClientClientState;
                RemoteClient.BatteryChargeLevel = backupClientBatteryChargeLevel;

                RemoteClient.MicrocontrollerUnit = backupMicrocontrollerUnit;
                RemoteClient.Cyclogramm = backupCyclogramm;
                if(clientDirMoved) {
                    Directory.Move(clientWorkPath, Path.Combine(clientRootPath, $"{RemoteClient.Number}_{RemoteClient.Name}"));
                }
                else if(clientDirCreated && Directory.Exists(clientWorkPath)) {
                    Directory.Delete(clientWorkPath);
                }
            }
        }

        private void SaveMicrocontroller()
        {
            RemoteClient.MicrocontrollerUnit = MicrocontrollerUnit;
        }

        private void SaveCyclogramm()
        {
            RemoteClient.Cyclogramm = Cyclogramm;
        }

        private void NotifyProperties()
        {
            OnPropertyChanged(nameof(HasChange));
            OnPropertyChanged(nameof(CanCommit));
        }

        public bool ClientUniqNameChanged => Name != RemoteClient.Name || Number != RemoteClient.Number;

        public bool HasChange {
            get {
                return Name != RemoteClient.Name
                || Number != RemoteClient.Number
                || IsDigitalPWMSignal != RemoteClient.IsDigitalPWMSignal
                || IsInvertedPWMSignal != RemoteClient.IsInvertedPWMSignal
                || DefaultLight != RemoteClient.DefaultLight
                || SPILedGlobalBrightness != RemoteClient.SPILedGlobalBrightness
                || MicrocontrollerUnit.MicrocontrollerId != RemoteClient.MicrocontrollerUnit.MicrocontrollerId
                || MicrocontrollerUnit.ZeroCharge != RemoteClient.MicrocontrollerUnit.ZeroCharge
                || MicrocontrollerUnit.FullCharge != RemoteClient.MicrocontrollerUnit.FullCharge
                || Cyclogramm.Name != RemoteClient.Cyclogramm.Name
                || ClientState != RemoteClient.ClientState
                || BatteryChargeLevel != RemoteClient.BatteryChargeLevel;
            }
        }

        public bool CanCommit {
            get {
                if(HasErrors || MicrocontrollerUnit.HasErrors) {
                    return false;
                }
                return true;
            }
        }

        protected override IEnumerable<ValidationResult> ValidateWithDataErrorNotification(ValidationContext validationContext)
        {
            if((editMode == EditMode.Create || (editMode == EditMode.Edit && Number != RemoteClient.Number)) && uniqueClientProvider.ClientExists(Number)) {
                yield return new PropertyValidationResult("Клиент с таким номером уже существует", nameof(Number));
            }

            if(!ConvertedCyclogrammExists && string.IsNullOrWhiteSpace(UnconvertedCyclogrammFile)) {
                yield return new PropertyValidationResult("К клиенту не добавлена циклограмма, необходимо выбрать файл с несконвертированной циклограммой", nameof(UnconvertedCyclogrammFile));
            }
            
            if(MicrocontrollerUnit != null) {
                MicrocontrollerUnit.Validate();
            }
        }
    }
}
