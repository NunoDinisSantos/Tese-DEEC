using TeseAPIs.Models;

namespace TeseAPIs.Validations
{
    public class CreditValidations : ICreditValidations
    {
        public bool VerifyNoNegativeAmounts(PlayerProgress progress, int creditos)
        {
            if (progress.Creditos + creditos < 0)
            {
                return false;
            }

            return true;
        }
    }
}