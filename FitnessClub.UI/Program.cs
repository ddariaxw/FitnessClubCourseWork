using System.Globalization;
using System.Text;
using FitnessClub.Core.Exceptions;
using FitnessClub.Core.Models;
using FitnessClub.Core.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var clubService = new FitnessClubService("Data");

Console.WriteLine("=== Система обліку спортивного клубу та абонементів (Варіант №8) ===");
Console.WriteLine("Дані автоматично завантажено (якщо файл існує).\n");

bool isRunning = true;

while (isRunning)
{
    PrintMenu();
    int choice = ReadInt("Оберіть пункт меню: ", 0, 10);
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case 1:
                AddMemberFlow(clubService);
                break;
            case 2:
                ShowMembersFlow(clubService);
                break;
            case 3:
                AddTrainerFlow(clubService);
                break;
            case 4:
                ShowTrainersFlow(clubService);
                break;
            case 5:
                IssueSubscriptionFlow(clubService);
                break;
            case 6:
                ShowSubscriptionsFlow(clubService);
                break;
            case 7:
                ScheduleSessionFlow(clubService);
                break;
            case 8:
                ShowSessionsFlow(clubService);
                break;
            case 9:
                RegisterAttendanceFlow(clubService);
                break;
            case 10:
                clubService.SaveAllData();
                Console.WriteLine("Усі дані успішно збережено у JSON-файли.");
                break;
            case 0:
                clubService.SaveAllData();
                Console.WriteLine("Дані збережено. Роботу завершено.");
                isRunning = false;
                break;
        }
    }
    catch (SubscriptionExpiredException ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[Помилка абонемента]: {ex.Message}");
        Console.ResetColor();
    }
    catch (TrainerScheduleConflictException ex)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[Конфлікт розкладу тренера]: {ex.Message}");
        Console.ResetColor();
    }
    catch (ArgumentException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Помилка даних]: {ex.Message}");
        Console.ResetColor();
    }
    catch (InvalidOperationException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Помилка операції]: {ex.Message}");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Непередбачена помилка]: {ex.Message}");
        Console.ResetColor();
    }

    if (isRunning)
    {
        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey(intercept: true);
        Console.Clear();
    }
}

static void PrintMenu()
{
    Console.WriteLine("1. Додати клієнта");
    Console.WriteLine("2. Переглянути всіх клієнтів (сортування за ПІБ)");
    Console.WriteLine("3. Додати тренера");
    Console.WriteLine("4. Переглянути всіх тренерів");
    Console.WriteLine("5. Оформити абонемент клієнту");
    Console.WriteLine("6. Переглянути абонементи (всі або за клієнтом)");
    Console.WriteLine("7. Запланувати групове тренування");
    Console.WriteLine("8. Переглянути розклад тренувань (сортування за датою)");
    Console.WriteLine("9. Записати клієнта на тренування / зафіксувати відвідування");
    Console.WriteLine("10. Зберегти дані у JSON зараз");
    Console.WriteLine("0. Зберегти та вийти");
    Console.WriteLine();
}

static void AddMemberFlow(FitnessClubService service)
{
    string fullName = ReadNonEmptyString("Введіть ПІБ клієнта: ");
    string phone = ReadNonEmptyString("Введіть телефон клієнта: ");

    Member member = service.AddMember(fullName, phone);
    Console.WriteLine($"Клієнта додано: {member}");
}

static void ShowMembersFlow(FitnessClubService service)
{
    List<Member> members = service.GetMembersSortedByName();
    if (members.Count == 0)
    {
        Console.WriteLine("Список клієнтів порожній.");
        return;
    }

    Console.WriteLine("--- Список клієнтів ---");
    for (int i = 0; i < members.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {members[i]}");
    }
}

static void AddTrainerFlow(FitnessClubService service)
{
    string fullName = ReadNonEmptyString("Введіть ПІБ тренера: ");
    string specialization = ReadNonEmptyString("Введіть спеціалізацію (наприклад, Силові, Йога, Пілатес): ");

    Trainer trainer = service.AddTrainer(fullName, specialization);
    Console.WriteLine($"Тренера додано: {trainer}");
}

static void ShowTrainersFlow(FitnessClubService service)
{
    List<Trainer> trainers = service.GetAllTrainers();
    if (trainers.Count == 0)
    {
        Console.WriteLine("Список тренерів порожній.");
        return;
    }

    Console.WriteLine("--- Список тренерів ---");
    for (int i = 0; i < trainers.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {trainers[i]}");
    }
}

static void IssueSubscriptionFlow(FitnessClubService service)
{
    List<Member> members = service.GetMembersSortedByName();
    if (members.Count == 0)
    {
        Console.WriteLine("Спочатку додайте хоча б одного клієнта.");
        return;
    }

    Console.WriteLine("Оберіть клієнта:");
    for (int i = 0; i < members.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {members[i]}");
    }

    int index = ReadInt("Номер клієнта: ", 1, members.Count) - 1;
    Guid memberId = members[index].Id;

    string title = ReadNonEmptyString("Назва абонемента (наприклад, Місячний / 8 занять): ");
    DateTime startDate = ReadDate("Дата початку (дд.мм.рррр): ");
    DateTime endDate = ReadDate("Дата завершення (дд.мм.рррр): ");
    int maxVisits = ReadInt("Кількість відвідувань за абонементом: ", 1, 365);

    Subscription subscription = service.IssueSubscription(memberId, title, startDate, endDate, maxVisits);
    Console.WriteLine($"Абонемент оформлено: {subscription}");
}

