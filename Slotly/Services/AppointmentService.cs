using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.DTOs.Appointment;
using Slotly.Entities;
using Slotly.Models.Appointment;

public class AppointmentService
{
    private readonly SlotlyContext _context;
    private readonly IMapper _mapper;

    public AppointmentService(SlotlyContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GetAppointmentDto>> GetByStaffAsync(Guid staffId)
    {
        var staffExists = await _context.Staffs
            .AnyAsync(s => s.Id == staffId);

        if (!staffExists)
            throw new Exception("Staff not found");

        var appointments = await _context.Appointments
            .Where(a => a.StaffId == staffId)
            .OrderBy(a => a.StartTime)
            .ToListAsync();

        return _mapper.Map<List<GetAppointmentDto>>(appointments);
    }

    public async Task<GetAppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        // Существует ли Staff
        var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.Id == dto.StaffId);

        if (staff == null) throw new Exception("Staff not found");

        // Получаем услугу сотрудника
        var staffService = await _context.StaffServices.FirstOrDefaultAsync(ss => ss.Id == dto.StaffServiceId);

        if (staffService == null) throw new Exception("Service not found");

        // Считаем время EndTime
        var endTime = dto.StartTime.AddMinutes(
                staffService.Duration.Hour * 60 + staffService.Duration.Minute
            );

        // Проверяем WorkHours
        var day = dto.StartTime.DayOfWeek;

        var workHours = await _context.WorkingHours
            .Where(w => w.StaffId == dto.StaffId && w.DayOfWeek == day)
            .ToListAsync();

        var isInsideWorkingHours = workHours.Any(
            w => dto.StartTime.TimeOfDay >= w.StartTime.ToTimeSpan()
            && endTime.TimeOfDay <= w.EndTime.ToTimeSpan()
            );

        if (!isInsideWorkingHours) throw new Exception("Outside workin hours");

        // Проверяем пересечения
        var hasConflict = await _context.Appointments
            .AnyAsync(a =>
            a.StaffId == dto.StaffId &&
            a.Status != AppointmentStatus.Cancelled &&
            dto.StartTime < a.EndTime &&
            endTime > a.StartTime);

        if (hasConflict) throw new Exception("Time slot is already broked");

        // Создание записи
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            StaffId = dto.StaffId,
            ClientId = dto.ClientId,
            StartTime = dto.StartTime,
            EndTime = endTime,
            Status = AppointmentStatus.Confirmed,
            FinalPrice = staffService.Price,
            Title = dto.Title,
            Description = dto.Description,
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return _mapper.Map<GetAppointmentDto>(appointment);
    }
}