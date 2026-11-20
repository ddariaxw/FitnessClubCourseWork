namespace FitnessClub.Core.Exceptions
{

    public class TrainerScheduleConflictException : Exception
    {
        private Guid _trainerId;
        private DateTime _conflictStart;
        private DateTime _conflictEnd;

        public Guid TrainerId
        {
            get => _trainerId;
            private set => _trainerId = value;
        }

        public DateTime ConflictStart
        {
            get => _conflictStart;
            private set => _conflictStart = value;
        }

        public DateTime ConflictEnd
        {
            get => _conflictEnd;
            private set => _conflictEnd = value;
        }

        public TrainerScheduleConflictException(string message, Guid trainerId, DateTime conflictStart, DateTime conflictEnd)
            : base($"{message} (Інтервал: {conflictStart:dd.MM.yyyy HH:mm} - {conflictEnd:HH:mm})")
        {
            _trainerId = trainerId;
            _conflictStart = conflictStart;
            _conflictEnd = conflictEnd;
        }
    }
}