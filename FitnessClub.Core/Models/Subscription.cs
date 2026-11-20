namespace FitnessClub.Core.Models
{

    public class Subscription
    {
        private Guid _id;
        private Guid _memberId;
        private string _title = string.Empty;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _maxVisits;
        private int _visitsUsed;

        public Guid Id
        {
            get => _id;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Ідентифікатор абонемента не може бути порожнім.");
                _id = value;
            }
        }

        public Guid MemberId
        {
            get => _memberId;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Ідентифікатор клієнта не може бути порожнім.");
                _memberId = value;
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва абонемента не може бути порожньою.");
                _title = value.Trim();
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value;
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (value < _startDate)
                    throw new ArgumentException("Дата завершення абонемента не може бути раніше дати початку.");
                _endDate = value;
            }
        }

        public int MaxVisits
        {
            get => _maxVisits;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Кількість доступних відвідувань має бути більшою за 0.");
                _maxVisits = value;
            }
        }

        public int VisitsUsed
        {
            get => _visitsUsed;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Кількість використаних відвідувань не може бути від'ємною.");
                if (_maxVisits > 0 && value > _maxVisits)
                    throw new ArgumentException("Кількість використаних відвідувань не може перевищувати ліміт.");
                _visitsUsed = value;
            }
        }

        public bool IsExpired(DateTime currentDate)
        {
            return currentDate.Date > _endDate.Date;
        }

        public int RemainingVisits => _maxVisits - _visitsUsed;

        public Subscription()
        {
            _id = Guid.NewGuid();
            _startDate = DateTime.Today;
            _endDate = DateTime.Today.AddMonths(1);
            _maxVisits = 8;
            _visitsUsed = 0;
        }

        public Subscription(Guid memberId, string title, DateTime startDate, DateTime endDate, int maxVisits)
            : this()
        {
            MemberId = memberId;
            Title = title;
            _startDate = startDate;
            EndDate = endDate;
            MaxVisits = maxVisits;
        }

        public void IncrementVisits()
        {
            if (_visitsUsed >= _maxVisits)
                throw new InvalidOperationException("Ліміт відвідувань за абонементом вичерпано.");
            _visitsUsed++;
        }

        public override string ToString()
        {
            string status = IsExpired(DateTime.Today) ? "ПРОСТРОЧЕНИЙ" : "АКТИВНИЙ";
            return $"[{Id.ToString()[..8]}] {_title} ({status}) | {_startDate:dd.MM.yyyy} - {_endDate:dd.MM.yyyy} | Відвідувань: {_visitsUsed}/{_maxVisits}";
        }
    }
}
