using FitnessClub.Core.Models;

namespace FitnessClub.Core.Interfaces
{

    public class WorkoutSessionDateComparer : IComparer<WorkoutSession>
    {
        public int Compare(WorkoutSession? x, WorkoutSession? y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;

            return x.StartTime.CompareTo(y.StartTime);
        }
    }
}