using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FitnessTracker.Messages
{
    public class WorkoutChangedMessage : ValueChangedMessage<bool>
    {
        public WorkoutChangedMessage() : base(true) { }
    }
}
