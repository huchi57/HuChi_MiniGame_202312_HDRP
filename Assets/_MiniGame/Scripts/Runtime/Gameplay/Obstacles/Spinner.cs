using System.Collections;
using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField] private bool m_preWarm;
        [SerializeField] private float m_targetSpinningSpeed;
        [SerializeField] private float m_acceleration;
        [SerializeField] private Vector3 m_spinningAxis;

        [Header("Blur Mimicing")]
        [SerializeField]
        private bool m_enableBlurMimicing;

        [SerializeField, NonEditable, ShowIf(nameof(m_enableBlurMimicing), true)]
        private MeshFilter m_mesh;

        [SerializeField, NonEditable, ShowIf(nameof(m_enableBlurMimicing), true)]
        private MeshRenderer m_renderer;

        [SerializeField, ShowIf(nameof(m_enableBlurMimicing), true)]
        private int m_numberOfBlurCopies;

        [SerializeField, ShowIf(nameof(m_enableBlurMimicing), true)]
        private float m_blurDistanceMultiplier = 1;

        [SerializeField, ShowIf(nameof(m_enableBlurMimicing), true)]
        private AnimationCurve m_blurTransparency0To1Falloff;

        private float m_currentSpinningSpeed;

        private Transform[] m_duplicateInstances;

        public float TargetSpinningSpeed
        {
            get => m_currentSpinningSpeed;
            set => m_currentSpinningSpeed = value;
        }

        public float Acceleration
        {
            get => m_acceleration;
            set => m_acceleration = value;
        }

        private void OnValidate()
        {
            m_mesh = GetComponent<MeshFilter>();
            m_renderer = GetComponent<MeshRenderer>();
        }

        private IEnumerator Start()
        {
            m_currentSpinningSpeed = m_preWarm ? m_targetSpinningSpeed : 0;
            if (m_enableBlurMimicing && m_renderer)
            {
                m_duplicateInstances = new Transform[m_numberOfBlurCopies];
                for (int i = 0; i < m_duplicateInstances.Length; i++)
                {
                    var materialAlpha = m_blurTransparency0To1Falloff.Evaluate(i / (float)m_numberOfBlurCopies);
                    var materialInstance = Instantiate(m_renderer.material);
                    materialInstance.color = new Color(materialInstance.color.r, materialInstance.color.g, materialInstance.color.b, materialAlpha);

                    var newInstance = new GameObject();
                    newInstance.transform.SetParent(transform);
                    newInstance.transform.localScale = Vector3.one;
                    newInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    newInstance.AddComponent<MeshFilter>().mesh = m_mesh.mesh;
                    newInstance.AddComponent<MeshRenderer>().material = materialInstance;
                    m_duplicateInstances[i] = newInstance.transform;
                    yield return null;
                }
            }
        }

        private void Update()
        {
            m_currentSpinningSpeed = Mathf.Lerp(m_currentSpinningSpeed, m_targetSpinningSpeed, m_acceleration * Time.deltaTime);
            transform.localEulerAngles += m_currentSpinningSpeed * Time.deltaTime * m_spinningAxis;

            if (m_enableBlurMimicing && TimeManager.TimeScale > 0.001f)
            {
                for (int i = 0; i < m_duplicateInstances.Length; i++)
                {
                    var item = m_duplicateInstances[i];
                    if (item)
                    {
                        item.localEulerAngles = -(i + 1) * m_blurDistanceMultiplier * m_currentSpinningSpeed * Time.deltaTime * m_spinningAxis;
                    }
                }
            }
        }
    }
}
