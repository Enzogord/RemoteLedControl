using System;
using System.Windows.Threading;

namespace Core.Sequence
{
    public class PlayerFactory
    {
        private readonly Dispatcher dispatcher;

        public PlayerFactory(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public SequencePlayer CreatePlayer()
        {
            var player = new SequencePlayer();
            player.Init(dispatcher);
            return player;
        }
    }
}
