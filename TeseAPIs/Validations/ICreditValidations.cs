namespace TeseAPIs.Validations
{
    public interface ICreditValidations
    {
        bool VerifyNoNegativeAmounts(int progressCredits, int creditos);
    }
}