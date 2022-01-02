using System.Collections.Generic;
using UnityEngine;

public class OrbitCube : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speedGain = 1;
    [SerializeField] private float doubleClickInterval = 0.4f; // seconds

    private MasterController m_masterController;
    private List<string> faces = new List<string>()
        {
            "CubeFaceFront",
            "CubeFaceBack",
            "CubeFaceRight",
            "CubeFaceLeft",
            "CubeFaceTop",
            "CubeFaceBottom",
        };

    private string targetFaceName = string.Empty; // rotate this to camera
    private string downwardFaceName = string.Empty; // rotate this to face downward
    private Vector3 mousePosLastFrame;
    private Vector3 vectorToCamera;
    private float mouseTimeLastClicked = -1f;
    
    private void Start()
    {
        try
        {
            m_masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        }
        catch
        {
            Debug.LogWarning("Master Controller not found.");
        }
        
        vectorToCamera = gameObject.transform.position - mainCamera.transform.position;
    }

    private void Update()
    {
        RotateCubeFaceToCamera();
    }

    // get mouse position from one frame to the next and rotate the cube accordingly
    private void OnMouseDrag()
    {
        Vector3 mouseDisplacement = Input.mousePosition - mousePosLastFrame;

        Vector3 angularVelocity = Vector3.Cross(mouseDisplacement, vectorToCamera);

        rb.angularVelocity = angularVelocity * speedGain;

        // reset
        mousePosLastFrame = Input.mousePosition;
    }

    // detect double-tap on given face
    private void OnMouseDown()
    {
        // reset target
        SetTargetNormal(string.Empty);
        downwardFaceName = string.Empty;

        if (Time.time - mouseTimeLastClicked < doubleClickInterval)
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                SetTargetNormal(hit.collider.name);
            }
        }

        // reset
        mouseTimeLastClicked = Time.time;
    }

    private void SetTargetNormal(string faceColliderName)
    {
        targetFaceName = faceColliderName;
    }

    // rotate the double-clicked face to the camera and also orient the cube
    private void RotateCubeFaceToCamera()
    {
        if (targetFaceName == string.Empty) return;

        Vector3 targetFaceNormal = GetFaceNormalFromFaceColliderName(targetFaceName);
        Vector3 angularVelFromTargetFace = Vector3.Cross(vectorToCamera, targetFaceNormal);

        // target has been reached
        if (angularVelFromTargetFace.magnitude < 0.001)
        {
            // todo move the reset logic to a "on scene load" function instead
            TransitionToSliceView(targetFaceName, downwardFaceName);
            SetTargetNormal(string.Empty);
            downwardFaceName = string.Empty;
        }

        // find out which of the faces that are adjacent to the target face are
        // closest to pointing down, then align that face w/ down direction
        if (downwardFaceName == string.Empty)
        {
            float minVal = float.NegativeInfinity;

            foreach(var face in faces)
            {
                Vector3 faceNormal = GetFaceNormalFromFaceColliderName(face);

                // skip target and opposite faces
                if (Vector3.Cross(faceNormal, targetFaceNormal) == Vector3.zero) continue;

                float checkVal = Vector3.Dot(faceNormal, Vector3.down);
                if (checkVal > minVal)
                {
                    minVal = checkVal;
                    downwardFaceName = face;
                }
            }
        }

        Vector3 downwardFaceNormal = GetFaceNormalFromFaceColliderName(downwardFaceName);
        Vector3 angularVelFromDownwardFace = Vector3.Cross(downwardFaceNormal, Vector3.down);

        // arbitrary weights
        rb.angularVelocity = angularVelFromTargetFace * 2f + angularVelFromDownwardFace * 5f;
    }

    private Vector3 GetFaceNormalFromFaceColliderName(string faceColliderName)
    {
        // since the transform changes for every rotation, this dict can't just
        // be in the Start() method because then the transform values would be constant
        Dictionary<string, Vector3> facesOrientation = new Dictionary<string, Vector3>()
        {
            {"CubeFaceFront", transform.forward },
            {"CubeFaceBack", -transform.forward },
            {"CubeFaceRight", transform.right },
            {"CubeFaceLeft", -transform.right },
            {"CubeFaceTop", transform.up },
            {"CubeFaceBottom", -transform.up },
        };

        Vector3 normal;
        if (!facesOrientation.TryGetValue(faceColliderName, out normal))
        {
            Debug.LogWarning("Cube face not found");
            return Vector3.zero;
        }

        normal = facesOrientation[faceColliderName];
        return normal;
    }

    // this info will be passed to the SliceView scene so that it loads the correct slice
    void TransitionToSliceView(string targetFaceName, string downwardFaceName)
    {
        if (m_masterController == null) return;

        m_masterController.sliceTargetFaceName = targetFaceName;
        m_masterController.sliceDownwardFaceName = downwardFaceName;
        m_masterController.LoadScene("SliceViewScene");
    }
}
