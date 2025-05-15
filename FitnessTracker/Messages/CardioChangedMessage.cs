using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FitnessTracker.Messages
{
    public class CardioChangedMessage : ValueChangedMessage<bool>
    {
        public CardioChangedMessage() : base(true) { }
    }
}
