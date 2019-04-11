using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
    public bool DisableAfterUse = false;
    public GameObject currentGameZone;

    // We'll draw a gizmo in the scene view, so it can be found....
    void OnDrawGizmos()
    {
        if (CameraOffset == null)
            return;

        Gizmos.color = Color.white;

        Gizmos.DrawSphere(CameraOffset.position, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawLine(transform.position, CameraOffset.position);
        Gizmos.DrawWireCube(GetComponent<BoxCollider>().center + transform.position, GetComponent<BoxCollider>().size);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other != Player.Instance.Agent.CharacterController)
            return;

        if (DisableAfterUse)
        {
            GetComponent<BoxCollider>().enabled = false;
        }

        Player.Instance.currentGameZone = currentGameZone;
    }