using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ItemThrower : MonoBehaviour
{
    [SerializeField] private float _throwForce;
    [SerializeField] private float _throwVerticalForce;
    [SerializeField] private float _heldObjectPositionDelay = 0.8f;
    private Vector3 _targetPosition;
    private Vector3 _targetOffset;
    public GameObject heldObject {  get; private set; }
    private List<Collider> _heldColliders = new List<Collider>();
    private Transform _oldParent;
    private float _pickupTick;
    private bool _dropping;
    private float _dropTick;

    private InputAction _throwAction;
    private InputAction _interactAction;

    private void Start()
    {
        _throwAction = InputSystem.actions.FindAction("Attack");
        _interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        if(CanThrow() && _throwAction.WasPressedThisFrame())
        {
            Throw();
        }
        if(CanThrow() && _interactAction.WasPressedThisFrame() && Time.time - _pickupTick >= 0.1f)
        {
            _dropping = true;
            _dropTick = Time.time;
        }
        if(heldObject != null)
        {
            _targetPosition = transform.position + _targetOffset;
            heldObject.transform.position = Vector3.Slerp(heldObject.transform.position, _targetPosition, Time.deltaTime * _heldObjectPositionDelay);
            heldObject.transform.rotation = Quaternion.Slerp(heldObject.transform.rotation, transform.rotation, Time.deltaTime * _heldObjectPositionDelay);
        }
        if (_dropping && heldObject != null)
        {
            if (Time.time - _dropTick <= 0.4f && _interactAction.WasReleasedThisFrame())
            {
                DropObject();
            }
        }
    }

    void Throw()
    {
        Rigidbody _rb = heldObject.GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.AddForce(Camera.main.transform.forward * _throwForce + Vector3.up * _throwVerticalForce, ForceMode.Impulse);
        heldObject = null;
        foreach (Collider _collider in _heldColliders)
        {
            _collider.enabled = true;
        }
        _heldColliders.Clear();
    }

    public void DropObject()
    {
        Rigidbody _rb =  heldObject.GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        heldObject.transform.parent = _oldParent;
        heldObject = null;
        foreach (Collider _collider in _heldColliders)
        {
            _collider.enabled = true;
        }
        _heldColliders.Clear();
    }

    public void GrabObject(GameObject _obj)
    {
        if(heldObject != null)
        {
            DropObject();
        }
        _targetOffset = Vector3.zero;
        if(_obj.transform.Find("GrabOffset") != null)
        {
            _targetOffset = _obj.transform.position - _obj.transform.Find("GrabOffset").position;
        }

        _obj.transform.position = transform.position + _targetOffset;
        _targetPosition = _obj.transform.position + _targetOffset;

        if (_obj.GetComponent<Rigidbody>() != null)
        {
            Rigidbody _rb = _obj.GetComponent<Rigidbody>();
            _rb.isKinematic = true;
        }
        Collider[] _collidersInObj = _obj.GetComponentsInChildren<Collider>();
        foreach(Collider _collider in _collidersInObj)
        {
            _heldColliders.Add(_collider);
            _collider.enabled = false;
        }
        heldObject = _obj;
        _pickupTick = Time.time;
    }
    bool CanThrow()
    {
        return heldObject != null;
    }

    public void DestroyHeldObject()
    {
        Destroy(heldObject);
        heldObject = null;
        _heldColliders.Clear();
    }
}
