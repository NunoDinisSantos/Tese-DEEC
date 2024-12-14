namespace TeseAPIs.Validations
{
    public interface IRegistrationValidations
    {
        Task<bool> VerifyIfCanRegister(string studentId);
    }
}