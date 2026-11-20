namespace FitnessClub.Core.Exceptions
{

    public class SubscriptionExpiredException : Exception
    {
        private DateTime _expirationDate;

        public DateTime ExpirationDate
        {
            get => _expirationDate;
            private set => _expirationDate = value;
        }

        public SubscriptionExpiredException(string message, DateTime expirationDate)
            : base($"{message} (Дата завершення дії: {expirationDate:dd.MM.yyyy})")
        {
            _expirationDate = expirationDate;
        }
    }
}