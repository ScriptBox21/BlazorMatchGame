using BlazorMatchGame.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BlazorMatchGame.Test
{
    public class MatchGameModelTest
    {
        [Fact]
        public void StartsWithZeroMatches()
        {
            var model = new MatchGameModel(0);
            Assert.Equal(0, model.MatchesFound);
        }

        [Fact]
        public void StartsWithAllCardsFaceDown()
        {
            var model = new MatchGameModel(0);
            Assert.All(model.ShuffledCards, card => Assert.False(card.IsTurned));
        }

        [Fact]
        public async Task WhenUserSelectsNonMatchingPair_TurnsBothBack()
        {
            // Find a non-matching pair
            var model = new MatchGameModel(0);
            var firstSelection = model.ShuffledCards[0];
            var secondSelection = model.ShuffledCards
                .First(c => c.Animal != firstSelection.Animal);

            // Select first one
            await model.SelectCardAsync(firstSelection);
            Assert.True(firstSelection.IsTurned);

            // Select second one - everything resets
            await model.SelectCardAsync(secondSelection);
            Assert.False(firstSelection.IsTurned);
            Assert.False(secondSelection.IsTurned);
            Assert.False(firstSelection.IsMatched);
            Assert.False(firstSelection.IsMatched);
            Assert.Equal(0, model.MatchesFound);
        }

        [Fact]
        public async Task WhenUserSelectsMatchingPair_StaysMatched()
        {
            // Find a non-matching pair
            var model = new MatchGameModel(0);
            var firstSelection = model.ShuffledCards[0];
            var secondSelection = model.ShuffledCards.Skip(1)
                .Single(c => c.Animal == firstSelection.Animal);

            // Select first one
            await model.SelectCardAsync(firstSelection);
            Assert.True(firstSelection.IsTurned);
            Assert.Equal(0, model.MatchesFound);

            // Select second one - everything resets
            await model.SelectCardAsync(secondSelection);
            Assert.True(firstSelection.IsTurned);
            Assert.True(secondSelection.IsTurned);
            Assert.True(firstSelection.IsMatched);
            Assert.True(secondSelection.IsMatched);
            Assert.Equal(1, model.MatchesFound);
            Assert.False(model.GameEnded);
        }

        [Fact]
        public async Task WhenAllMatchesFound_GameEnds()
        {
            var model = new MatchGameModel(0);
            var distinctAnimals = model.ShuffledCards.Select(c => c.Animal).Distinct().ToList();
            var expectedMatchCount = 0;

            // Select each pair in turn
            foreach (var animal in distinctAnimals)
            {
                Assert.False(model.GameEnded);
                Assert.False(model.LatestCompletionTime.HasValue);

                var matchingCards = model.ShuffledCards.Where(c => c.Animal == animal).ToList();
                Assert.Equal(2, matchingCards.Count);
                await model.SelectCardAsync(matchingCards[0]);
                await model.SelectCardAsync(matchingCards[1]);
                Assert.True(matchingCards[0].IsMatched);
                Assert.True(matchingCards[1].IsMatched);
                Assert.Equal(++expectedMatchCount, model.MatchesFound);
            }

            // Finally, the game should be completed
            Assert.True(model.GameEnded);
            Assert.True(model.LatestCompletionTime.HasValue);
        }
    }
}
