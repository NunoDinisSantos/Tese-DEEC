using TeseAPIs.Validations;

namespace APIUnitTests
{
    public class CreditValidationUnitTest
    {
        [Theory]
        [InlineData(100,50)]
        [InlineData(100,-50)]
        [InlineData(100, -100)]
        public void VerifyNoNegativeAmounts_ShouldUpdateReturnsTrue_WhenAddingCredits(
            int studentSavedCredits, int creditsToSum)
        {
            var updater = new CreditValidations();

            var result = updater.VerifyNoNegativeAmounts(studentSavedCredits, creditsToSum);

            Assert.True(result);
        }

        [Fact]
        public void VerifyNoNegativeAmounts_ShouldUpdateReturnsFalse_WhenSubtractingCreditsAreMoreThanTotalCredits()
        {
            var updater = new CreditValidations();
            int studentSavedCredits = 100;
            int creditsToSum = -101;

            var result = updater.VerifyNoNegativeAmounts(studentSavedCredits, creditsToSum);

            Assert.False(result);
        }
    }
}