static void ShowSubscriptionsFlow(FitnessClubService service)
{
    List<Member> members = service.GetMembersSortedByName();
    List<Subscription> subscriptions;

    if (members.Count > 0)
    {
        Console.WriteLine("0. Показати всі абонементи");
        for (int i = 0; i < members.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Фільтр за клієнтом: {members[i].FullName}");
        }

        int choice = ReadInt("Оберіть варіант: ", 0, members.Count);
        subscriptions = choice == 0
            ? service.GetAllSubscriptions()
            : service.GetSubscriptionsByMember(members[choice - 1].Id);
    }
    else
    {
        subscriptions = service.GetAllSubscriptions();
    }

    if (subscriptions.Count == 0)
    {
        Console.WriteLine("Абонементів не знайдено.");
        return;
    }

    Console.WriteLine("--- Список абонементів ---");
    for (int i = 0; i < subscriptions.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {subscriptions[i]}");
    }
}

static void ScheduleSessionFlow(FitnessClubService service)
{
    List<Trainer> trainers = service.GetAllTrainers();
    if (trainers.Count == 0)
    {
        Console.WriteLine("Спочатку додайте хоча б одного тренера.");
        return;
    }

    Console.WriteLine("Оберіть тренера:");
    for (int i = 0; i < trainers.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {trainers[i]}");
    }

    int index = ReadInt("Номер тренера: ", 1, trainers.Count) - 1;
    Guid trainerId = trainers[index].Id;

    string title = ReadNonEmptyString("Назва групового тренування: ");
    DateTime startTime = ReadDateTime("Дата та час початку (дд.мм.рррр гг:хх): ");
    DateTime endTime = ReadDateTime("Дата та час завершення (дд.мм.рррр гг:хх): ");
    int capacity = ReadInt("Максимальна кількість учасників: ", 1, 100);

    WorkoutSession session = service.ScheduleWorkoutSession(title, trainerId, startTime, endTime, capacity);
    Console.WriteLine($"Тренування додано до розкладу: {session}");
}

static void ShowSessionsFlow(FitnessClubService service)
{
    List<WorkoutSession> sessions = service.GetSessionsSortedByDate();
    if (sessions.Count == 0)
    {
        Console.WriteLine("Розклад тренувань порожній.");
        return;
    }

    Console.WriteLine("--- Розклад групових тренувань ---");
    for (int i = 0; i < sessions.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {sessions[i]}");
    }
}

static void RegisterAttendanceFlow(FitnessClubService service)
{
    List<Member> members = service.GetMembersSortedByName();
    List<WorkoutSession> sessions = service.GetSessionsSortedByDate();

    if (members.Count == 0)
    {
        Console.WriteLine("Немає зареєстрованих клієнтів.");
        return;
    }

    if (sessions.Count == 0)
    {
        Console.WriteLine("Немає запланованих групових тренувань.");
        return;
    }

    Console.WriteLine("Оберіть клієнта:");
    for (int i = 0; i < members.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {members[i]}");
    }
    Member selectedMember = members[ReadInt("Номер клієнта: ", 1, members.Count) - 1];

    List<Subscription> memberSubscriptions = service.GetSubscriptionsByMember(selectedMember.Id);
    if (memberSubscriptions.Count == 0)
    {
        Console.WriteLine("У обраного клієнта немає оформлених абонементів.");
        return;
    }

    Console.WriteLine("Оберіть абонемент клієнта:");
    for (int i = 0; i < memberSubscriptions.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {memberSubscriptions[i]}");
    }
    Subscription selectedSubscription = memberSubscriptions[ReadInt("Номер абонемента: ", 1, memberSubscriptions.Count) - 1];

    Console.WriteLine("Оберіть групове тренування:");
    for (int i = 0; i < sessions.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {sessions[i]}");
    }
    WorkoutSession selectedSession = sessions[ReadInt("Номер тренування: ", 1, sessions.Count) - 1];

    DateTime visitDate = ReadDate("Дата відвідування (дд.мм.рррр): ");

    service.RegisterAttendance(selectedMember.Id, selectedSubscription.Id, selectedSession.Id, visitDate);
    Console.WriteLine("Відвідування успішно зафіксовано! Кількість використаних занять оновлено.");
}

static int ReadInt(string prompt, int min, int max)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int value) && value >= min && value <= max)
            return value;

        Console.WriteLine($"Помилка! Введіть ціле число в межах від {min} до {max}.");
    }
}

static string ReadNonEmptyString(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
            return input.Trim();

        Console.WriteLine("Помилка! Значення не може бути порожнім.");
    }
}

static DateTime ReadDate(string prompt)
{
    string[] formats = { "dd.MM.yyyy", "d.M.yyyy" };

    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (DateTime.TryParseExact(input?.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            return date.Date;

        Console.WriteLine("Помилка! Введіть коректну дату у форматі дд.мм.рррр (наприклад, 19.11.2026).");
    }
}

static DateTime ReadDateTime(string prompt)
{
    string[] formats = { "dd.MM.yyyy HH:mm", "d.M.yyyy HH:mm", "dd.MM.yyyy H:mm", "d.M.yyyy H:mm" };

    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (DateTime.TryParseExact(input?.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            return dateTime;

        Console.WriteLine("Помилка! Введіть дату та час у форматі дд.мм.рррр гг:хх (наприклад, 19.11.2026 18:00).");
    }
}