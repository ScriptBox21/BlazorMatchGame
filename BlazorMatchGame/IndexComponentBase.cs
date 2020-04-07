using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;

namespace BlazorMatchGame
{
    public class IndexComponentBase : ComponentBase
    {
        public readonly List<string> AnimalEmojis = new List<string>()
        {
            "🐶", "🐶",
            "🐺", "🐺",
            "🐮", "🐮",
            "🦊", "🦊",
            "🐱", "🐱",
            "🦁", "🦁",
            "🐯", "🐯",
            "🐹", "🐹",
        };

        public List<string> ShuffledAnimals { get; set; }

        protected Timer timer;
        protected int tenthsOfSecondsElapsed = 0;
        protected int matchesFound = 0;
        protected string timeDisplay;

        protected override void OnInitialized()
        {
            this.timer = new Timer(100);
            this.timer.Elapsed += Timer_Tick;
            SetUpGame();
        }

        protected void SetUpGame()
        {
            Random random = new Random();
            ShuffledAnimals = AnimalEmojis
                .OrderBy(item => random.Next())
                .ToList();

            this.matchesFound = 0;
            this.tenthsOfSecondsElapsed = 0;
        }

        private void Timer_Tick(Object source, ElapsedEventArgs e)
        {
            InvokeAsync(() =>
            {
                this.tenthsOfSecondsElapsed++;
                this.timeDisplay = (this.tenthsOfSecondsElapsed / 10F)
                    .ToString("0.0s");
                StateHasChanged();
            });
        }
        /*string firstClick= string.Empty;
        string secondClick = string.Empty;*/
        string lastAnimalFound = string.Empty;

        protected void ButtonClick(string animal)
        {
            if (lastAnimalFound == string.Empty)
            {
                //First selection of the pair. Remember it.
                lastAnimalFound = animal;
                timer.Start();
                return;
            }
            else if (lastAnimalFound == animal)
            {
                //Match found! Reset for next pair.
                lastAnimalFound = string.Empty;

                //Replace found animals with empty string to hide them
                ShuffledAnimals = ShuffledAnimals
                    .Select(a => a.Replace(animal, string.Empty))
                    .ToList();

                this.matchesFound++;
                if (this.matchesFound == 8)
                {
                    timer.Stop();
                    timeDisplay += " - Play Again?";
                    SetUpGame();
                }
            }
            else
            {
                //User selected a pair that don't match.
                //Reset selection.
                lastAnimalFound = string.Empty;
            }
        }
    }
}
