using TeseAPIs.Models;

namespace TeseAPIs.Validations
{
    public interface ICreditValidations
    {
        public bool VerifyNoNegativeAmounts(PlayerProgress progress, int creditos);
    }
}