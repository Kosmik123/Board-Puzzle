﻿using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.PuzzleBoard.Components
{
    [DisallowMultipleComponent]
    public abstract class BoardCollapseController : MonoBehaviour
    {
        [SerializeField]
        protected BoardController boardController;

        public abstract System.Type BoardType { get; }
        public abstract void Collapse();

        protected virtual void Reset()
        {
            boardController = FindObjectOfType<BoardController>();
        }

        protected void RequestCommand(IBoardCommand command) => boardController.RequestCommand(command);
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoardComponent<>))]
    public class BoardCollapseController<TStrategy, TBoard> : BoardCollapseController
        where TBoard : Board
        where TStrategy : BoardCollapseStrategy<TBoard>
    {
        [SerializeField]
        private PieceFactoryWrapper pieceFactory;

        private BoardComponent<TBoard> _boardComponent;
        public BoardComponent<TBoard> BoardComponent
        {
            get
            {
                if (_boardComponent == null)
                    _boardComponent = GetComponent<BoardComponent<TBoard>>();
                return _boardComponent;
            }
        }

        private BoardCollapser<TBoard> _collapser;
        public BoardCollapser<TBoard> Collapser
        {
            get
            {
                _collapser ??= CreateNewCollapser();
                return _collapser;
            }
        }

        [SerializeField]
        protected TStrategy strategy;
        public virtual TStrategy Strategy => strategy;

        [SerializeField]
        protected PiecesMover<TStrategy, TBoard> mover;

        private BoardCollapser<TBoard> CreateNewCollapser() => new BoardCollapser<TBoard>(
            BoardComponent.GetBoard(),
            Strategy,
            pieceFactory ? pieceFactory.PieceFactory : null);

        public sealed override System.Type BoardType => typeof(TBoard);

        [ContextMenu("Collapse")]
        public sealed override void Collapse()
        {
            Collapser.Collapse();
            var collapseCommand = new CollapseBoardCommand<TStrategy, TBoard>(mover, new List<ICollapseEventArgs>(Collapser.CollapseEvents), Strategy);
            RequestCommand(collapseCommand);
        }

        private void OnDestroy()
        {
            _collapser?.Dispose();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                _collapser = null;
            else
                _collapser ??= CreateNewCollapser();
        }
    }
}
