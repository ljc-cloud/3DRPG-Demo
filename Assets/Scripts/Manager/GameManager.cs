using RPG.State;
using RPG.Tools;
using System.Collections.Generic;

namespace RPG.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public CharacterStates playerStates;
        private List<IEndGameObserve> endGameObserves;
        protected override void Awake()
        {
            base.Awake();
            endGameObserves = new List<IEndGameObserve>();
        }

        public void RegisterPlayerStates(CharacterStates characterStates)
        {
            playerStates = characterStates;
        }

        public void AddObserve(IEndGameObserve observe)
        {
            endGameObserves.Add(observe);
        }
        public void RemoveObserve(IEndGameObserve observe)
        {
            endGameObserves.Remove(observe);
        }

        public void NotiFyEndGame()
        {
            foreach (var observe in endGameObserves)
            {
                observe.EndNotify();
            }
        }
    }
}