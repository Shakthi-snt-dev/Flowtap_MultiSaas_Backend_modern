using Flowtap_Application.Common.DTOs;
using Flowtap_Medical.DbContext;
using Flowtap_Medical.Domain.Entities;
using Flowtap_Medical.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Medical.Application.Consultations.CreateConsultation;

public class CreateConsultationCommandHandler(IMedicalDbContext db)
    : IRequestHandler<CreateConsultationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateConsultationCommand request, CancellationToken ct)
    {
        var appointment = await db.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId && a.CompanyId == request.CompanyId, ct);

        if (appointment is null) return Result<Guid>.Failure("Appointment not found.");

        var consultation = new Consultation
        {
            CompanyId       = request.CompanyId,
            AppointmentId   = request.AppointmentId,
            ChiefComplaint  = request.ChiefComplaint,
            Diagnosis       = request.Diagnosis,
            TreatmentPlan   = request.TreatmentPlan,
            VitalSigns      = request.VitalSigns,
            DoctorNotes     = request.DoctorNotes,
            ConsultationFee = request.ConsultationFee,
            Prescriptions   = request.Prescriptions?.Select(p => new Prescription
            {
                MedicineName = p.MedicineName,
                ProductId    = p.ProductId,
                Dosage       = p.Dosage,
                Duration     = p.Duration,
                Instructions = p.Instructions
            }).ToList() ?? []
        };

        appointment.Status = AppointmentStatus.Completed;

        db.Consultations.Add(consultation);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(consultation.Id);
    }
}
