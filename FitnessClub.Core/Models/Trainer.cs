namespace FitnessClub.Core.Models
{

    public class Trainer
    {
        private Guid _id;
        private string _fullName = string.Empty;
        private string _specialization = string.Empty;

        public Guid Id
        {
            get => _id;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Ідентифікатор тренера не може бути порожнім.");
                _id = value;
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("ПІБ тренера не може бути порожнім.");
                _fullName = value.Trim();
            }
        }

        public string Specialization
        {
            get => _specialization;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Спеціалізація тренера не може бути порожньою.");
                _specialization = value.Trim();
            }
        }

        public Trainer()
        {
            _id = Guid.NewGuid();
        }

        public Trainer(string fullName, string specialization) : this()
        {
            FullName = fullName;
            Specialization = specialization;
        }

        public override string ToString()
        {
            return $"[{Id.ToString()[..8]}] {_fullName} | Напрям: {_specialization}";
        }
    }
}