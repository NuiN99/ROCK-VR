using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuiN.NExtensions;
using TNRD;
using UnityEngine;

namespace NuiN.Movement
{
    [RequireComponent(typeof(IMovement), typeof(IMovementInput))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] SerializableInterface<IMovement> movement;
        [SerializeField] SerializableInterface<IMovementInput> input;
        [SerializeField] SerializableInterface<IMovementInput> vrInput;

        [SerializeField] bool useMouseAndKeyboard;

        IMovement _movement;
        IMovementInput _input;
        
        List<MovementConstraint> _activeConstraints = new();

        [field: SerializeField, ReadOnlyPlayMode] public bool CanMove { get; private set; } = true;
        [field: SerializeField, ReadOnlyPlayMode] public bool CanRotate { get; private set; } = true;
        [field: SerializeField, ReadOnlyPlayMode] public bool IsRunning { get; private set; } = false;

        bool ConstraintApplied => _activeConstraints.Count > 0;

        void Awake()
        {
            _movement = movement.Value;
            _input = useMouseAndKeyboard ? input.Value : vrInput.Value;
            
            if(movement == null) Debug.LogError($"Missing Movement component on {gameObject}", gameObject);
            if (input == null) Debug.LogError($"Missing MovementInput on {gameObject.name}", gameObject);
        }

        void Update()
        {
            if(_input.ShouldJump()) _movement.Jump();
            
            if(CanRotate) _movement.Rotate(_input);
        }

        void FixedUpdate()
        {
            IsRunning = _input.IsRunning(); // only for debugging
            
            if(CanMove) _movement.Move(_input);
            
            _movement.FixedTick();
        }
        
        public void ApplyConstraint(float duration, MovementConstraint constraint)
        {
            StartCoroutine(ApplyConstraintRoutine(duration, constraint));
        }

        IEnumerator ApplyConstraintRoutine(float duration, MovementConstraint constraint)
        {
            _activeConstraints.Add(constraint);
            EvaluateConstraints();
            
            yield return new WaitForSeconds(duration);
            
            _activeConstraints.Remove(constraint);
            EvaluateConstraints();
        }

        void EvaluateConstraints()
        {
            // only allow move and rotate if every element in the list allows it
            CanMove = _activeConstraints.All(constraint => constraint.canMove);
            CanRotate = _activeConstraints.All(constraint => constraint.canRotate);
        }
    }
}



