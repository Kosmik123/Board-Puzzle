﻿using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.PuzzleBoard.Spawning
{
    public class PooledPiecesSpawner : PiecesSpawner
    {
        [SerializeField]
        private PieceComponent piecePrototype;
        [SerializeField]
        private Transform piecesContainer;

        private Stack<PieceComponent> piecesPool = new Stack<PieceComponent>();

        protected override PieceComponent Spawn(int x, int y)
        {
            var spawnedPiece = piecesPool.Count > 0 ? piecesPool.Pop() : CreateNewPiece();
            spawnedPiece.IsCleared = false;
            spawnedPiece.gameObject.SetActive(true);
            return spawnedPiece;
        }

        private PieceComponent CreateNewPiece()
        {
            var pieceComponent = Instantiate(piecePrototype, piecesContainer);
            pieceComponent.Piece = new Piece();
            pieceComponent.OnCleared += Release;
            return pieceComponent;
        }

        private void Release(PieceComponent piece)
        {
            piece.gameObject.SetActive(false);
            piecesPool.Push(piece);
        }
    }
}
