namespace TeseAPIs.Validations
{
    public class CreditValidations : ICreditValidations
    {
        public bool VerifyNoNegativeAmounts(int progressCredits, int creditos)
        {
            return progressCredits+creditos>=0;
        }
    }
}