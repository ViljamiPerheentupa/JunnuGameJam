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
    private GameObject _heldObject;
    private List<Collider> _heldColliders = new List<Collider>();
    private Transform _oldParent;
    private float _pickupTick;

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
            DropObject();
        }
        if(_heldObject != null)
        {
            _heldObject.transform.position = Vector3.Slerp(_heldObject.transform.position, transform.position, Time.deltaTime * _heldObjectPositionDelay);
            _heldObject.transform.rotation = Quaternion.Slerp(_heldObject.transform.rotation, transform.rotation, Time.deltaTime * _heldObjectPositionDelay);
        }
    }

    void Throw()
    {
        Rigidbody _rb = _heldObject.GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.AddForce(Camera.main.transform.forward * _throwForce + Vector3.up * _throwVerticalForce, ForceMode.Impulse);
        _heldObject = null;
        foreach (Collider _collider in _heldColliders)
        {
            _collider.enabled = true;
        }
        _heldColliders.Clear();
    }

    public void DropObject()
    {
        Rigidbody _rb = _heldObject.GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _heldObject.transform.parent = _oldParent;
        _heldObject = null;
        foreach (Collider _collider in _heldColliders)
        {
            _collider.enabled = true;
        }
        _heldColliders.Clear();
    }

    public void GrabObject(GameObject _obj)
    {
        if(_heldObject != null)
        {
            DropObject();
        }
        Vector3 _offset = Vector3.zero;
        if(_obj.transform.Find("GrabOffset") != null)
        {
            _offset = _obj.transform.position - _obj.transform.Find("GrabOffset").position;
        }

        _obj.transform.position = transform.position + _offset;
        _targetPosition = _obj.transform.position + _offset;

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
        _heldObject = _obj;
        _pickupTick = Time.time;
    }
    bool CanThrow()
    {
        return _heldObject != null;
    }
}
