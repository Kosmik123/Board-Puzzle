﻿using UnityEngine;

namespace Bipolar.PuzzleBoard
{
    public class Piece : MonoBehaviour
    {
        public event System.Action<PieceType> OnTypeChanged;
        public event System.Action<Piece> OnCleared;

        [SerializeField]
        private PieceType type;
        public PieceType Type
        {
            get => type;
            set
            {
                type = value;
                if (type && gameObject.scene.buildIndex >= 0) 
                    gameObject.name = $"Piece ({type.name})";
                OnTypeChanged?.Invoke(type);
            }
        }

        private bool isCleared = false;
        public bool IsCleared
        {
            get => isCleared;
            set
            {
                isCleared = value;
                if (isCleared)
                    Invoke(nameof(CallClearedEvent), 0);            
            }
        }

        private void CallClearedEvent()
        {
            OnCleared?.Invoke(this);
        }

        private void OnValidate()
        {
            Type = type;
        }
    }
}
