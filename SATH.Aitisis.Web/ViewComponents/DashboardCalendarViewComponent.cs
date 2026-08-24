using Microsoft.AspNetCore.Mvc;

namespace SATH.Aitisis.Web.ViewComponents
{
    public sealed class DashboardCalendarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year,today.Month,1);

            var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

            var leadingEmptyDays = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

            var days = new List<DashboardCalendarDayViewModel>();

            for (var i = 0; i < leadingEmptyDays; i++)
            {
                days.Add(
                    new DashboardCalendarDayViewModel
                    {
                        IsEmpty = true
                    });
            }

            for (var day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(today.Year, today.Month, day);

                days.Add(
                    new DashboardCalendarDayViewModel
                    {
                        DayNumber = day,
                        Date = date,
                        IsToday = date.Date == today.Date,
                        IsEmpty = false
                    });
            }

            var model = new DashboardCalendarViewModel
            {
                MonthName = today.ToString("MMMM yyyy"),
                TodayText = today.ToString("dd/MM/yyyy"),
                Days = days
            };

            return View(model);
        }
    }

    public sealed class DashboardCalendarViewModel
    {
        public string MonthName { get; init; } = string.Empty;

        public string TodayText { get; init; } = string.Empty;

        public IReadOnlyCollection<DashboardCalendarDayViewModel> Days { get; init; } = Array.Empty<DashboardCalendarDayViewModel>();
    }


    public sealed class DashboardCalendarDayViewModel
    {
        public int DayNumber { get; init; }

        public DateTime Date { get; init; }

        public bool IsToday { get; init; }

        public bool IsEmpty { get; init; }
    }
}
