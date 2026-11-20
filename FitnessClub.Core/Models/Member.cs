namespace FitnessClub.Core.Models
{

    public class Member : IComparable<Member>
    {
        private Guid _id;
        private string _fullName = string.Empty;
        private string _phone = string.Empty;

        public Guid Id
        {
            get => _id;
            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Ідентифікатор клієнта не може бути порожнім.");
                _id = value;
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("ПІБ клієнта не може бути порожнім.");
                _fullName = value.Trim();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Номер телефону не може бути порожнім.");
                _phone = value.Trim();
            }
        }

        public Member()
        {
            _id = Guid.NewGuid();
        }

        public Member(string fullName, string phone) : this()
        {
            FullName = fullName;
            Phone = phone;
        }

        public int CompareTo(Member? other)
        {
            if (other == null)
                return 1;

            return string.Compare(_fullName, other._fullName, StringComparison.CurrentCultureIgnoreCase);
        }

        public override string ToString()
        {
            return $"[{Id.ToString()[..8]}] {_fullName} | Тел.: {_phone}";
        }
    }
}