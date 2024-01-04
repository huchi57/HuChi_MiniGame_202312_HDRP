using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, NonEditable] private Rigidbody m_rigidbody;

        [SerializeField] private float m_minInitialVelocity;
        [SerializeField] private float m_maxInitialVelocity;

        [SerializeField] private float m_maxPitchUpAngle;
        [SerializeField] private float m_maxPitchDownAngle;
        [SerializeField] private float m_pitchAngleSlerpSpeed;

        [SerializeField] private float m_pitchInputMultiplier;
        [SerializeField] private float m_maxInputPitchUpSecond;
        [SerializeField] private float m_pitchUpAngleThresholdForCooldownTimerStart;
        [SerializeField] private float m_pitchDownAngleThresholdBeforeCooldownEnds;

        [SerializeField] private float m_pitchNaturalDropSpeed;

        [SerializeField] private AnimationCurve m_pitchToAccelCurve;
        [SerializeField] private float m_minMoveVelocity;
        [SerializeField] private float m_maxMoveVelocity;
        [SerializeField] private float m_velocityLerpRate;

        private Vector3 m_originalPosition;
        private Quaternion m_originalRotation;

        private float m_pitchInput;
        private float m_inputPitchUpTimer;
        private bool m_isInPitchUpCooldown;

        private float m_targetPitchAngle;
        private float m_acceleration;
        private float m_moveVelocity;

        private bool m_isBlownAwayByWind;
        private Vector3 m_externalWindSpeed;

        private float CurrentPitchAngle => m_rigidbody.rotation.eulerAngles.z.AnglePositiveOrNegative180();

        public void UpdateRespawnPoint(Vector3 point)
        {
            m_originalPosition = point;
        }

        public void LockToPitchAngle(float pitchAngle)
        {
            m_targetPitchAngle = pitchAngle;
        }

        public void ResetPlanePosition()
        {
            transform.SetPositionAndRotation(m_originalPosition, m_originalRotation);
            m_rigidbody.useGravity = false;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_isBlownAwayByWind = false;
        }

        public void InitializeAndEjectPlane()
        {
            m_targetPitchAngle = CurrentPitchAngle;
            m_moveVelocity = Random.Range(m_minInitialVelocity, m_maxInitialVelocity);
            GameManager.Instance.SwitchGameState(GameState.GameplayPausable);
            CameraBrain.Main.SaveCameraCheckpointPosition();
        }

        public void TriggerGameOver(bool isInstantDeath = false)
        {
            EnableUnityBuiltInGravity();
            if (isInstantDeath)
            {
                GameManager.Instance.GameOverAndRestartCheckpoint_Instant();
            }
            else
            {
                GameManager.Instance.GameOverAndRestartCheckpoint_FadeOut();
            }
        }

        public void TriggerGameOverByEnteringWindTrigger(Vector3 windSpeed)
        {
            m_isBlownAwayByWind = true;
            m_externalWindSpeed = windSpeed;
            TriggerGameOver();
        }

        private void OnValidate()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.useGravity = false;
        }

        private void Awake()
        {
            m_originalPosition = transform.position;
            m_originalRotation = transform.rotation;
            GameManager.RegisterPlayer(this);
            InputManager.OnAnyKeyPressed += OnAnyKeyPressed;
            GameManager.OnEachGameOverSignaled += EnableUnityBuiltInGravity;
            GameManager.OnEachLoadingOperationStarts += ResetPlanePosition;
            GameManager.OnEachFadeInCompleted += ChangeGameStateToWaitForPlayerStart;
        }

        private void OnDestroy()
        {
            InputManager.OnAnyKeyPressed -= OnAnyKeyPressed;
            GameManager.OnEachGameOverSignaled -= EnableUnityBuiltInGravity;
            GameManager.OnEachLoadingOperationStarts -= ResetPlanePosition;
            GameManager.OnEachFadeInCompleted -= ChangeGameStateToWaitForPlayerStart;
        }

        private void OnAnyKeyPressed(UnityEngine.InputSystem.InputControl key)
        {
            if (GameManager.Instance.CurrentGameState == GameState.WaitForInputToStartGame && !key.name.ToLower().Contains("esc") && !key.name.ToLower().Contains("back") && !key.name.ToLower().Contains("f6"))
            {
                InitializeAndEjectPlane();
            }
        }

        private void ChangeGameStateToWaitForPlayerStart()
        {
            GameManager.Instance.SwitchGameState(GameState.WaitForInputToStartGame);
        }

        private void OnEnable()
        {
            InputManager.OnMove += OnMove;
            // TODO: Add callback to reset player when restart
        }

        private void OnDisable()
        {
            InputManager.OnMove -= OnMove;
            // TODO: Remove callback to reset player when restart
        }

        private void OnMove(Vector2 move)
        {
            if (GameManager.Instance.CurrentGameState != GameState.GameplayPausable)
            {
                return;
            }

            m_pitchInput = m_pitchInputMultiplier * move.y;
            if (!m_isInPitchUpCooldown)
            {
                if (CurrentPitchAngle > m_pitchUpAngleThresholdForCooldownTimerStart)
                {
                    m_inputPitchUpTimer += Time.deltaTime;
                    if (m_inputPitchUpTimer > m_maxInputPitchUpSecond)
                    {
                        m_isInPitchUpCooldown = true;
                    }
                }
                else
                {
                    m_inputPitchUpTimer = 0;
                }
            }
            else
            {
                m_pitchInput = Mathf.Min(m_pitchInput, 0);
                if ((CurrentPitchAngle < -m_pitchDownAngleThresholdBeforeCooldownEnds) || (CurrentPitchAngle < 0 && move.y < 0))
                {
                    m_isInPitchUpCooldown = false;
                    m_inputPitchUpTimer = 0;
                }
            }
        }

        private void FixedUpdate()
        {
            if (m_isBlownAwayByWind)
            {
                m_rigidbody.velocity += Time.fixedDeltaTime * m_externalWindSpeed;
            }

            if (GameManager.Instance.CurrentGameState != GameState.GameplayPausable)
            {
                return;
            }

            m_targetPitchAngle += m_pitchInput * Time.fixedDeltaTime;
            if (m_pitchInput <= 0)
            {
                m_targetPitchAngle -= m_pitchNaturalDropSpeed * Time.fixedDeltaTime;
            }
            m_targetPitchAngle = Mathf.Clamp(m_targetPitchAngle, -m_maxPitchDownAngle, m_maxPitchUpAngle);
            m_rigidbody.rotation = Quaternion.Slerp(m_rigidbody.rotation, Quaternion.Euler(new Vector3(0, 0, m_targetPitchAngle)), m_pitchAngleSlerpSpeed * Time.deltaTime);

            m_acceleration = m_pitchToAccelCurve.Evaluate(CurrentPitchAngle);
            m_moveVelocity += m_acceleration * Time.fixedDeltaTime;
            m_moveVelocity = Mathf.Lerp(m_moveVelocity, Mathf.Clamp(m_moveVelocity, m_minMoveVelocity, m_maxMoveVelocity), m_velocityLerpRate * Time.fixedDeltaTime);
            m_rigidbody.velocity = m_moveVelocity * transform.right;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (GameManager.Instance.CurrentGameState != GameState.GameCompletedWaitForInput)
            {
                TriggerGameOver(isInstantDeath: collision.gameObject.GetComponent<InstantGameOverOnCollision>());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GameOverTrigger>(out var trigger))
            {
                GameManager.Instance.GameOverAndRestartCheckpoint_FadeOut(trigger.WaitTimeBeforeRestartingWhenGameOverTriggered);
            }
        }

        private void EnableUnityBuiltInGravity()
        {
            m_rigidbody.useGravity = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            UnityEditor.Handles.Label(transform.position, $"Pitch: {CurrentPitchAngle:F2}\n" +
                $"Target Pitch Angle: {m_targetPitchAngle:F2}\n" +
                $"Accel: {m_acceleration:F2}\n" +
                $"Move Velocity: {m_moveVelocity:F2}\n" +
                $"Cooldown Timer: {m_inputPitchUpTimer:F2}\n" +
                $"Is Pitchup Cooldown: {m_isInPitchUpCooldown}\n"
                );
        }
#endif
    }
}
