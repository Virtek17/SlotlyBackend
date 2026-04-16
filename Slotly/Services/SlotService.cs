using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.DTOs.Slot;
using Slotly.Entities;
using System.Security;

namespace Slotly.Services
{
    public class SlotService
    {
        private readonly SlotlyContext _context;

        public SlotService(SlotlyContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlotDto>> GetAvailabeSlots(
            Guid staffId, Guid staffServiceId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;

            // 1. Рабочие часы
            var workingHours = await _context.WorkingHours
                .Where(w => w.StaffId == staffId && w.DayOfWeek == dayOfWeek)
                .ToListAsync();

            if (!workingHours.Any())
                return new List<TimeSlotDto>();

            // 2. Записи на день
            var dayStart = date.Date;
            var dayEnd = date.Date.AddDays(1);

            var appointments =await _context.Appointments
                .Where(a => a.StaffId == staffId && 
                            a.StartTime >= dayStart && 
                            a.EndTime < dayEnd)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
            
            // 3.Длительность услуги
            var staffService = await _context.StaffServices
                .FirstOrDefaultAsync(ss => ss.Id == staffServiceId);

            if (staffService == null) 
                return new List<TimeSlotDto>();

            var duration = staffService.Duration;
            var buffer = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(5));

            var result = new List<TimeSlotDto>();

            foreach (var wh in workingHours)
            {
                var interavalStart = date.Date.Add(wh.StartTime.ToTimeSpan());
                var intervalEnd = date.Date.Add(wh.EndTime.ToTimeSpan()); 

                var freeIntervals = SubtractAppointments(interavalStart, intervalEnd, appointments);
                
                foreach (var interval in freeIntervals) {
                    var slot = GenerateSlots(interval.start, interval.end, duration, buffer);
                    result.AddRange(slot);
                }
            }

            //4. Убираем прошлое время 
            var now = DateTime.Now;

            result = result.Where(s => s.Start > now).ToList();

            return result;
        }

        private List<TimeSlotDto> GenerateSlots(
            DateTime start,
            DateTime end,
            TimeOnly duration,
            TimeOnly buffer)
        {
            var result = new List<TimeSlotDto>();

            var step = duration.ToTimeSpan() + buffer.ToTimeSpan();
            var serviceDuration = duration.ToTimeSpan();

            var current = start;

            while (true)
            {
                var slotEnd = current.Add(serviceDuration);

                if (slotEnd > end) 
                    break;
                result.Add(new TimeSlotDto
                {
                    Start = current,
                    End = slotEnd,
                });

                current = current.Add(step);
            }

            return result;
        }

        private List<(DateTime start, DateTime end)> SubtractAppointments(
            DateTime start,
            DateTime end,
            List<Appointment> appointments)
        {
            var result = new List<(DateTime,  DateTime)>();
            var current = start;

            foreach (var appt in  appointments)
            {
                if (appt.StartTime >= end || appt.EndTime <= current)
                    continue;

                if (appt.StartTime > current)
                {
                    result.Add((current, appt.StartTime));
                }

                current = appt.EndTime > current ? appt.EndTime : current;
            }

            if (current < end)
            {
                result.Add((current, end));
            }

            return result;
        }
    }
}
