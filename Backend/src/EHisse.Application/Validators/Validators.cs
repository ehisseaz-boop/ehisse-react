using FluentValidation;
using EHisse.Application.DTOs;
using EHisse.Domain.Entities;

namespace EHisse.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("İstifadəçi adı və ya nömrə boş ola bilməz.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifrə boş ola bilməz.");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş ola bilməz.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş ola bilməz.");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Telefon nömrəsi tələb olunur.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Şifrə ən azı 6 simvol olmalıdır.");
    }
}

public class PartInputValidator : AbstractValidator<PartInput>
{
    public PartInputValidator()
    {
        RuleFor(x => x.DeviceModelId).GreaterThan(0).WithMessage("Model seçilməlidir.");
        RuleFor(x => x.PartTypeId).GreaterThan(0).WithMessage("Hissə növü seçilməlidir.");
        RuleFor(x => x.Title).NotEmpty().Length(3, 200).WithMessage("Adın uzunluğu 3 ilə 200 simvol arasında olmalıdır.");
        RuleFor(x => x.SKU).NotEmpty().Length(2, 50).WithMessage("SKU uzunluğu 2 ilə 50 simvol arasında olmalıdır.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Qiymət sıfırdan böyük olmalıdır.");
        RuleFor(x => x.WarrantyMonths).GreaterThanOrEqualTo(0).WithMessage("Zəmanət müddəti mənfi ola bilməz.");
    }
}
