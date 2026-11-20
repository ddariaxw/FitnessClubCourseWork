namespace FitnessClub.Core.Models
{

    public class WorkoutSession
    {
        private Guid _id;
        private string _title = string.Empty;
        private Guid _trainerId;
        private DateTime _startTime;
        private DateTime _endTime;
        private int _capacity;
        private List<Guid> _participantIds;

        public Guid Id
        {
            get => _id;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Ідентифікатор тренування не може бути порожнім.");
                _id = value;
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва тренування не може бути порожньою.");
                _title = value.Trim();
            }
        }

        public Guid TrainerId
        {
            get => _trainerId;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Ідентифікатор тренера не може бути порожнім.");
                _trainerId = value;
            }
        }

        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                if (value <= _startTime)
                    throw new ArgumentException("Час завершення тренування має бути пізнішим за час початку.");
                _endTime = value;
            }
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Місткість групи має бути більшою за 0.");
                _capacity = value;
            }
        }

        public List<Guid> ParticipantIds
        {
            get => _participantIds;
            set => _participantIds = value ?? throw new ArgumentNullException(nameof(value));
        }

        public WorkoutSession()
        {
            _id = Guid.NewGuid();
            _participantIds = new List<Guid>();
            _startTime = DateTime.Now.AddHours(1);
            _endTime = DateTime.Now.AddHours(2);
            _capacity = 10;
        }

        public WorkoutSession(string title, Guid trainerId, DateTime startTime, DateTime endTime, int capacity)
            : this()
        {
            Title = title;
            TrainerId = trainerId;
            _startTime = startTime;
            EndTime = endTime;
            Capacity = capacity;
        }

        public override string ToString()
        {
            return $"[{Id.ToString()[..8]}] {_title} | {_startTime:dd.MM.yyyy HH:mm} - {_endTime:HH:mm} | Учасників: {_participantIds.Count}/{_capacity}";
        }
    }
}