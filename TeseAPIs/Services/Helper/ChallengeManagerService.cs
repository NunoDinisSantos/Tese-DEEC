using System.Globalization;
using TeseAPIs.Data;
using TeseAPIs.Models;

namespace TeseAPIs.Services.Helper
{
    public class ChallengeManagerService(IChallengeProgress challengeWinnerProgress, IDbConnectionFactory connectionFactory, IChallengeProgress challengeProgressService, IChallengeWinner challengeWinnerService) : IChallengeManagerService
    {
        public async Task<int> ValidateForChallenges(Challenge challenge, bool endedByApp)
        {
            var possibleWinners = await challengeWinnerProgress.GetChallengeProgress(challenge.EventType);
            var winner = new ChallengeProgressData() { PlayerId = "" };

            bool playerParticipation = false;

            if (possibleWinners != null && possibleWinners.Any())
            {
                playerParticipation = possibleWinners.OrderByDescending(x => x.Coins).First().Coins > 0;
            }

            if (playerParticipation) {

                using var connection = await connectionFactory.CreateConnectionAsync();

                var endDate = DateTime.Parse(challenge.EndDate, CultureInfo.InvariantCulture);

                if (challenge.EventType < 7 && !endedByApp) // need to wait for challenge to end
                {
                    if (DateTime.Compare(DateTime.Now, endDate) < 1) //Challenge still ongoing
                    {
                        return 0;
                    }
                }

                switch (challenge.EventType)
                {
                    case 0:
                        //Highest amount of coins until challenge end
                        winner = possibleWinners.OrderByDescending(x => x.Coins)
                            .FirstOrDefault();

                        break;

                    case 1:
                        //Highest amount of credits until challenge end.
                        winner = possibleWinners.OrderByDescending(x => x.Credits)
                            .FirstOrDefault();

                        break;

                    case 2:
                        //Highest amount of fish caught until challenge end.
                        winner = possibleWinners.OrderByDescending(x => x.FishCaught)
                            .FirstOrDefault();

                        break;

                    case 3:
                        //Highest amount of coins and fish caught until challenge end.
                        winner = possibleWinners
                            .OrderByDescending(x => x.Coins + x.FishCaught)
                            .FirstOrDefault();
                        break;

                    case 4:
                        //Highest amount of coins and credits until challenge end.
                        winner = possibleWinners
                            .OrderByDescending(x => x.Coins + x.Credits)
                            .FirstOrDefault();
                        break;

                    case 5:
                        //Highest amount of credits and fish caught until challenge end.
                        winner = possibleWinners
                            .OrderByDescending(x => x.Credits + x.FishCaught)
                            .FirstOrDefault();
                        break;

                    case 6:
                        //Highest amount of coins, credits and fish caught until challenge end.
                        winner = possibleWinners
                            .OrderByDescending(x => x.Coins + x.FishCaught + x.Credits)
                            .FirstOrDefault();
                        break;

                    case 7:
                        //First player to get to XXX coins.
                        winner = possibleWinners
                            .FirstOrDefault(x => x.Coins >= challenge.QuantityX);

                        break;

                    case 8:
                        //First player to get to XXX credits.
                        winner = possibleWinners
                            .FirstOrDefault(x => x.Credits >= challenge.QuantityX);
                        break;

                    case 9:
                        //First player to get to XXX fish.
                        winner = possibleWinners
                            .FirstOrDefault(x => x.FishCaught >= challenge.QuantityX);
                        break;

                    case 10:
                        //First player to get to XXX coins and YYY credits.
                        winner = possibleWinners
                                .FirstOrDefault(x => x.Coins >= challenge.QuantityX && x.Credits >= challenge.QuantityY);
                        break;

                    case 11:
                        //First player to get to XXX coins and YYY fish caught.
                        winner = possibleWinners
                            .FirstOrDefault(x => x.Coins >= challenge.QuantityX && x.FishCaught >= challenge.QuantityY);
                        break;

                    case 12:
                        //First player to get to XXX fish caught and YYY credits.
                        winner = possibleWinners
                            .FirstOrDefault(x => x.FishCaught >= challenge.QuantityX && x.Credits >= challenge.QuantityY);
                        break;

                    case 13:
                        //First player to get to XXX coins, YYY credits and ZZZ fish caught.
                        winner = possibleWinners
                            .FirstOrDefault(x => x.Coins >= challenge.QuantityX && x.Credits >= challenge.QuantityY
                            && x.FishCaught >= challenge.QuantityZ);
                        break;

                    case 14:
                        winner = possibleWinners
                        .FirstOrDefault(x => x.CaughtRareFish > 0);
                        break;
                }
            }

            if (winner == null || winner.PlayerId == string.Empty || !playerParticipation)
            {
                var endDate = DateTime.Parse(challenge.EndDate, CultureInfo.InvariantCulture);

                if (DateTime.Compare(DateTime.Now, endDate) > 0 || endedByApp) //Challenge ended 
                {
                    var noWinnerDto = new ChallengeWinnerDataDTO()
                    {
                        ChallengeId = challenge.Id,
                        Nick_Name = "NO WINNER",
                        Player_Id = ""
                    };


                    await challengeWinnerService.PostWinner(noWinnerDto); 
                    //await challengeProgressService.ResetChallengeProgress();
                    return -1;
                }

                return 0;
            }

            var winnerDto = new ChallengeWinnerDataDTO()
            {
                ChallengeId = challenge.Id,
                Nick_Name = winner.Nick_Name,
                Player_Id = winner.PlayerId
            };

            await challengeWinnerService.PostWinner(winnerDto);
            //await challengeProgressService.ResetChallengeProgress();

            return 1;
        }
    }
}
