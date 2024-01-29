using System.Collections;
using UnityEngine;
using Cinemachine.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyController : MonoBehaviour
{
    private Transform _playerTransformRef;
    private LayerMask _playerLayerMask;

    private Rigidbody2D _RB;

    public bool HasPlayerNearbyMovement;

    [SerializeField] private EnemyIdle     _idleBehaviour;
    [SerializeField] private EnemyMovement _moveBehaviour;

    [SerializeField] private float _detectionRange;
    [SerializeField] private float _detectionTime;

    private void Awake()
    {
        _playerTransformRef = GameObject.FindGameObjectWithTag("Player").transform;
        _playerLayerMask = LayerMask.GetMask("Player");

        _RB = GetComponent<Rigidbody2D>();

        _idleBehaviour = GetComponent<EnemyIdle>();
        _idleBehaviour.SetRigidBody(_RB);
        _idleBehaviour.StartIdleBehaviour();

        if (HasPlayerNearbyMovement)
        {
            _moveBehaviour = GetComponent<EnemyMovement>();
            _moveBehaviour.SetRigidBody(_RB);
            _moveBehaviour.IsMoving = false;

            InvokeRepeating(nameof(SwapMovementIdle), 0.5f, _detectionTime);
        }
    }

    /// <summary>
    /// Swap between movement and idle behaviour based on player in range.
    /// </summary>
    private void SwapMovementIdle()
    {
        if (Physics2D.OverlapCircle(transform.position, _detectionRange, _playerLayerMask))
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            _idleBehaviour.StopIdleBehaviour();
            _moveBehaviour.ResumeMovement();
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            _idleBehaviour.StartIdleBehaviour();
            _moveBehaviour.StopMovement();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyController))]
class EnemyControllerEditor : Editor
{
    SerializedProperty HasPlayerNearbyMovementProperty;

    SerializedProperty IdleBehaviourProperty;
    SerializedProperty MoveBehaviourProperty;

    SerializedProperty DetectionRangeProperty;
    SerializedProperty DetectionTimeProperty;

    private void OnEnable()
    {
        HasPlayerNearbyMovementProperty = serializedObject.FindProperty("HasPlayerNearbyMovement");

        IdleBehaviourProperty  = serializedObject.FindProperty("_idleBehaviour");
        MoveBehaviourProperty  = serializedObject.FindProperty("_moveBehaviour");

        DetectionRangeProperty = serializedObject.FindProperty("_detectionRange");
        DetectionTimeProperty  = serializedObject.FindProperty("_detectionTime");
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Label("Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(HasPlayerNearbyMovementProperty);
        EditorGUILayout.PropertyField(IdleBehaviourProperty);
        if (HasPlayerNearbyMovementProperty.boolValue)
        {
            EditorGUILayout.PropertyField(MoveBehaviourProperty);
        }

        GUILayout.Space(10);
        GUILayout.Label("Detection", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(DetectionRangeProperty);
        EditorGUILayout.PropertyField(DetectionTimeProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif