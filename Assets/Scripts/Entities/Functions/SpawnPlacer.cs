using System;
using ClassExtensions;
using Entities.Data;
using Entities.System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Edge = Entities.Data.Edge;

namespace Entities.Functions
{
    public class SpawnPlacer : MonoBehaviour
    {
        [SerializeField] private float _basicEdgeLift = 1f;
        [SerializeField] private float _topEdgeLift = 2.5f;
        
        private Vector2 _switchedPosition;
        private Vector2 _startLevelPosition;
        private bool _afterRestart;
        
        private readonly PreviousLevel? _previousLevel = LevelSwitcher.PreviousLevel;

        private void Awake() => SetSpawn();

        private void DoRestartResetIfNeeded()
        {
            if (_afterRestart)
            {
                transform.position = _startLevelPosition;
                _afterRestart = false;
                return;
            }

            _startLevelPosition = transform.position;
        }

        private void OnEnable()
        {
            LevelSwitcher.OnLevelSwitch += SaveSwitchPosition;
            LevelSwitcher.OnLevelStart += DoRestartResetIfNeeded;
            LevelSwitcher.OnLevelStart += SetSpawn;
            LevelSwitcher.OnLevelRestart += MarkRestart;
        }
        private void OnDisable()
        {
            LevelSwitcher.OnLevelSwitch -= SaveSwitchPosition;
            LevelSwitcher.OnLevelStart -= DoRestartResetIfNeeded;
            LevelSwitcher.OnLevelStart -= SetSpawn;
            LevelSwitcher.OnLevelRestart -= MarkRestart;
        }

        private void MarkRestart() => _afterRestart = true;

        private void SaveSwitchPosition() => _switchedPosition = transform.position;

        private void SetSpawn()
        {
            if(!NeedPositionReplace())
                return;

            transform.position = GetSpawnCoordinates();
        }

        private bool NeedPositionReplace() =>
            _previousLevel != null 
            && _previousLevel.Value.Id != SceneManager.GetActiveScene().buildIndex;

        // ReSharper disable once PossibleInvalidOperationException
        private Vector2 GetSpawnCoordinates() => _previousLevel.Value.Crossed switch
            {
                Edge.Left => _switchedPosition.ReflectX().WithAdjustedX(-_basicEdgeLift),
                Edge.Right => _switchedPosition.ReflectX().WithAdjustedX(_basicEdgeLift),
                Edge.Bottom => _switchedPosition.ReflectY().WithAdjustedY(-_basicEdgeLift),
                Edge.Top => _switchedPosition.ReflectY() + new Vector2(0, _topEdgeLift),
                _ => _switchedPosition
            };
    }
}