namespace TeseAPIs.Validations
{
    public interface IRegistrationValidations
    {
        public Task<bool> VerifyIfCanRegister(string studentId);
    }
}