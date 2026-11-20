using FitnessClub.Core.Exceptions;
using FitnessClub.Core.Interfaces;
using FitnessClub.Core.Models;
using FitnessClub.Core.Repositories;

namespace FitnessClub.Core.Services
{

    public class FitnessClubService
    {
        private readonly IRepository<Member> _membersRepository;
        private readonly IRepository<Trainer> _trainersRepository;
        private readonly IRepository<Subscription> _subscriptionsRepository;
        private readonly IRepository<WorkoutSession> _sessionsRepository;

        private readonly JsonFileService<Member> _memberFileService;
        private readonly JsonFileService<Trainer> _trainerFileService;
        private readonly JsonFileService<Subscription> _subscriptionFileService;
        private readonly JsonFileService<WorkoutSession> _sessionFileService;

        public FitnessClubService(string dataDirectory = "Data")
        {
            if (string.IsNullOrWhiteSpace(dataDirectory))
                throw new ArgumentException("Шлях до каталогу даних не може бути порожнім.", nameof(dataDirectory));

            _memberFileService = new JsonFileService<Member>(Path.Combine(dataDirectory, "members.json"));
            _trainerFileService = new JsonFileService<Trainer>(Path.Combine(dataDirectory, "trainers.json"));
            _subscriptionFileService = new JsonFileService<Subscription>(Path.Combine(dataDirectory, "subscriptions.json"));
            _sessionFileService = new JsonFileService<WorkoutSession>(Path.Combine(dataDirectory, "sessions.json"));

            _membersRepository = new InMemoryRepository<Member>(_memberFileService.Load());
            _trainersRepository = new InMemoryRepository<Trainer>(_trainerFileService.Load());
            _subscriptionsRepository = new InMemoryRepository<Subscription>(_subscriptionFileService.Load());
            _sessionsRepository = new InMemoryRepository<WorkoutSession>(_sessionFileService.Load());
        }

        public void SaveAllData()
        {
            _memberFileService.Save(_membersRepository.GetAll());
            _trainerFileService.Save(_trainersRepository.GetAll());
            _subscriptionFileService.Save(_subscriptionsRepository.GetAll());
            _sessionFileService.Save(_sessionsRepository.GetAll());
        }

        public Member AddMember(string fullName, string phone)
        {
            var member = new Member(fullName, phone);
            _membersRepository.Add(member);
            return member;
        }

        public List<Member> GetMembersSortedByName()
        {
            var members = _membersRepository.GetAll();
            members.Sort();
            return members;
        }

        public Trainer AddTrainer(string fullName, string specialization)
        {
            var trainer = new Trainer(fullName, specialization);
            _trainersRepository.Add(trainer);
            return trainer;
        }

        public List<Trainer> GetAllTrainers()
        {
            return _trainersRepository.GetAll();
        }

        public Subscription IssueSubscription(Guid memberId, string title, DateTime startDate, DateTime endDate, int maxVisits)
        {
            bool memberExists = _membersRepository.Find(m => m.Id == memberId).Count > 0;
            if (!memberExists)
                throw new ArgumentException("Клієнта з вказаним ідентифікатором не знайдено.");

            var subscription = new Subscription(memberId, title, startDate, endDate, maxVisits);
            _subscriptionsRepository.Add(subscription);
            return subscription;
        }

        public List<Subscription> GetSubscriptionsByMember(Guid memberId)
        {
            return _subscriptionsRepository.Find(s => s.MemberId == memberId);
        }

        public List<Subscription> GetAllSubscriptions()
        {
            return _subscriptionsRepository.GetAll();
        }

        public WorkoutSession ScheduleWorkoutSession(string title, Guid trainerId, DateTime startTime, DateTime endTime, int capacity)
        {
            bool trainerExists = _trainersRepository.Find(t => t.Id == trainerId).Count > 0;
            if (!trainerExists)
                throw new ArgumentException("Тренера з вказаним ідентифікатором не знайдено.");

            var session = new WorkoutSession(title, trainerId, startTime, endTime, capacity);

            var trainerSessions = _sessionsRepository.Find(s => s.TrainerId == trainerId);
            foreach (var existing in trainerSessions)
            {
                bool overlaps = session.StartTime < existing.EndTime && session.EndTime > existing.StartTime;
                if (overlaps)
                {
                    throw new TrainerScheduleConflictException(
                        $"Тренер вже зайнятий на тренуванні «{existing.Title}».",
                        trainerId,
                        existing.StartTime,
                        existing.EndTime);
                }
            }

            _sessionsRepository.Add(session);
            return session;
        }

        public List<WorkoutSession> GetSessionsSortedByDate()
        {
            var sessions = _sessionsRepository.GetAll();
            sessions.Sort(new WorkoutSessionDateComparer());
            return sessions;
        }

        public void RegisterAttendance(Guid memberId, Guid subscriptionId, Guid sessionId, DateTime visitDate)
        {
            var members = _membersRepository.Find(m => m.Id == memberId);
            if (members.Count == 0)
                throw new ArgumentException("Клієнта не знайдено.");

            var subscriptions = _subscriptionsRepository.Find(s => s.Id == subscriptionId && s.MemberId == memberId);
            if (subscriptions.Count == 0)
                throw new ArgumentException("Абонемент для цього клієнта не знайдено.");

            var sessions = _sessionsRepository.Find(s => s.Id == sessionId);
            if (sessions.Count == 0)
                throw new ArgumentException("Групове тренування не знайдено.");

            Subscription subscription = subscriptions[0];
            WorkoutSession session = sessions[0];

            if (subscription.IsExpired(visitDate))
            {
                throw new SubscriptionExpiredException(
                    $"Термін дії абонемента «{subscription.Title}» закінчився.",
                    subscription.EndDate);
            }

            if (subscription.RemainingVisits <= 0)
            {
                throw new SubscriptionExpiredException(
                    $"Ліміт відвідувань за абонементом «{subscription.Title}» вичерпано.",
                    subscription.EndDate);
            }

            if (session.ParticipantIds.Count >= session.Capacity)
            {
                throw new InvalidOperationException("На обраному груповому тренуванні немає вільних місць.");
            }

            if (session.ParticipantIds.Contains(memberId))
            {
                throw new InvalidOperationException("Клієнт уже записаний на це тренування.");
            }

            subscription.IncrementVisits();
            session.ParticipantIds.Add(memberId);
        }
    }
}