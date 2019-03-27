using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLCCore.Transport
{
    public class PackageBuilder
    {
        public static PackageBuilder CreatePackage(int projectKey)
        {
            return new PackageBuilder(projectKey);
        }

        private const int packageMaxSize = 200;

        byte[] package;
        private byte command;

        private PackageBuilder(int projectKey)
        {
            if(projectKey == 0) {
                throw new ArgumentException("Ключ должен быть указан");
            }
            package = new byte[packageMaxSize];
            InsertProjectKey(projectKey);
        }

        private void InsertProjectKey(int projectKey)
        {
            package[0] = (byte)(projectKey >> 24);
            package[1] = (byte)(projectKey >> 16);
            package[2] = (byte)(projectKey >> 8);
            package[3] = (byte)(projectKey >> 0);
        }

        public PackageBuilder InsertCommand(ClientCommands command, ICommandContext commandContext)
        {
            InsertCommand((byte)command, commandContext.GetBytes());
            return this;
        }

        public PackageBuilder InsertCommand(GlobalCommands command, ICommandContext commandContext)
        {
            InsertCommand((byte)command, commandContext.GetBytes());
            return this;
        }

        private void InsertCommand(byte command, byte[] contextBytes)
        {
            if(contextBytes.Length > packageMaxSize - 5) {
                throw new ArgumentException($"Параметр \"{nameof(contextBytes)}\" имеет слишком большую длину");
            }
            this.command = command;
            package[4] = command;
            contextBytes.CopyTo(package, 5);
        }

        public byte[] GetPackage()
        {
            if(command == 0) {
                throw new InvalidOperationException("Пакет не сконфигурирован до конца, не добавлена команда");
            }
            return package;
        }
    }
}
