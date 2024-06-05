using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BlockShift
{
    public class GameManager : Singleton<GameManager>
    {
        #region Serialized Field

        [SerializeField] private GridManager gridManager;
        [SerializeField] private GridBuilder gridBuilder;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private GemParticleManager particleManager;
        #endregion

        #region Seed

        public UnityEngine.Random.State seedGenerator;

        #endregion


        #region Actions

        public Action<GameState> OnGameStateChanged;
        public Action<Cell> OnCellTouched;
        public Action<GridState> OnGridChanged;
        public Action<int, int> OnEarnedXp;
        public Action OnLevelUp;

        #endregion

        public GameState currentGameState;
        public GridState currentGridState;
        public int currentMoves;
        public bool canSpin;

        private void Start()
        {
            currentGridState = GridState.Empty;
            currentGameState = GameState.Loading;
        }

        public void ChangeGameState(GameState state)
        {
            if (currentGameState != state)
            {
                currentGameState = state;
                OnGameStateChanged?.Invoke(state);
            }
        }

        public void ChangeGridState(GridState state)
        {
            if (currentGridState != state)
            {
                currentGridState = state;
                OnGridChanged?.Invoke(state);
            }
        }

        public GridManager GetGridManager()
        {
            return gridManager;
        }

        public void SetGridManager(GridManager manager)
        {
            this.gridManager = manager;
        }

        public GridBuilder GetGridBuilder()
        {
            return gridBuilder;
        }

        public void SetGridBuilder(GridBuilder builder)
        {
            this.gridBuilder = builder;
        }

        public void SetParticleManager(GemParticleManager gemParticleManager)
        {
            particleManager = gemParticleManager;
        }

        public void SetUIManager(UIManager uiManager)
        {
            this.uiManager = uiManager;
        }

        public UIManager GetUIManager()
        {
            return uiManager;
        }

        public GemParticleManager GetParticleManager()
        {
            return particleManager;
        }
    }

}
